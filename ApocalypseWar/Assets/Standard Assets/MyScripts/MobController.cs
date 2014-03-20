using UnityEngine;
using System.Collections;

public class MobController : MonoBehaviour {
	public GameObject target;
	public bool flee;
	
	public float maxVelocity;
	public float maxAcceleration;
	public float satisfactionRadius;
	public float t2t;
	public float turnSpeed;
	
	private Vector2 velocity;
	
	// Use this for initialization
	void Start () {
		velocity = new Vector2 (0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.deltaTime;

		SteeringArrive (new Vector2 (target.transform.position.x, target.transform.position.z), t);

		Align (new Vector3(velocity.x,0f,velocity.y), t);
		
		Vector3 position = transform.position;
		
		transform.position = position;
	}
	
	void KinematicArrive(Vector2 target, float t) {
		Vector2 position = new Vector2 (transform.position.x, transform.position.z);
		Vector2 direction = target - position;
		
		if(direction.magnitude > satisfactionRadius) {
			velocity = Mathf.Min (maxVelocity, direction.magnitude / t2t) * direction.normalized;
		}
		else {
			velocity = new Vector2(0, 0);
		}
		
		position = position + velocity * t;
		
		transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void SteeringArrive(Vector2 target, float t) {
		Vector2 position = new Vector2 (transform.position.x, transform.position.z);
		Vector2 direction = target - position;
		Vector2 acceleration = maxAcceleration * direction.normalized;
		
		velocity = velocity + acceleration * t;
		
		if (velocity.magnitude > maxVelocity) {
			velocity = Mathf.Min (maxVelocity, direction.magnitude / t2t) * velocity.normalized;
		}
		
		position = position + velocity * t;
		
		transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void SteeringFlee(Vector2 target, float t) {
		Vector2 position = new Vector2 (transform.position.x, transform.position.z);
		Vector2 direction = position - target;
		Vector2 acceleration = maxAcceleration * direction.normalized;
		
		velocity = velocity + acceleration * t;
		
		if (velocity.magnitude > maxVelocity)
			velocity = maxVelocity * velocity.normalized;
		
		position = position + velocity * t;
		
		transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void KinematicFlee(Vector2 target, float t) {
		Vector2 position = new Vector2 (transform.position.x, transform.position.z);
		Vector2 direction = position - target;
		
		velocity = maxVelocity * direction.normalized;
		
		position = position + velocity * t;
		
		transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void Align(Vector3 targetOrientation, float t) {
		Quaternion targetRotation = Quaternion.LookRotation (targetOrientation, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t * turnSpeed);
	}
}
