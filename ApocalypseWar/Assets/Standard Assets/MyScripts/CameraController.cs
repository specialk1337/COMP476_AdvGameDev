using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	private bool mouselook;
	private Vector3 prevMousePos;
	public float zoomSpeed;
	public float maxZoom;
	public float minZoom;

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
			newCameraPos.x -= (Input.mousePosition.x - prevMousePos.x) / 5;
			newCameraPos.z -= (Input.mousePosition.y - prevMousePos.y) / 5;
			transform.position = newCameraPos;
			prevMousePos = Input.mousePosition;
		}

		// added zoom min and max after demo
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			transform.position += -Vector3.up * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
			transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minZoom, maxZoom), transform.position.z);
		}
	}
}
