using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// these define the flock's behavior
/// </summary>
public class ControlFlock : MonoBehaviour
{
	public float minVelocity = 5;
	public float maxVelocity = 20;
	public float randomness = 1;
	public int flockSize = 10;
	public FlockUnit prefab;
	public Transform target;
	
	internal Vector3 flockCenter;
	internal Vector3 flockVelocity;
	
	List<FlockUnit> boids = new List<FlockUnit>();
	
	void Start()
	{
		for (int i = 0; i < flockSize; i++)
		{
			FlockUnit boid = Instantiate(prefab, transform.position, transform.rotation) as FlockUnit;
			boid.transform.parent = transform;
			boid.transform.localPosition = new Vector3(
				Random.value * collider.bounds.size.x,
				Random.value * collider.bounds.size.y,
				Random.value * collider.bounds.size.z) - collider.bounds.extents;
			boid.controller = this;
			boids.Add(boid);
		}
	}
	
	void Update()
	{
		Vector3 center = Vector3.zero;
		Vector3 velocity = Vector3.zero;
		foreach (FlockUnit boid in boids)
		{
			center += boid.transform.localPosition;
			velocity += boid.rigidbody.velocity;
		}
		flockCenter = center / flockSize;
		flockVelocity = velocity / flockSize;
	}
}