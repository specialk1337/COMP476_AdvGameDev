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
	//public float t2t;
	private Vector3 velocity;
	public float separationDistance;

	// Use this for initialization
	void Start () {

	}

	public void initilize(Vector3 Destination, List<GameObject> formationObjects)
	{
		_destination = Destination;
		_formationObjects = new List<GameObject> (formationObjects);
		_pathToDestination.Add (Destination);

	}
	
	// Update is called once per frame
	void Update () {
		move ();
		formations ();
		killMe ();

	}

	private void move()
	{
		/* later logic for path finding could go here*/
		arrive (_destination);
	}
	private void formations()
	{
		/* Basic formation 4xN*/
		if (_formationObjects != null) {
			int numTroops = _formationObjects.Count;
			
			for(int n = 0; n < numTroops/5; ++n)
			{
				int objNum = 0;
				for(int m = -2; m <= 2; ++m)
				{
					if(_formationObjects[objNum+(n*5)]!=null)
					{
						_formationObjects[objNum+(n*5)].GetComponent<MobController> ().target = transform.position + 
							new Vector3(m * separationDistance, 0, n * separationDistance);
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
}
