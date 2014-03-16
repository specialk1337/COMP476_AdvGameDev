using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint : MonoBehaviour {

	private enum ownerControl{Friendly, Neutral, Enemy};

	private GameObject c_object;
	private GameObject c_light;
	private static bool _isSelected;
	private float SelectedLightIntensity;
	private float BaseLightIntensity;

	[SerializeField]ownerControl controlOwner;

	[SerializeField]private List<GameObject> connectedPoints;
	// Use this for initialization
	void Start () {
		c_object = this.gameObject;
		c_light = this.transform.FindChild ("c_Light").gameObject;
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

		if (Input.GetMouseButton (0)) {
				}
			//checkSelected();
	}
	void checkSelected()
	{
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray ();
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (_isSelected)
						_isSelected = false;

		if (Physics.Raycast(ray, out hit)){
			if(hit.transform.gameObject.tag == "ControlPoint")
			{}}

	}

	void OnMouseDown(){
		_isSelected = !_isSelected;

	}

}
