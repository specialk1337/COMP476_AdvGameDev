using UnityEngine;
using System.Collections;

public class FlockUnit : MonoBehaviour {
	private GameObject Controller;
	private bool start = false;
	private float minVelocity;
	private float maxVelocity;
	private float randomness;
	private GameObject lackey;
	
	void Start ()
	{
		StartCoroutine ("Steer");
	}
	
	IEnumerator Steer ()
	{
		while (true)
		{
			if (start)
			{
				rigidbody.velocity = rigidbody.velocity + Calc () * Time.deltaTime;
				
				// enforce minimum and maximum speeds for the boids
				float speed = rigidbody.velocity.magnitude;
				if (speed > maxVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
				}
				else if (speed < minVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * minVelocity;
				}
			}
			
			float waitTime = Random.Range(0.3f, 0.5f);
			yield return new WaitForSeconds (waitTime);
		}
	}
	
	private Vector3 Calc ()
	{
		Vector3 randomize = new Vector3 ((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1);
		
		randomize.Normalize();
		ControlFlock controlFlock = Controller.GetComponent<ControlFlock>();
		Vector3 flockCenter = controlFlock.flockCenter;
		Vector3 flockVelocity = controlFlock.flockVelocity;
		Vector3 follow = controlFlock.transform.localPosition;
		
		flockCenter = flockCenter - transform.localPosition;
		flockVelocity = flockVelocity - rigidbody.velocity;
		follow = follow - transform.localPosition;
		
		return (flockCenter + flockVelocity + follow * 2 + randomize * randomness);
	}
	
	public void SetController (GameObject theController)
	{
		Controller = theController;
		ControlFlock controlFlock = Controller.GetComponent<ControlFlock>();
		minVelocity = controlFlock.minVelocity;
		maxVelocity = controlFlock.maxVelocity;
		randomness = controlFlock.randomness;
		lackey = controlFlock.lackey;
		start = true;
	}
}
