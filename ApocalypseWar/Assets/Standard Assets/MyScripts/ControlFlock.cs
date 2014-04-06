using UnityEngine;
using System.Collections;

public class ControlFlock : MonoBehaviour {

	public float minVelocity = 1;
	public float maxVelocity = 8;
	public float randomness = 1;
	public int flockSize = 10;

	public GameObject leaderPrefab;
	public GameObject lackey;

	public Vector3 flockCenter;
	public Vector3 flockVelocity;

	private GameObject[] units;
	// Use this for initialization
	void Start () {
		units = new GameObject [flockSize];

		for (int i = 0; i < flockSize; i++) {
			Vector3 position  = new Vector3 (Random.value * collider.bounds.size.x, Random.value * collider.bounds.size.y, Random.value * collider.bounds.size.z) - collider.bounds.extents;

			GameObject unit = Instantiate (leaderPrefab, transform.position, transform.rotation) as GameObject;
			unit.transform.parent = transform;
			unit.transform.localPosition = position;
			unit.GetComponent<FlockUnit>().SetController (gameObject);
			units[i] = unit;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 center = Vector3.zero;
		Vector3 velocity = Vector3.zero;

		foreach (GameObject unit in units) {
			center = center + unit.transform.localPosition;
			velocity = velocity + unit.rigidbody.velocity;
		}

		flockCenter = flockCenter / (flockSize);
		flockVelocity = flockVelocity / (flockSize);
	}
}
