using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCLogic : MonoBehaviour {

	private List<GameObject> MyCPs = new List<GameObject>();
	private List<GameObject> NeutCPs = new List<GameObject>();
	private List<GameObject> PlayerCPs = new List<GameObject>();

	/* Primary Target High value*/
	private GameObject PrimaryTarget;

	/* Secondary targets - Not as valuable */

	private List<GameObject> SecondaryTargets = new List<GameObject> ();

	public bool Aggressive;
	public bool Deffensive;
	private int MyTroopCount;
	private int PlayerTotalKnownTroopsCount;
	private int PlayerLargestArmyCount;
	private GameObject PlayerLargetArmyLocation;
	public float radius;

	static private GameObject priorityTarget;
	// Use this for initialization
	void Start () {
		Aggressive = false;
		Deffensive = false;
		MyTroopCount = 0;
		PlayerTotalKnownTroopsCount = 0;
		PlayerLargestArmyCount = 0;
		PlayerLargetArmyLocation = null;
		PrimaryTarget = null;
	}
	
	// Update is called once per frame
	float EvaluateLast = 1;
	float EvaluateDelay = 15;

	void Update () {
		// Basic AI to set arrow to first Not controlled Node

		if (EvaluateLast >= EvaluateDelay) {
			EvaluateMySelf(); //internal knowlage
			EvaluateExternal();

			pickTargets();

			SetArrows();

		} else {
			EvaluateLast += Time.deltaTime;
		}
	}
	
	private void EvaluateMySelf()
	{
		MyCPs.Clear ();

		foreach(GameObject CP in GameObject.FindGameObjectsWithTag("ControlPoint"))
		{

			if(CP.GetComponent<ControlPoint>().isNPC())
			{
				MyCPs.Add (CP);
				Collider[] hitColliders = Physics.OverlapSphere(CP.transform.position, radius);

				foreach(Collider inRangeUnit in hitColliders)
				{
					if(inRangeUnit.transform.tag == "Mob")
					{
						if(!inRangeUnit.transform.gameObject.GetComponent<MobController>().friendly)
						{
							MyTroopCount++;
						}
					}
				}
			}
		}
	}
	private void SetArrows()
	{
		foreach(GameObject CP in MyCPs)
		{
			CP.GetComponent<ControlPoint>().setTargets(PrimaryTarget, SecondaryTargets);
		}
	}

	private void EvaluateExternal()
	{
		NeutCPs.Clear ();
		PlayerCPs.Clear ();
		PlayerTotalKnownTroopsCount = 0;
		PlayerLargestArmyCount = 0;
		PlayerLargetArmyLocation = null;

		foreach(GameObject CP in MyCPs)
		{
			if(CP != null)
			{
				foreach(GameObject destination in CP.GetComponent<ControlPoint>().connectedPoints)
				{
					if(destination != null)
					{
						if(destination.GetComponent<ControlPoint>().isNeutral())
						{
							NeutCPs.Add(destination);
						}
						if(destination.GetComponent<ControlPoint>().isPlayer())
						{
							PlayerCPs.Add(destination);
							Collider[] hitColliders = Physics.OverlapSphere(destination.transform.position, radius);

							int playerTroops = 0;
							foreach(Collider inRangeUnit in hitColliders)
							{
								if(inRangeUnit.transform.tag == "Mob")
								{
									if(inRangeUnit.transform.gameObject.GetComponent<MobController>().friendly)
									{
										PlayerTotalKnownTroopsCount++;
										playerTroops++;
									}
								}
							}
							if(playerTroops > PlayerLargestArmyCount)
							{
								PlayerLargestArmyCount = playerTroops;
								PlayerLargetArmyLocation = destination;
							}
						}
					}
				}
			}
		}
	}
	private void pickTargets()
	{
		SecondaryTargets.Clear();
		PrimaryTarget = null;

		if(PlayerCPs.Count == 0)
		{
			/* We dont know anything about the player movements
			 * Lets expand to Neutral CPs
			 * */
			float distanceToTarget = 0;
			float shortestDistance = 100000;

			if(NeutCPs.Count > 0)
			{
				foreach(GameObject CP in MyCPs)
				{
					if(CP != null)
					{
						foreach(GameObject destination in CP.GetComponent<ControlPoint>().connectedPoints)
						{
							if(destination != null && destination.GetComponent<ControlPoint>().isNeutral())
							{
								if(Vector3.Distance(destination.transform.position, CP.transform.position) < shortestDistance)
								{
									shortestDistance = Vector3.Distance(destination.transform.position, CP.transform.position);
									if(PrimaryTarget!=null)
									{
										SecondaryTargets.Add(PrimaryTarget);
									}
									PrimaryTarget = destination;
								}
								else
								{
									SecondaryTargets.Add(destination);
								}
							}
						}
					}
				}
			}

		}

		if(NeutCPs.Count == 0 && PlayerCPs.Count > 0)
		{
			/*
			 *  No more known Neut points left, time to war
			 */
			foreach(GameObject CP in MyCPs)
			{
				if(CP != null)
				{
					foreach(GameObject destination in CP.GetComponent<ControlPoint>().connectedPoints)
					{
						if(destination.Equals(PlayerLargetArmyLocation))
						{
							PrimaryTarget = CP;
							Deffensive = true;

							Collider[] hitColliders = Physics.OverlapSphere(CP.transform.position, radius);
							
							int NPCTroopsInDef = 0;
							foreach(Collider inRangeUnit in hitColliders)
							{
								if(inRangeUnit.transform.tag == "Mob")
								{
									if(!inRangeUnit.transform.gameObject.GetComponent<MobController>().friendly)
									{
										NPCTroopsInDef++;
									}
								}
							}

							if(PlayerLargestArmyCount < NPCTroopsInDef)
							{
								PrimaryTarget = PlayerLargetArmyLocation;
								Deffensive = false;
								Aggressive = true;
							}
						}
					}
				}
			}

		}

		if(PlayerCPs.Count > 0 && NeutCPs.Count > 0)
		{
			Aggressive = false;

			if(PlayerLargestArmyCount > MyTroopCount/2)
			{
				foreach(GameObject CP in MyCPs)
				{
					if(CP != null)
					{
						foreach(GameObject destination in CP.GetComponent<ControlPoint>().connectedPoints)
						{
							if(destination.Equals(PlayerLargetArmyLocation))
							{
								PrimaryTarget = CP;
								Deffensive = true;
								if(!Aggressive)
									SecondaryTargets.Add(NeutCPs[0]);
							}
						}
					}
				}
			}
			else
			{
				float distanceToTarget = 0;
				float shortestDistance = 100000;

				Deffensive = false;

				foreach(GameObject CP in MyCPs)
				{
					if(CP != null)
					{
						foreach(GameObject destination in CP.GetComponent<ControlPoint>().connectedPoints)
						{
							if(destination != null && destination.GetComponent<ControlPoint>().isNeutral())
							{
								if(Vector3.Distance(destination.transform.position, CP.transform.position) < shortestDistance)
								{
									shortestDistance = Vector3.Distance(destination.transform.position, CP.transform.position);
									SecondaryTargets.Add(PrimaryTarget);
									PrimaryTarget = destination;
								}
								else
								{
									SecondaryTargets.Add(destination);
								}
							}
						}
					}
				}
			}
		}
	}
}

















