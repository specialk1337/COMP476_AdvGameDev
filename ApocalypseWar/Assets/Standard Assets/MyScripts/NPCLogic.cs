using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCLogic : MonoBehaviour {
	/*
	private List<GameObject> EnemyCP = new List<GameObject>();

	static private GameObject priorityTarget;
	// Use this for initialization
	void Start () {

		foreach( GameObject CP in GameObject.FindGameObjectsWithTag("ControlPoint"))
			if(CP.GetComponent<ControlPoint>().getcontrolPointState() == ControlPoint.ownerControl.Enemy)
				EnemyCP.Add(CP);
	}
	
	// Update is called once per frame
	void Update () {
		// Basic AI to set arrow to first Not controlled Node
		foreach(GameObject CP in EnemyCP)
		{
			//if(priorityTarget.GetComponent<ControlPoint>().controlPointState == ControlPoint.ownerControl.Enemy)
		//	{
				foreach(GameObject destination in CP.GetComponent<ControlPoint>().connectedPoints)
				{
					if(destination.GetComponent<ControlPoint>().controlPointState == ControlPoint.ownerControl.Friendly)
					{
						priorityTarget = destination;
						CP.GetComponent<ControlPoint>().setArrow(destination);
					}

					if(destination.GetComponent<ControlPoint>().controlPointState == ControlPoint.ownerControl.Neutral)
					{
						CP.GetComponent<ControlPoint>().setArrow(destination);
					}
				}
		//	}
		}
	}*/
}
