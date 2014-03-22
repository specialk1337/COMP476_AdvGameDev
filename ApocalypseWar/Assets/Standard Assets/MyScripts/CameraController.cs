using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	private bool mouselook;
	private Vector3 prevMousePos;

	// Use this for initialization
	void Start () {
		mouselook = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (1)) {
			mouselook = true;
			prevMousePos = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp (1)) {
			mouselook = false;
		}

		if (mouselook) {
			Vector3 newCameraPos = transform.position;
			newCameraPos.x -= (Input.mousePosition.x - prevMousePos.x) / 10;
			newCameraPos.z -= (Input.mousePosition.y - prevMousePos.y) / 10;
			transform.position = newCameraPos;
			prevMousePos = Input.mousePosition;
		}
	}
}
