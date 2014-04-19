using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Rect button = new Rect(50,50,100,65);
	public Rect buttonCred = new Rect(180,100, 100, 40);

	public string LoadScreen = "WarZone2";
	public string LoadCredit = "Credits";

	public GUISkin guiSkin;

	private void OnGUI(){
		GUI.skin = guiSkin;
		//GUI.backgroundColor = Color.clear;
		if (GUI.Button(button, "Start"))
			Application.LoadLevel(LoadScreen);
		if (GUI.Button(buttonCred, "Credits"))
			Application.LoadLevel(LoadCredit);
	}
}
