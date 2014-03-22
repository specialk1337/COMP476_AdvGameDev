using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint : MonoBehaviour {

	private enum ownerControl{Friendly, Neutral, Enemy};
	private static bool oneSelected = false;

	private GameObject c_object;
	private GameObject c_arrow;
	private GameObject c_light;
	private bool _isSelected;
	private bool firstDelay;
	private float SelectedLightIntensity;
	private float BaseLightIntensity;

	[SerializeField]ownerControl controlOwner;

	[SerializeField]private List<GameObject> connectedPoints;
	[SerializeField]private List<GameObject> arrows;

	// Use this for initialization
	void Start () {
		c_object = this.gameObject;
		c_light = this.transform.FindChild ("c_Light").gameObject;
		c_arrow = this.transform.FindChild ("c_arrow").gameObject;
		ArrowScript arrowS = (ArrowScript)c_arrow.GetComponent (typeof(ArrowScript));
		arrowS.initilize (connectedPoints[0].transform.position, this.transform.position);
		arrows.Add (c_arrow);
		for(int i = 1; i != connectedPoints.Count; ++i)
		{
			GameObject newArrow = GameObject.Instantiate(c_arrow, this.transform.position, Quaternion.identity) as GameObject;
			arrowS = (ArrowScript)newArrow.GetComponent (typeof(ArrowScript));
			newArrow.transform.position = new Vector3(0,0,0);
			arrowS.initilize (connectedPoints[i].transform.position, this.transform.position);
			newArrow.transform.parent = this.gameObject.transform;

			arrows.Add(newArrow);
		}
		BaseLightIntensity = c_light.light.intensity;
		SelectedLightIntensity = BaseLightIntensity + 5;
		controlOwner = ownerControl.Neutral;
		_isSelected = false;

		buildConnections ();

	}
	private void buildConnections()
	{
		/* Hardcoded for now as input from designer. so a "level" builder will need to build the paths. 
		 */
	}
	
	// Update is called once per frame
	void Update () {
		/* */
		switch(controlOwner)
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

		if (Input.GetMouseButton(0)) {
			checkSelected();
		}
			
	}
	void checkSelected()
	{
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (_isSelected)
			_isSelected = false;

		if (Physics.Raycast(ray, out hit))
		{
			if(hit.transform.gameObject.Equals(this.gameObject))
			{
				_isSelected = true;
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
