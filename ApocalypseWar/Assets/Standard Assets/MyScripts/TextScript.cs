using UnityEngine;
using System.Collections;

public class TextScript : MonoBehaviour {

	public GameObject mainCamera;
	public float displayTime;
	public float speed;
	public float sizeDecay;
	private float timeCounter;

	// Use this for initialization
	void Start() {
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		timeCounter = 0f;
		Billboard();
	}

	public void Init(string text, Color color) {
		gameObject.GetComponent<TextMesh>().text = text;
		gameObject.GetComponent<TextMesh>().color = color;
	}
	
	// Update is called once per frame
	void Update() {

		transform.localScale *= sizeDecay;
		transform.position += transform.up * speed * Time.deltaTime;

		Billboard();

		timeCounter += Time.deltaTime;
		if (timeCounter >= displayTime) {
			Die();
		}
	}

	void Billboard() {
		transform.forward = mainCamera.transform.forward;
	}

	void Die() {
		Destroy (this.gameObject);
	}
}
