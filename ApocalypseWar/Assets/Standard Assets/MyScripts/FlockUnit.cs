using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockUnit : MonoBehaviour
{


	public float speed = 10f;
	public float maxSpeed = 20f;
	public float maxSteer = .05f;
	

	public float separationWeight = 1f;
	public float alignmentWeight = 1f;
	public float cohesionWeight = 1f;
	public float boundsWeight = 1f;
	
	public float neighborRadius = 50f;
	public float desiredSeparation = 6f;
	
	// velocity influences
	public Vector3 separation;
	public Vector3 alignment;
	public Vector3 cohesion;
	public Vector3 _bounds;
	
	// other members of my swarm
	public List<GameObject> spiders;
	public ControlFlock swarm;
	
		void FixedUpdate ()
		{

				Flock ();
		}
	
	void Start ()
	{
		
	}
	
//	void Update ()
//	{
//		Flock ();
//	}
	
	public  void Flock ()
	{
		
		Vector3 newVelocity = Vector3.zero;
		
		CalculateVelocities ();
		
		//transform.forward = alignment;
		newVelocity += separation * separationWeight;
		newVelocity += alignment * alignmentWeight;
		newVelocity += cohesion * cohesionWeight;
		newVelocity += _bounds * boundsWeight;
		newVelocity = newVelocity * speed;
		newVelocity = rigidbody.velocity + newVelocity;
		newVelocity.y = 0f;
		
		rigidbody.velocity = Limit (newVelocity, maxSpeed);
		transform.LookAt (transform.position + alignment);
		Vector3 temp = transform.position;
		temp.y = 20f;
		this.transform.position = temp;
	}

	public void CalculateVelocities ()
	{
		Vector3 separationSum = Vector3.zero;
		Vector3 alignmentSum = Vector3.zero;
		Vector3 cohesionSum = Vector3.zero;
		Vector3 boundsSum = Vector3.zero;
		
		int separationCount = 0;
		int alignmentCount = 0;
		int cohesionCount = 0;
		int boundsCount = 0;
		
		for (int i = 0; i < this.spiders.Count; i++) {
			if (spiders [i] == null)
				continue;
			
			float distance = Vector3.Distance (transform.position, spiders [i].transform.position);

			if (distance > 0 && distance < desiredSeparation) {
				// calculate vector headed away from myself
				Vector3 direction = transform.position - spiders [i].transform.position;	
				direction.Normalize ();
				direction = direction / distance; // weight by distance
				separationSum += direction;
				separationCount++;
			}

			if (distance > 0 && distance < neighborRadius) {
				alignmentSum += spiders [i].rigidbody.velocity;
				alignmentSum.y = 0f;
				alignmentCount++;
								
				cohesionSum += spiders [i].transform.position;
				cohesionSum.y = 0f;
				cohesionCount++;
			}
			

			Bounds bounds = new Bounds (swarm.transform.position, new Vector3 (swarm.swarmBounds.x, 100f, swarm.swarmBounds.y));
			if (distance > 0 && distance < neighborRadius && !bounds.Contains (spiders [i].transform.position)) {
				Vector3 diff = transform.position - swarm.transform.position;
				if (diff.magnitude > 0) {
					boundsSum += swarm.transform.position;
					boundsSum.y = 0f;
					boundsCount++;
				}
			}
		}
		

		separation = separationCount > 0 ? separationSum / separationCount : separationSum;
		alignment = alignmentCount > 0 ? Limit (alignmentSum / alignmentCount, maxSteer) : alignmentSum;
		cohesion = cohesionCount > 0 ? SteerArrive (cohesionSum / cohesionCount, false) : cohesionSum;
		_bounds = boundsCount > 0 ? SteerArrive (boundsSum / boundsCount, false) : boundsSum;
	}

	public Vector3 SteerArrive (Vector3 target, bool slowDown)
	{
		// the steering vector
		Vector3 steer = Vector3.zero;
		Vector3 targetDirection = target - transform.position;
		float targetDistance = targetDirection.magnitude;
		
		if (targetDistance > 0) {

			targetDirection.Normalize ();
	
			if (slowDown && targetDistance < 100f * speed) {
				targetDirection *= (maxSpeed * targetDistance / (100f * speed));
				targetDirection *= speed;
			} else {
				targetDirection *= maxSpeed;
			}

			steer = targetDirection - rigidbody.velocity;
			steer = Limit (steer, maxSteer);
			steer.y = 0f;
		}
		
		return steer;
	}
	

	public Vector3 Limit (Vector3 v, float max)
	{
		if (v.magnitude > max) {
			return v.normalized * max;
		} else {
			return v;
		}
	}
	

}