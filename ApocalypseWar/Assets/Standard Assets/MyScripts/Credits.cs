using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {
	public Rect button = new Rect(50,50,100,65);
	public Rect buttonCred = new Rect(180,100, 100, 40);

	private string teams = "Kevin Cameron\n Nicholas Constantinidis\n Audrey Paiement\n David Huang";

	public string LoadIntro = "Intro";
	
	public GUISkin guiSkin;
	
	private void OnGUI(){
		GUI.skin = guiSkin;
		//GUI.backgroundColor = Color.clear;
		GUI.Box(buttonCred, teams);
		if (GUI.Button(button, "Go Back"))
			Application.LoadLevel(LoadIntro);
	
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
