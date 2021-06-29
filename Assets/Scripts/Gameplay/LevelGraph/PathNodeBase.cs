using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Only using the editor in the editor
#endif

public class PathNodeBase : MonoBehaviour {
	// The length of the rays used to update the graphs
	const int MAX_SEARCH_DISTANCE = 500;

	// Enum defining the directions of the connectedNodes
	[Flags]
	public enum Directions : byte {
	    North = 1 << 0,	// Pos z
		East =  1 << 1,	// Pos x
		South = 1 << 2,	// Neg z
		West =  1 << 3,	// Neg x
		All = 0x0f,
		Max = 1 << 4,

	}

	// Converts a direction into an index into the nodes array
	public static int toIndex(Directions d){
		switch(d){
			case Directions.North: return 0;
			case Directions.East: return 1;
			case Directions.South: return 2;
			case Directions.West: return 3;
			default: throw new IndexOutOfRangeException("The direction combination " + d + " can't be converted to an array index!");
		}
	}

	// Converts a direction into the direction exactly opposed to it
	public static Directions opposite(Directions d){
		switch(d){
			case Directions.North: return Directions.South;
			case Directions.East: return Directions.West;
			case Directions.South: return Directions.North;
			case Directions.West: return Directions.East;
			default: throw new IndexOutOfRangeException("The direction combination " + d + " doesn't have an opposite!");
		}
	}

	// Function which converts a direction into a vector3 pointed in that direction
	Vector3 calculateLineDirection(Directions d){
		switch(d){
			case Directions.North: return Vector3.forward;
			case Directions.South: return -Vector3.forward;
			case Directions.East: return Vector3.right;
			case Directions.West: return -Vector3.right;
			default: return Vector3.zero;
		}
	}


	// Variable determining the direction that can be connected
	public Directions connectableMask = Directions.All;
	// Array tracking the connected nodes of the graph
    public PathNodeBase[] connectedNodes = new PathNodeBase[4];


	// On awake make sure that the graph connections are updated
	virtual protected void Awake(){
		UpdateLocalGraphConnections();
	}


	// Function which updates the graph connections for this particular node
	protected void UpdateLocalGraphConnections(){
		PathNodeBase[] allNodes = FindObjectsOfType<PathNodeBase>(); // List of PathNodeBase's
		Directions[] toLoopThrough = new Directions[]{ Directions.North, Directions.East, Directions.South, Directions.West }; // List of directions to loop through

		// For each of the cardinal directions
		foreach(Directions direction in toLoopThrough)
			// If we don't have a connection in this direction and we can make connections in this direction...
			if(connectedNodes[toIndex(direction)] == null && (connectableMask & direction) > 0){
				// Variables used to find the closest object in this direction
				float minDist = float.MaxValue;
				PathNodeBase minObj = null;

				// For each of the other nodes...
				foreach(PathNodeBase node in allNodes){
					if(node == this) continue; // Ignoring this node

					// Calculate the positions of the object origins ignoring height
					Vector3 positionNoY = transform.position, nodePositionNoY = node.transform.position;
					positionNoY.y = 0;
					nodePositionNoY.y = 0;

					// Don't bother with this other node if it doesn't fall along a "ray" pointing in the cardinal direction
					if(!Utilities.isBetweenAndColinear(positionNoY, positionNoY + calculateLineDirection(direction) * MAX_SEARCH_DISTANCE, nodePositionNoY)) continue;

					// Calculate the distance to this node
					float distance = (positionNoY - nodePositionNoY).magnitude;
					// If that distance is less than every other distance then this is the closest node in this direction (but only save it if we can connect in this direction)
					if(distance < minDist && (node.connectableMask & opposite(direction)) > 0){
						minDist = distance;
						minObj = node;
					}
				}


				// Set the closest node found as the connected node in that direction
				connectedNodes[toIndex(direction)] = minObj;
				// If the closest node exists, set this node as the closest node to that node in the opposite direction
				if(minObj != null) minObj.connectedNodes[toIndex(opposite(direction))] = this;
			}
	}

	// Struct used to track information used by the findPathTo function
	class NodeDistInfo {
		public PathNodeBase node; // The current node in the graph
		public PathNodeBase previous = null; // The node before this node in the graph
		public float distance; // The distance from the starting node to this node in the graph
		public bool visited = false; // If this node has been visited yet or not

		public NodeDistInfo(PathNodeBase _node, float dist){
			node = _node;
			distance = dist;
		}
	}

	// Finds the (theoretically) shortest path to the target node using Dijkstra's algorithm
	public List<PathNodeBase> findPathTo(PathNodeBase target){
		// Create a list of node distance information
		List<NodeDistInfo> nodeDistances = new List<NodeDistInfo>();

		// Add all of the nodes to a list of node/distance pairs (all of the distances are infinity, except this node which is 0)
		PathNodeBase[] tempAllNodes = FindObjectsOfType<PathNodeBase>();
		foreach(PathNodeBase node in tempAllNodes)
			if(node == this) nodeDistances.Add(new NodeDistInfo(node, 0));
			else nodeDistances.Add(new NodeDistInfo(node, Mathf.Infinity));

		// Variable tracking the current node in the algorithm
		PathNodeBase currentNode = this;

		while(true){
			// Get the node info for the current
			NodeDistInfo currentDistancePair = nodeDistances.Find(pair => pair.node == currentNode);

			// For each neighbor of the current node
			foreach(PathNodeBase neighbor in currentNode.connectedNodes){
				if(neighbor == null) continue; // Make sure it exists

				// Get the neighbor's info
				NodeDistInfo neighborDistancePair = nodeDistances.Find(pair => pair.node == neighbor);
				if(neighborDistancePair.visited) continue; // Skip the neighbor if it has already been visited

				// Check if the distance to the neighbor through this node is less than any other path we have tried so far...
				float newNeighborDist = currentDistancePair.distance + (Utilities.positionNoY(neighbor.transform.position) - Utilities.positionNoY(transform.position)).magnitude;
				if(newNeighborDist < neighborDistancePair.distance){
					// If it is, update the distance and mark this node as the neighbor's previous node
					neighborDistancePair.distance = newNeighborDist;
					neighborDistancePair.previous = currentNode;
				}
			}

			// Mark the current node as visited
			currentDistancePair.visited = true;
			// If the current node is the target, we are done
			if(currentNode == target) break;

			// Find the unvisited node with the smallest distance (which isn't infinity)
			currentNode = null;
			float smallestDist = Mathf.Infinity;
			foreach(var nodePair in nodeDistances)
				if(!nodePair.visited)
					if(nodePair.distance != Mathf.Infinity){
						if(nodePair.distance < smallestDist){
							smallestDist = nodePair.distance;
							currentNode = nodePair.node;
						}
					}
			// If we can't find an unvisited node who's distance isn't infinity, we are done
			if(currentNode == null) break;
		}

		// Once we have given the target a previous node... follow the chain of previous nodes to get the shortest path
		List<PathNodeBase> path = new List<PathNodeBase>();
		while(currentNode != null){
			path.Add(currentNode);
			currentNode = nodeDistances.Find(pair => pair.node == currentNode).previous; // The current node becomes the previous node
		}

		// The path starts with the target node, so we reverse it so it starts with the start point node
		path.Reverse();
		return path;
	}

#if UNITY_EDITOR
	// Menu item which updates the links in the graph
	[MenuItem("CONTEXT/PathNodeBase/Update Graph Connections")]
	public static void UpdateGraphConnections(MenuCommand command){
		ClearGraphConnections(command);

		PathNodeBase[] allNodes = FindObjectsOfType<PathNodeBase>();

		foreach(PathNodeBase node in allNodes)
			node.UpdateLocalGraphConnections();
	}

	// Menu item which clears the links in the graph
	[MenuItem("CONTEXT/PathNodeBase/Clear Graph Connections")]
	public static void ClearGraphConnections(MenuCommand command){
		PathNodeBase[] allNodes = FindObjectsOfType<PathNodeBase>();
		// Clear the current graph
		foreach(PathNodeBase node in allNodes)
			node.connectedNodes = new PathNodeBase[4];
	}
#endif
}
