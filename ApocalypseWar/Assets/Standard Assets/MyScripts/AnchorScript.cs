using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnchorScript : MonoBehaviour {

	private Vector3 _destination;
	private List<GameObject> _formationObjects;
	private List<Vector3> _pathToDestination = new List<Vector3>();
	public float distance;
	public float maxAcceleration;
	public float maxSpeed;
	public float turnSpeed;
	//public float t2t;
	private Vector3 velocity;
	public float separationDistance;

	// Use this for initialization
	void Start () {

	}

	public void initilize(DDNode target, DDNode start, List<GameObject> formationObjects)
	{
		_destination = target.transform.position;
		_formationObjects = new List<GameObject> (formationObjects);
		_pathToDestination = GetPath(start, target);

	}
	
	// Update is called once per frame
	void Update () {
		move ();
		formations ();
		killMe ();

	}

	private void move()
	{
		if (_pathToDestination.Count == 0){
			arrive (_destination);
			LookAt (_destination);
		}
		else {
			Vector3 dis = transform.position - _pathToDestination[0];
			if(dis.magnitude < 3)
			{
				_pathToDestination.RemoveAt(0);
			}
			if (_pathToDestination.Count > 0){
				arrive (_pathToDestination[0]);
				LookAt (_pathToDestination[0]);
			}
		}
	}
	private void formations()
	{
		/* Basic formation 4xN*/
		if (_formationObjects != null) {
			int numTroops = _formationObjects.Count;
			
			for(int n = 0; n < numTroops/4; ++n)
			{
				int objNum = 0;
				for(int m = -2; m < 2; ++m)
				{
					if(_formationObjects[objNum+(n*4)]!=null)
					{
						_formationObjects[objNum+(n*4)].GetComponent<MobController> ().target = transform.position + 
							transform.right * m * separationDistance + -transform.forward * n * separationDistance;
					}
					objNum++;
				}
			}
		}


	}
	private void seek(Vector3 dest)
	{
		Vector3 direction = dest - transform.position;
		Vector3 acceleration = maxAcceleration * direction.normalized;
		
		velocity = velocity + acceleration * Time.deltaTime;
		
		if (velocity.magnitude > maxSpeed) {
			velocity = maxSpeed * velocity.normalized;
		}
		
		//transform.position = position + velocity * t;
		
		transform.position += new Vector3(velocity.x*Time.deltaTime, 0, velocity.z*Time.deltaTime);
	}
	private void killMe()
	{
		Vector3 dis = transform.position - _destination;
		if(dis.magnitude < 3)
		{
			Destroy(gameObject);
		}
	}

	private void arrive(Vector3 dest)
	{
		Vector3 direction = dest - transform.position;
		Vector3 acceleration = maxAcceleration * direction.normalized;
		
		velocity = velocity + acceleration * Time.deltaTime;
		
		if (velocity.magnitude > maxSpeed) {
			velocity = Mathf.Min (maxSpeed, direction.magnitude / distance) * velocity.normalized;
		}
		
		//transform.position = position + velocity * t;
		
		transform.position += new Vector3(velocity.x*Time.deltaTime, 0, velocity.z*Time.deltaTime);
	}
	
	void Align(Vector3 targetOrientation, float t) {
		Quaternion targetRotation = Quaternion.LookRotation (targetOrientation, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t * turnSpeed);
	}
	
	void LookAt(Vector3 target) {
		Align((target - transform.position).normalized, Time.deltaTime);
	}

	List<Vector3> GetPath (DDNode start, DDNode dest)
	{
		
		List<SearchNode> openList = new List<SearchNode> ();
		List<SearchNode> closedList = new List<SearchNode> ();
		
		openList.Add (new SearchNode(start, null));
		
		SearchNode currentNode = openList[0];
		SearchNode closedToRemove = null;
		bool toVisit;
		
		while (dest != currentNode.node) {
			openList.Sort (delegate(SearchNode c1, SearchNode c2) {
				return Vector3.Distance (dest.transform.position, c1.node.transform.position).CompareTo (Vector3.Distance (dest.transform.position, c2.node.transform.position));
			});
			
			currentNode = openList [0];
			openList.RemoveAt (0);
			closedList.Add (currentNode);
			
			if (currentNode.node != dest) {
				foreach (DDNode neighbour in currentNode.node.neighbours) {
					toVisit = true;
					closedToRemove = null;
					foreach (SearchNode closedNode in closedList) {
						if (closedNode.node == neighbour) {
							if (currentNode.cost + Vector3.Distance (closedNode.node.transform.position, currentNode.node.transform.position) < closedNode.cost) {
								closedToRemove = closedNode;
							} else {
								toVisit = false;
							}
							break;
						}
					}
					
					if (closedToRemove != null) {
						closedList.Remove (closedToRemove);
					}
					
					if (toVisit) {
						openList.Add (new SearchNode(neighbour, currentNode));
					}
				}
			}
		}
		
		List<Vector3> path = new List<Vector3> ();
		while (currentNode.parent != null) {
			path.Add (currentNode.node.transform.position);
			currentNode = currentNode.parent;
		}
		path.Reverse ();
		
		return path;
	}

	private class SearchNode {
		public float cost;
		public DDNode node;
		public SearchNode parent;

		public SearchNode(DDNode node, SearchNode parent) {
			this.node = node;
			this.parent = parent;
			if (parent != null)
				this.cost = parent.cost + Vector3.Distance (node.transform.position, parent.node.transform.position);
			else
				this.cost = 0;
		}
	}
}
