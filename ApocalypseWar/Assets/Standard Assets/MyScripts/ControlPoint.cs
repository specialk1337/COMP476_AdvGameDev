using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint : MonoBehaviour {

	private static bool gameOver = false;

	public enum ownerControl{Friendly, Neutral, Enemy, InConflict};
	private static bool oneSelected = false;
	private static bool DefendPoint = true; //set True when no forward point and troops should defend this node
	public float spawnDelay;
	public float anchorDelay;
	public GameObject mobPrefab;
	public GameObject fireworksPrefab;
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
	private float ActiveRadius = 50; //Find all troops inside this distance

	public float controlCounter; // range [-1, 1] from enemy to friendly
	public float controlDistance;
	public float captureSpeed;
	public float colorFlashSpeed;
	private float colorFlashCounter;
	private GameObject PrimaryTarget = null;
	private List<GameObject> SecondaryTargets = new List<GameObject> ();

	public int healAmount;
	public float healDelay;
	private float healCounter;

	public DDNode Node;

	public ownerControl controlPointState;
	public ownerControl prevControlState;

	[SerializeField]public List<GameObject> connectedPoints;
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
		//c_light.light.color = Color.white;
		BaseLightIntensity = c_light.light.intensity;
		SelectedLightIntensity = BaseLightIntensity + 3f;
		_isSelected = false;

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

		prevControlState = controlPointState;
	}
	public bool isNeutral()
	{
		return controlPointState == ownerControl.Neutral;
	}
	public bool isNPC()
	{
		return controlPointState == ownerControl.Enemy;
	}
	public bool isPlayer()
	{
		return controlPointState == ownerControl.Friendly;
	}

	public List<GameObject> getArrows()
	{
		return arrows;
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
			if (m.GetComponent<MobController>() != null) {
				distance = (m.transform.position - gameObject.transform.position).magnitude;
				if (distance < controlDistance) {
					float force = (controlDistance - distance) / controlDistance;
					controlCounter += force * captureSpeed * t * (m.GetComponent<MobController>().friendly ? 1f : -1f);
				}
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

		if ((controlPointState == ownerControl.Friendly || controlPointState == ownerControl.Enemy) && controlPointState != prevControlState) {
			prevControlState = controlPointState;
			GameObject fireworks = (GameObject)Instantiate (fireworksPrefab, transform.position + transform.up * 10f, Quaternion.identity);
        }
	}

	private void SetColor(Color c) {
		c_light.light.color = c;
	}

	// added after demo
	private static void CheckGameOver() {
		int friendlyMobCount = 0;
		int enemyMobCount = 0;
		GameObject[] mobs = GameObject.FindGameObjectsWithTag ("Mob");
		foreach (GameObject mob in mobs) {
			if (mob.GetComponent<MobController>().friendly) {
				++friendlyMobCount;
			} else {
				++enemyMobCount;
			}
		}
		int friendlyCPCount = 0;
		int enemyCPCount = 0;
		GameObject[] CPs = GameObject.FindGameObjectsWithTag ("ControlPoint");
		foreach (GameObject cp in CPs) {
			if (cp.GetComponent<ControlPoint>().controlPointState == ownerControl.Friendly) {
				++friendlyCPCount;
			} else if (cp.GetComponent<ControlPoint>().controlPointState == ownerControl.Enemy){
				++enemyCPCount;
			}
		}
		if ((friendlyCPCount == 0 && friendlyMobCount == 0) || (enemyCPCount == 0 && enemyMobCount == 0)) {
			gameOver = true;
		}
	}
	
	// Update is called once per frame
	void Update () {

		UpdateControlState (Time.deltaTime);

		colorFlashCounter += colorFlashSpeed * Time.deltaTime;

		switch(controlPointState)
		{
		case ownerControl.Enemy:
			SetColor(Color.red);
			break;
		case ownerControl.Friendly:
			SetColor(Color.green);
			break;
		case ownerControl.Neutral:
			SetColor (Color.yellow);
			break;
		case ownerControl.InConflict:
			SetColor(Color.Lerp(Color.yellow, (controlCounter > 0f ? Color.green : Color.red), Mathf.Sin(colorFlashCounter)));
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
				// added check after demo to stop mobs from spawning after game ends
				if (!gameOver) {
					CheckGameOver();
					Vector3 v = new Vector3 (Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
					GameObject mob = (GameObject)Instantiate (mobPrefab, new Vector3(transform.position.x, 0f, transform.position.z)+v, Quaternion.identity);
					mob.GetComponent<MobController> ().Init(controlPointState == ownerControl.Friendly);
					mob.GetComponent<MobController> ().target = getStandLocation();
				}
				lastSpawn -= spawnDelay;
			} else {
				lastSpawn += Time.deltaTime;
			}
			runAnchor();
			break;
		case ownerControl.InConflict:
		case ownerControl.Neutral:
			lastSpawn = 0f;
			break;
		}

		if(controlPointState == ownerControl.Enemy)
		{
			setArrowsToTargets();
		}
		
		healCounter += Time.deltaTime;
		if (healCounter >= healDelay) {
			healCounter -= healDelay;
            healMobs(healAmount);
        }

	}

	private void setArrowsToTargets()
	{
		bool directlink = false;
		/*If CP is directly connected to a ST then capture it*/
		if(SecondaryTargets.Count > 0)
		{
			foreach(GameObject cp in SecondaryTargets)
			{
				foreach(GameObject connection in connectedPoints)
				{
					if(connection.Equals(cp))
					{
						ActiveTarget = connection;
						directlink = true;
						foreach(GameObject arrow in arrows)
						{
							arrow.SetActive(false);
							if(arrow.GetComponent<ArrowScript>().getPointingAt().Equals(connection))
								arrow.SetActive(true);
						}
					}
				}
			}
		}
		if(PrimaryTarget!=null && !directlink)
		{
			float myDist = Vector3.Distance(PrimaryTarget.transform.position, transform.position);

			foreach(GameObject myConnection in connectedPoints)
			{
				if (myConnection != null) {
					float connectedDist = Vector3.Distance(PrimaryTarget.transform.position, myConnection.transform.position);
					if(connectedDist < myDist)
					{
						ActiveTarget = myConnection;
						directlink = true;
						foreach(GameObject arrow in arrows)
						{
							arrow.SetActive(false);
							if(arrow.GetComponent<ArrowScript>().getPointingAt().Equals(myConnection))
								arrow.SetActive(true);
						}
					}
				}
			}
		}
	}

	public void healMobs(int amount) {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, ActiveRadius);
		if(hitColliders.Length>0)
		{
			foreach(Collider inRangeUnit in hitColliders)
			{
				if(inRangeUnit.transform.tag == "Mob")
				{
					if(	controlPointState == ownerControl.Friendly && inRangeUnit.gameObject.GetComponent<MobController>().friendly ||
					   controlPointState == ownerControl.Enemy && !inRangeUnit.gameObject.GetComponent<MobController>().friendly) 
					{
						inRangeUnit.gameObject.GetComponent<MobController>().HealDamage(amount);
					}
				}
			}
		}
	}

	private Vector3 getStandLocation()
	{
		Vector3 standAt = transform.position;

		Vector3 position = new Vector3(Random.Range(-4.0F, 4.0F), 0, Random.Range(-4.0F, 4.0F));
		Vector3 direction = standAt - (position+standAt);

		standAt = direction.normalized;


		standAt = standAt * Random.Range (1.5F, 4.0F);
		return standAt;
	}

	private void runAnchor()
	{
		if (lastAnchor >= anchorDelay) {
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, ActiveRadius);
			activeTroops.Clear();

			if(hitColliders.Length>0)
			{
				int meleeCount = 0;
				int mageCount = 0;

				foreach(Collider inRangeUnit in hitColliders)
				{
					if(inRangeUnit.transform.tag == "Mob" && meleeCount+mageCount < 16)
					{

						if(	controlPointState == ownerControl.Friendly && inRangeUnit.gameObject.GetComponent<MobController>().friendly ||
							controlPointState == ownerControl.Enemy && !inRangeUnit.gameObject.GetComponent<MobController>().friendly) 
						{
							if(inRangeUnit.transform.gameObject.name.Equals("skeletonMage(Clone)"))
							{
								if(mageCount < 4)
								{
									mageCount++;
									activeTroops.Add (inRangeUnit.gameObject);
								}
							}
							else
							{
								meleeCount++;
								activeTroops.Add (inRangeUnit.gameObject);
							}
						}
					}
				}
				if (activeTroops != null && ActiveTarget != null) {
					GameObject anchor = (GameObject)Instantiate (AnchorPreFab, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity);
					anchor.GetComponent<AnchorScript> ().initilize(ActiveTarget.GetComponent<ControlPoint> ().Node, this.Node, activeTroops);
					lastAnchor = 0;
				}

			}
			
		} else {
			lastAnchor += Time.deltaTime;
		}
	}

	public void setTargets(GameObject _PrimaryTarget, List<GameObject> _SecondaryTargets)
	{
		PrimaryTarget = _PrimaryTarget;
		SecondaryTargets = _SecondaryTargets;
	}

	void checkSelected()
	{
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if(hit.transform.gameObject.Equals(this.gameObject) && controlPointState == ownerControl.Friendly)
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
}
