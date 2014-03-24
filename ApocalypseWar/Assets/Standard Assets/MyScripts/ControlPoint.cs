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

	[SerializeField]ownerControl controlPointState;

	[SerializeField]private List<GameObject> connectedPoints;
	[SerializeField]private List<GameObject> arrows;
	private List<GameObject> activeTroops;

	// Use this for initialization
	void Start () {
		lastSpawn = 0;
		lastAnchor = 0;
		c_object = this.gameObject;
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
		BaseLightIntensity = c_light.light.intensity;
		SelectedLightIntensity = BaseLightIntensity + 5;
		controlPointState = ownerControl.Neutral;
		_isSelected = false;

		buildConnections ();

	}

	public List<GameObject> getArrows()
	{
		return arrows;
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Mob")) {
			activeTroops.Add(other.gameObject);
		}
		if (other.CompareTag ("Anchor")) {
			Destroy(other.gameObject);
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
	
	// Update is called once per frame
	void Update () {
		/* */
		switch(controlPointState)
		{
		case ownerControl.Enemy:
			c_light.light.color = Color.red;
			break;
		case ownerControl.Friendly:
			c_light.light.color = Color.green;
			break;
		case ownerControl.Neutral:
			c_light.light.color = Color.gray;
			break;
		}

		c_light.light.intensity = (_isSelected) ? SelectedLightIntensity : BaseLightIntensity;

		if (Input.GetMouseButtonDown(0)) {
			checkSelected();
			debugCounter++;
			Debug.Log(debugCounter.ToString());
		}

		if (lastSpawn >= spawnDelay) {
			GameObject mob = (GameObject)Instantiate (mobPrefab, transform.position, Quaternion.identity);
			mob.GetComponent<MobController> ().target = this.gameObject;
			activeTroops.Add(mob);
			lastSpawn = 0;
		} else {
			lastSpawn += Time.deltaTime;
		}

		if (lastAnchor >= anchorDelay && controlPointState != ownerControl.InConflict) {
			GameObject anchor = (GameObject)Instantiate (AnchorPreFab, transform.position, Quaternion.identity);
			anchor.GetComponent<MobController> ().target = ActiveTarget;
			List<GameObject> mobWave = activeTroops;
			lastAnchor = 0;
			if(mobWave.Count > 0)
			{
				foreach(GameObject mob in mobWave)
				{
					mob.GetComponent<MobController> ().target = anchor;
				}
				activeTroops.Clear();
			}
		} else {
			lastAnchor += Time.deltaTime;
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
			else
				_isSelected = false;
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
