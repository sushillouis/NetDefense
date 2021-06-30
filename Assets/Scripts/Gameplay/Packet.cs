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

	// Structure which stores the details (color, size, and shape) of a packet
	[Serializable]
	public struct Details {
		public Color color;
		public Size size;
		public Shape shape;

		public Details(Color color, Size size, Shape shape){
			this.color = color;
			this.size = size;
			this.shape = shape;
		}

		// Variable representing the details for a malicious packet
		public static Details maliciousPacketDetails = new Details(Color.Blue, Size.Large, Shape.Cone); // TODO: Replace with global malicious packet settings
		// Generates a random set of details, ensuring that the returned values aren't considered malicious
		public static Details randomNonMaliciousDetails() {
			Details details = new Details(Utilities.randomEnum<Color>(), Utilities.randomEnum<Size>(), Utilities.randomEnum<Shape>());
			if(details == /*TODO: Needs to be swapped for a per turn malicious packet*/ maliciousPacketDetails) details = randomNonMaliciousDetails();
			return details;
		}

		// Object equality (Required to override ==)
		public override bool Equals(System.Object obj) {
			if (obj == null)
				return false;
			Details? o = obj as Details?;
			return Equals(o.Value);
		}

		// Details equality
		public bool Equals(Details o){
			return color == o.color
				&& size == o.size
				&& shape == o.shape;
		}

		// Required to override Equals
		public override int GetHashCode() { return base.GetHashCode(); }

		// Equality Operator
		public static bool operator ==(Details a, Details b){ return a.Equals(b); }
		// Inequality Operator (Required if == is overriden)
		public static bool operator !=(Details a, Details b){ return !a.Equals(b); }
	}

	// This packet's mesh filter
	public MeshFilter filter;
	// This packet's mesh renderer
	new public MeshRenderer renderer;
	// This packet's rigidbody
	new public Rigidbody rigidbody;
	// This object's selection cylinder
	public GameObject selectionCylinder;

	// List of meshes which define this packet's shape
	public Mesh[] meshes;
	// The material that the rendering is based on
	public Material material;
	// List of colors for the packet to become
	public UnityEngine.Color[] colors;

	// Property defining the packet's details (color, size, shape) (automatically network synced)
	[SerializeField]
	Details _details;
	public Details details {
		get => _details;
		set => SetProperties(value, _movementSpeed, _isMalicious);
	}

	// Property defining the packet's movement speed (automatically network synced)
	[SerializeField]
	float _movementSpeed = 1;
	public float movementSpeed {
		get => _movementSpeed;
		set => SetProperties(_details, value, _isMalicious);
	}

	// Property defining if the packet is malicious (automatically network synced)
	[SerializeField]
	bool _isMalicious = false;
	public bool isMalicious {
		get => _isMalicious;
		set => SetProperties(_details, _movementSpeed, value);
	}


	// Nodes defining the start and end point of the packet's journey
	public PathNodeBase startPoint, destination; // TODO: do we actually care about the startPoint and destination? Or do we only care about the path?
	// Path to get from the start point to the destination point
	public List<PathNodeBase> path = null;


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

	// Function called whenever the packet interacts with another trigger
	void OnTriggerEnter(Collider collider){
		if(!NetworkingManager.isHost) return;

		// If the trigger was a destination...
		if(collider.transform.tag == "Destination"){
			// TODO: Update scoring information

			// Destroy the packet after it has had a few seconds to enter the destination
			StartCoroutine(DestroyAfterSeconds(1));
		}
	}


	// -- Network Synchronization Functions --


	// Synchronizes the properties across the network
	public void SetProperties(Color color, Size size, Shape shape, float movementSpeed, bool isMalicious){ SetProperties(color, size, shape, movementSpeed, isMalicious); }
	public void SetProperties(Details details, float movementSpeed, bool isMalicious){ photonView.RPC("RPC_Packet_SetProperties", RpcTarget.AllBuffered, details.color, details.size, details.shape, movementSpeed, isMalicious); }
	[PunRPC] void RPC_Packet_SetProperties(Color color, Size size, Shape shape, float movementSpeed, bool isMalicious){
		// Ensure the local properties match the remote ones
		if(isMalicious) _details = Details.maliciousPacketDetails;  // TODO: Global malicious packet settings
		else _details = new  Details(color, size, shape);
		_movementSpeed = movementSpeed;
		_isMalicious = isMalicious;

		// Set the mesh based on the shape
		filter.mesh = meshes[(int)details.shape];

		// Get the list of materials off the mesh
		Material[] mats = renderer.materials;
		// Replace the first one with a new instance of the packet material
		mats[0] = new Material(material);
		mats[0].SetColor( "_EmissionColor", colors[(int)details.color] * ((int)details.size) * .5f ); // Set the packet color and emmisive intensity
		// Copy the material changes back to the model
		renderer.materials = mats;

		// Set the size of the packet
		switch(details.size){
			case Size.Small: transform.localScale = Utilities.toVec(.1f); break;
			case Size.Medium: transform.localScale = Utilities.toVec(.2f); break;
			case Size.Large: transform.localScale = Utilities.toVec(.3f); break;
		}

		// TODO: this check should also be based off of difficulty
		if(isMalicious) selectionCylinder.SetActive(true);
		else selectionCylinder.SetActive(false);
	}

	// Wrapper function which calls all of the functions needed to setup this packet's path
	public void setStartDestinationAndPath(PathNodeBase startPoint, PathNodeBase destination){
		SetStartPoint(startPoint);
		SetDestination(destination);
		InitPath();
	}

	// Sets the start point (network synced)
	public void SetStartPoint(PathNodeBase startPoint){ photonView.RPC("RPC_Packet_SetStartPoint", RpcTarget.AllBuffered, startPoint.name); }
	[PunRPC] void RPC_Packet_SetStartPoint(string startPointName){
		startPoint = GameObject.Find(startPointName).GetComponent<PathNodeBase>();

		// If we are the host make sure that the object is properly positioned
		if(NetworkingManager.isHost)
			transform.position = startPoint.transform.position;
	}

	// Sets the destination (network synced)
	public void SetDestination(PathNodeBase Destination){ photonView.RPC("RPC_Packet_SetDestination", RpcTarget.AllBuffered, Destination.name); }
	[PunRPC] void RPC_Packet_SetDestination(string DestinationName){
		destination = GameObject.Find(DestinationName).GetComponent<PathNodeBase>();
	}

	// Generates a path from the start point to the destination (network synced)
	public void InitPath(){ photonView.RPC("RPC_Packet_InitPath", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_Packet_InitPath(){
		path = startPoint.findPathTo(destination);
	}


	// Coroutine which destroys the packet after the specified number of seconds
	IEnumerator DestroyAfterSeconds(float seconds){
		yield return new WaitForSeconds(seconds);
		Destroy();
	}
	// Destroys the packet (network synced)
	public void Destroy(){ photonView.RPC("RPC_Packet_Destroy", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_Packet_Destroy(){
		if(!NetworkingManager.isHost) return;

		PhotonNetwork.Destroy(gameObject);
	}

}
