using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

	private bool _isSelected;
	// Use this for initialization
	void Start () {
		_isSelected = false;
	}

	public void initilize(Vector3 pointAt, Vector3 pointFrom)
	{

		float scale = 3.5f;
		//transform.localPosition = pointFrom;
		//Vector3 direction = pointFrom - pointAt;
		Vector3 direction = pointAt - pointFrom;
		direction = direction.normalized;
		transform.localPosition = new Vector3(scale*direction.x, transform.position.y, scale*direction.z);
		//transform.position.z += direction.z;
		transform.LookAt (pointAt);
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		_isSelected = !_isSelected;
	}
}
