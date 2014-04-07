using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

	//private bool _isSelected;
	private GameObject pointingAt;
	// Use this for initialization
	void Start () {
		//_isSelected = false;
	}

	public GameObject getPointingAt()
	{
		return pointingAt;
		}
	public void initilize(GameObject pointAt, Vector3 pointFrom)
	{

		float scale = 5f;
		pointingAt = pointAt;
		//transform.localPosition = pointFrom;
		//Vector3 direction = pointFrom - pointAt;
		Vector3 direction = pointAt.transform.position - pointFrom;
		direction = direction.normalized;
		transform.localPosition = new Vector3(scale*direction.x, 2f, scale*direction.z);
		//transform.position.z += direction.z;
		transform.LookAt (pointAt.transform.position);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
