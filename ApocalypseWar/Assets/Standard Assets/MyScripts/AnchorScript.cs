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
	public float separationWeight;
	public float alignmentWeight;
	public float cohesionWeight;

	// Use this for initialization
	void Start () {
		separationWeight = 0.1f;
		alignmentWeight = 0.1f;
		cohesionWeight = 0.2f;
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
		/*
		int numTroops = _formationObjects.Count;

		for(int n = 0; n < numTroops/4; ++n)
		{
			int objNum = 0;
			for(int m = -2; m != 2; ++m)
			{
				if(_formationObjects[objNum+(n*4)]!=null)
				{
					_formationObjects[objNum+(n*4)].GetComponent<MobController> ().target = transform.position + new Vector3(m, 0, n);
				}
				objNum++;
			}
		}
		*/

		Vector3 separation = Vector3.zero;
		Vector3 alignment = Vector3.zero;
		Vector3 cohesion = Vector3.zero;
		Vector3 v = Vector3.zero;

		// o = current mob, n = neighbor mobs
		foreach (GameObject o in _formationObjects) {
			foreach (GameObject n in _formationObjects) {
				if (!o.Equals(n)) {
					// separation
					v = (o.transform.position - n.transform.position);
					float mag = v.magnitude;
					v = v.normalized * (1 / (mag + 0.01f));
					separation += v;

					// alignment
					if (n.GetComponent<MobController>().velocity.magnitude > 0.1f) {
						alignment += n.GetComponent<MobController>().velocity.normalized;
					}
					
					// cohesion
					cohesion += n.transform.position;
				}
			}

			Vector3.ClampMagnitude(separation, o.GetComponent<MobController>().maxVelocity);

			alignment.Normalize();

			if (_formationObjects.Count > 1) {
				cohesion /= (float)(_formationObjects.Count - 1);
			}
			cohesion -= o.transform.position;

			Debug.Log (separation);
			Debug.Log (alignment);
			Debug.Log (cohesion);

			o.GetComponent<MobController>().velocity = Vector3.ClampMagnitude(_destination - o.transform.position, o.GetComponent<MobController>().maxVelocity);
			o.GetComponent<MobController>().velocity += separation * separationWeight + alignment * alignmentWeight + cohesion * cohesionWeight;
			o.GetComponent<MobController>().target = o.transform.position + o.GetComponent<MobController>().velocity;
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
		if(dis.magnitude < 1.5)
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
