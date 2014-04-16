using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// these define the flock's behavior
/// </summary>
public class ControlFlock : MonoBehaviour
{
		public int spiderCount = 50;
		public float spawnRadius = 100f;
		public List<GameObject> spiders;
	
		public Vector2 swarmBounds = new Vector2 (300f, 300f);
	
		public GameObject prefab;
	
		// Use this for initialization
		protected virtual void Start ()
		{
	
		
				// instantiate the spiders
				GameObject spiderTmp;
				spiders = new List<GameObject> ();
				for (int i = 0; i < spiderCount; i++) {
						spiderTmp = (GameObject)GameObject.Instantiate (prefab);
						FlockUnit db = spiderTmp.GetComponent<FlockUnit> ();
						db.spiders = this.spiders;
						db.swarm = this;
			
						// spawn inside circle
						Vector2 pos = new Vector2 (transform.position.x, transform.position.z) + Random.insideUnitCircle * spawnRadius;
						spiderTmp.transform.position = new Vector3 (pos.x, transform.position.y, pos.y);
						spiderTmp.transform.parent = transform;
			
						spiders.Add (spiderTmp);
				}
		}
	
		// Update is called once per frame
		protected virtual void Update ()
		{
		
		}
}