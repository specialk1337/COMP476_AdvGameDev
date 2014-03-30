using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint : MonoBehaviour {

	private enum ownerControl{Friendly, Neutral, Enemy, InConflict};
	private static bool oneSelected = false;
	private static bool DefendPoint = true; //set True when no forward point and troops should defend this node
	public float spawnDelay;
	public float anchorDelay;
	public GameObject mobPrefab;
	[SerializeField]private GameObject ArrowPreFab;
	[SerializeField]private GameObject AnchorPreFab;
	private GameObject ActiveTarget;

	private int debugCounter = 0;

	private GameObject c_object;
	private GameObject c_arrow;
	private GameObject c_light;
	private bool _isSelected;
	private bool firstDelay;
	private float SelectedLightIntensity;
	private float BaseLightIntensity;
	private float lastSpawn;
	private float lastAnchor;

	public float controlCounter; // range [-1, 1] from enemy to friendly
	public float controlDistance;
	public float captureSpeed;

	[SerializeField]ownerControl controlPointState;

	[SerializeField]private List<GameObject> connectedPoints;
	[SerializeField]private List<GameObject> arrows;
	private List<GameObject> activeTroops;

	// Use this for initialization
	void Start () {
		lastSpawn = 0;
		lastAnchor = 0;
		c_object = this.transform.FindChild("c_Structure").gameObject;
		c_light = this.transform.FindChild ("c_Light").gameObject;
		activeTroops = new List<GameObject>();
		//ArrowScript arrowS = (ArrowScript)c_arrow.GetComponent (typeof(ArrowScript));

		ActiveTarget = this.gameObject;
		for(int i = 0; i < connectedPoints.Count; ++i)
		{
			if (connectedPoints[i] != null) {
				GameObject newArrow = GameObject.Instantiate(ArrowPreFab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
				newArrow.transform.parent = this.gameObject.transform;
				newArrow.transform.localPosition = new Vector3(0,0,0);
				ArrowScript arrowS = (ArrowScript)newArrow.GetComponent (typeof(ArrowScript));
				arrowS = (ArrowScript)newArrow.GetComponent (typeof(ArrowScript));
				arrowS.initilize (connectedPoints[i], this.transform.position);
				arrows.Add(newArrow);
				newArrow.SetActive(false);
			}
		}
		c_light.light.color = Color.white;
		BaseLightIntensity = c_light.light.intensity;
		SelectedLightIntensity = BaseLightIntensity + 1.75f;
		_isSelected = false;

		controlDistance = 3f;
		captureSpeed = 0.1f;

		// state chosen manually in inspector
		switch (controlPointState)
		{
		case ownerControl.Friendly:
			controlCounter = 1f;
			break;
		case ownerControl.Enemy:
			controlCounter = -1f;
			break;
		default:
			controlCounter = 0f;
			break;
		}

		buildConnections ();

	}

	public List<GameObject> getArrows()
	{
		return arrows;
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Mob")) {
			if (controlPointState == ownerControl.Friendly && other.gameObject.GetComponent<MobController>().friendly ||
			    controlPointState == ownerControl.Enemy && !other.gameObject.GetComponent<MobController>().friendly) {
				activeTroops.Add(other.gameObject);
			}
		}
	}

	private void arrowSelector()
	{

	}

	private void buildConnections()
	{
		/* Hardcoded for now as input from designer. so a "level" builder will need to build the paths. 
		 */
	}

	private void UpdateControlState(float t) {
		GameObject[] mobs = GameObject.FindGameObjectsWithTag ("Mob");
		float distance;
		foreach (GameObject m in mobs) {
			distance = (m.transform.position - gameObject.transform.position).magnitude;
			if (distance < controlDistance) {
				float force = (controlDistance - distance) / controlDistance;
				controlCounter += force * captureSpeed * t * (m.GetComponent<MobController>().friendly ? 1f : -1f);
			}
		}
		controlCounter = Mathf.Clamp (controlCounter, -1f, 1f);
		if (controlCounter >= 1f) {
			controlPointState = ownerControl.Friendly;
		} else if (controlCounter <= -1f) {
			controlPointState = ownerControl.Enemy;
		} else if (controlCounter == 0f) {
			controlPointState = ownerControl.Neutral;
		} else {
			controlPointState = ownerControl.InConflict;
		}
	}
	
	// Update is called once per frame
	void Update () {

		UpdateControlState (Time.deltaTime);

		switch(controlPointState)
		{
		case ownerControl.Enemy:
			c_object.renderer.material.color = Color.red;
			break;
		case ownerControl.Friendly:
			c_object.renderer.material.color = Color.green;
			break;
		case ownerControl.Neutral:
		case ownerControl.InConflict:
			c_object.renderer.material.color = Color.Lerp(Color.red, Color.green, (controlCounter + 1) / 2f);
			break;
		}

		c_light.light.intensity = (_isSelected) ? SelectedLightIntensity : BaseLightIntensity;

		if (Input.GetMouseButtonDown(0)) {
			checkSelected();
		}
		
		switch(controlPointState)
		{
		case ownerControl.Friendly:
		case ownerControl.Enemy:
			if (lastSpawn >= spawnDelay) {
				Vector3 v = new Vector3 (Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
				GameObject mob = (GameObject)Instantiate (mobPrefab, transform.position + v, Quaternion.identity);
				mob.GetComponent<MobController> ().friendly = (controlPointState == ownerControl.Friendly);
				mob.GetComponent<MobController> ().target = this.transform.position;
				//activeTroops.Add(mob);
				lastSpawn -= spawnDelay;
			} else {
				lastSpawn += Time.deltaTime;
			}
			if (lastAnchor >= anchorDelay) {
				activeTroops.Clear();
				GameObject[] mobs = GameObject.FindGameObjectsWithTag ("Mob");
				float distance;
				foreach (GameObject m in mobs) {
					distance = (m.transform.position - gameObject.transform.position).magnitude;
					if (distance < controlDistance &&
					    controlPointState == ownerControl.Friendly && m.GetComponent<MobController>().friendly ||
					    controlPointState == ownerControl.Enemy && !m.GetComponent<MobController>().friendly) {
						activeTroops.Add (m);
					}
				}
				GameObject anchor = (GameObject)Instantiate (AnchorPreFab, transform.position, Quaternion.identity);
				anchor.GetComponent<AnchorScript> ().initilize(ActiveTarget.transform.position, activeTroops);
				lastAnchor = 0;
			
			} else {
				lastAnchor += Time.deltaTime;
			}
			break;
		case ownerControl.InConflict:
		case ownerControl.Neutral:
			lastSpawn = 0f;
			break;
		}			
	}
	void checkSelected()
	{
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if(hit.transform.gameObject.Equals(this.gameObject))
			{
				if(_isSelected)
				{
					DefendPoint = true;
					ActiveTarget = this.gameObject;
					_isSelected = false;
					foreach( GameObject arrow in arrows)
					{
						arrow.SetActive(false);
					}
				}
				else
				{
					_isSelected = true;
					foreach( GameObject arrow in arrows)
					{
						arrow.SetActive(true);
					}
				}
			}
			else if(hit.transform.gameObject.tag == "Arrow")
			{
				GameObject hitArrowParent = hit.transform.gameObject;

				/* Should be moved to arrow class for optimizations, but what ever. */
				if(hitArrowParent.transform.parent.gameObject.Equals(this.gameObject)){
					ControlPoint thisScript = (ControlPoint)hitArrowParent.transform.parent.GetComponent(typeof(ControlPoint));

					foreach( GameObject arrow in thisScript.getArrows())
					{
						arrow.SetActive(false);
					}
					ArrowScript arrowS = (ArrowScript)hit.transform.gameObject.GetComponent (typeof(ArrowScript));
					ActiveTarget = arrowS.getPointingAt();
					hit.transform.gameObject.SetActive(true);
					_isSelected = false;
				}
			}
			else {
				if (_isSelected) {
					DefendPoint = true;
					ActiveTarget = this.gameObject;
					foreach( GameObject arrow in arrows)
					{
						arrow.SetActive(false);
					}
				}
				_isSelected = false;
			}
		}

	}

	/*void OnMouseDown(){
		firstDelay = true;
		if(!oneSelected){
			_isSelected = true;
			oneSelected = true; // Static valuse to limit 1 object selected at a time
		}
		else if(oneSelected && _isSelected)
		{
			_isSelected = false;
			oneSelected = false;
		}
	}*/
}
