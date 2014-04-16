using UnityEngine;
using System.Collections;

public class FireworksScript : MonoBehaviour {

	public float deathDelay;
	private float deathCounter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		deathCounter += Time.deltaTime;
		if (deathCounter >= deathDelay) {
			Destroy(gameObject);
		}
	}
}
