using UnityEngine;
using System.Collections;
using System.Linq;

public class Boid : MonoBehaviour {

	public enum BoidState{
		Alignment = 0, 
		Cohesion = 1,
		Separation = 2,
		Ignore = -1
	}

	public string tagSearch;
	public float searchRadius;

	public int packSize = 10;
	public GameObject leader;
	private GameObject[] units;

	public BoidState bState = BoidState.Ignore;
	public float separationRadius;


	//public List<GameObject> pack = new List<GameObject>();
	// Use this for initialization
	void Start () {
		units = new GameObject[packSize];
	}
	
	// Update is called once per frame
	void Update () {
		var goa = GameObject.FindGameObjectsWithTag(tagSearch);

//		foreach(GameObject gobj in goa){
//			if (Vector3.Distance
//		}
	}
}
