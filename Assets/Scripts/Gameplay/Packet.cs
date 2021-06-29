using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Packet : MonoBehaviourPun {
	// Enum defining a packet's color
	[Serializable]
	public enum Color {
		Blue,
		Pink,
		Green
	}

	// Enum defining a packet's size
	[Serializable]
	public enum Size {
		Large = 1,
		Medium = 4,
		Small = 6,
	}

	// Enum defining a packet's shape
	[Serializable]
	public enum Shape {
		Cube,
		Sphere,
		Cone
	}

	// This packet's mesh filter
	public MeshFilter filter;
	// This packet's mesh renderer
	new public MeshRenderer renderer;
	// This packet's rigidbody
	new public Rigidbody rigidbody;

	// List of meshes which define this packet's shape
	public Mesh[] meshes;
	// The material that the rendering is based on
	public Material material;
	// List of colors for the packet to become
	public UnityEngine.Color[] colors;

	// Property defining the packet's color (automatically network synced)
	[SerializeField]
	Color _color;
	public Color color {
		get => _color;
		set => SetProperties(value, _size, _shape, _movementSpeed);
	}

	// Property defining the packet's size (automatically network synced)
	[SerializeField]
	Size _size;
	public Size size {
		get => _size;
		set => SetProperties(_color, value, _shape, _movementSpeed);
	}

	// Property defining the packet's shape (automatically network synced)
	[SerializeField]
	Shape _shape;
	public Shape shape {
		get => _shape;
		set => SetProperties(_color, _size, value, _movementSpeed);
	}

	// Property defining the packet's movement speed (automatically network synced)
	[SerializeField]
	float _movementSpeed = 1;
	public float movementSpeed {
		get => _movementSpeed;
		set => SetProperties(_color, _size, _shape, value);
	}


	// Nodes defining the start and end point of the packet's journey
	public PathNodeBase startPoint, destination; // TODO: do we actually care about the startPoint and destination? Or do we only care about the path?
	// Path to get from the start point to the destination point
	public List<PathNodeBase> path = null;



    // Start is called before the first frame update
    void Start() {
		SetProperties(Color.Blue, Size.Small, Shape.Sphere, 1);
    }

	// Manages packet movement
	void Update() {
		// Packet movement is controlled by the host
		if(!NetworkingManager.isHost) return;

		// If we don't have a path, create one (network synced)
		if(path == null || path.Count == 0) setStartDestinationAndPath(startPoint, destination);

		// Follow the path
		FollowPath();
	}


	// Function which moves the packet along the path
	int pathIndex = 1; // Variable defining the next waypoint in the path
	float lastDistance = Mathf.Infinity; // Variable defining how far this packet was from the next waypoint last frame
	void FollowPath(){
		// Determine the direction we should be heading in
		Vector3 direction = (Utilities.positionNoY(path[pathIndex].transform.position) - Utilities.positionNoY(path[pathIndex - 1].transform.position)).normalized;
		// Apply that direction to the rigidbody's velocity
		rigidbody.velocity = direction * movementSpeed;
		// Calculate the distance to the next waypoint
		float distance = Mathf.Abs((Utilities.positionNoY(path[pathIndex].transform.position) - Utilities.positionNoY(transform.position)).magnitude);

		// If we have started moving backwards...
		if(distance > lastDistance){
			// Snap to the current waypoint
			transform.position = Utilities.positionSetY(path[pathIndex].transform.position, transform.position.y);

			// Look at the next waypoint (if it exists)
			if(pathIndex + 1 < path.Count){
				++pathIndex; // Updates the current waypoint to the next waypoint
				transform.LookAt(Utilities.positionSetY(path[pathIndex].transform.position, transform.position.y));
			}

			// Reset previous distance
			lastDistance = Mathf.Infinity;
		// Otherwise... update the previous distance
		} else lastDistance = distance;
	}


	// -- Network Synchronization Functions --


	// Synchronizes the properties across the network
	public void SetProperties(Color color, Size size, Shape shape, float movementSpeed){ photonView.RPC("RPC_SetProperties", RpcTarget.AllBuffered, color, size, shape, movementSpeed); }
	[PunRPC] void RPC_SetProperties(Color color, Size size, Shape shape, float movementSpeed){
		// Ensure the local properties match the remote ones
		_color = color;
		_size = size;
		_shape = shape;
		_movementSpeed = movementSpeed;

		// Set the mesh based on the shape
		filter.mesh = meshes[(int)shape];

		// Get the list of materials off the mesh
		Material[] mats = renderer.materials;
		// Replace the first one with a new instance of the packet material
		mats[0] = new Material(material);
		mats[0].SetColor( "_EmissionColor", colors[(int) color] * ((int)size) * .5f ); // Set the packet color and emmisive intensity
		// Copy the material changes back to the model
		renderer.materials = mats;

		// Set the size of the packet
		switch(size){
			case Size.Small: transform.localScale = Utilities.toVec(.1f); break;
			case Size.Medium: transform.localScale = Utilities.toVec(.2f); break;
			case Size.Large: transform.localScale = Utilities.toVec(.3f); break;
		}
	}

	// Wrapper function which calls all of the functions needed to setup this packet's path
	public void setStartDestinationAndPath(PathNodeBase startPoint, PathNodeBase destination){
		SetStartPoint(startPoint);
		SetDestination(destination);
		InitPath();
	}

	// Sets the start point (network synced)
	public void SetStartPoint(PathNodeBase startPoint){ photonView.RPC("RPC_SetStartPoint", RpcTarget.AllBuffered, startPoint.name); }
	[PunRPC] void RPC_SetStartPoint(string startPointName){
		startPoint = GameObject.Find(startPointName).GetComponent<PathNodeBase>();

		// If we are the host make sure that the object is properly positioned
		if(NetworkingManager.isHost)
			transform.position = startPoint.transform.position;
	}

	// Sets the destination (network synced)
	public void SetDestination(PathNodeBase Destination){ photonView.RPC("RPC_SetDestination", RpcTarget.AllBuffered, Destination.name); }
	[PunRPC] void RPC_SetDestination(string DestinationName){
		destination = GameObject.Find(DestinationName).GetComponent<PathNodeBase>();
	}

	// Generates a path from the start point to the destination (network synced)
	public void InitPath(){ photonView.RPC("RPC_InitPath", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_InitPath(){
		path = startPoint.findPathTo(destination);
	}

}
