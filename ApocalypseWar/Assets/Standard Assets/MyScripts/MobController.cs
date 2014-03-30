using UnityEngine;
using System.Collections;

public class MobController : MonoBehaviour {
	public Vector3 target;
	private Vector3 targetPoint;/*Kevin*/
	public bool flee;
	
	public float maxVelocity;
	public float maxAcceleration;
	public float satisfactionRadius;
	public float t2t;
	public float turnSpeed;
	
	public Vector3 velocity;

	public bool friendly;
	public int maxHitPoints;
	public int currentHitPoints;
	public int attackPower; // hp damage, attacks are assumed to be 100% accurate
	public float damageVariance; // % of damage variation each attack, [0, 1]
	public float attackDelay; // seconds per attack
	public float attackTimer; // attack cooldown time
	public float attackRange; // attack range
	public float dodgeChance; // % chance to avoid an attack, range [0, 1]
	public float armor; // % damage reduction, range [0, 1]
	public float idleDelay;
	public float idleTimer;
	public float deathDelay;
	public float deathTimer;

	// Use this for initialization
	void Start () {
		velocity = new Vector3 (0, 0, 0);
		targetPoint = this.transform.position;

		currentHitPoints = maxHitPoints = 30;
		attackPower = 15;
		damageVariance = 0.2f;
		attackDelay = 1f;
		attackRange = 2f;
		dodgeChance = 0.1f;
		armor = 0f;

		idleDelay = 0.1f;
		idleTimer = 0f;
		deathDelay = 3f;
		deathTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = friendly ? Color.green : Color.red;
		
		float t = Time.deltaTime;

		if (currentHitPoints <= 0) {
			Die();
		}

		// if the code below is uncommented instead of "Move(t);", only the red team moves+?

		GameObject enemy = FindClosestEnemy ();
		if (enemy != null && Vector3.Distance (transform.position, enemy.transform.position) < attackRange) {
			Attack(enemy, t);
		} else {
			Move(t);
		}
		//Move(t); // comment this out if uncommenting the block above
	}

	private GameObject FindClosestEnemy() {
		GameObject[] mobs = GameObject.FindGameObjectsWithTag ("Mob");
		GameObject enemy = mobs[0]; // to stop the compiler from complaining
		float distance = float.MaxValue;
		if (mobs != null && mobs.Length > 0) {
			foreach (GameObject m in mobs) {
				if (m != null && m.GetComponent<MobController>() != null && friendly != m.GetComponent<MobController>().friendly) {
					float d = (m.transform.position - gameObject.transform.position).magnitude;
					if (d < distance) {
						distance = d;
						enemy = m;
					}
				}
			}
		}
		return enemy;
	}

	private void Attack(GameObject target, float t) {
		MobController enemy = target.GetComponent<MobController>();
		LookAt (target.transform.position, t);
		if (attackTimer <= 0) {
			animation.Play ("attack");
			attackTimer += attackDelay;
			// calculate chance to hit
			if (Random.Range(0f, 1f) > enemy.dodgeChance) {
				// variable damage, minimum 1
				float variance = Random.Range(1f - damageVariance, 1f + damageVariance);
				int damage = Mathf.Min(1, Mathf.RoundToInt(attackPower * variance * (1f - enemy.armor)));
				enemy.currentHitPoints -= damage;
			} else {
				// miss
			}
		}
		attackTimer -= t;
	}

	private void Die() {
		// death animation etc.
		animation.Play ("die");
		if (deathTimer <= deathDelay) {
			deathTimer += Time.deltaTime;
		} else {
			Destroy (this.gameObject);
		}
	}

	private void Move(float t) {

		attackTimer = 0f;

		/* Handle if the anchor gets deleted - Kevin*/
		if(target != null)
		{
			targetPoint = target;
		}
		
		KinematicArrive (targetPoint, t);
		
		if (velocity.magnitude > 0) {
			Align (velocity, t);
		}

		Vector3 position = transform.position;
		
		transform.position = position;

		if (velocity.magnitude == 0) {
			idleTimer += Time.deltaTime;
		} else {
			idleTimer = 0f;
		}
		if (idleTimer >= idleDelay)
			animation.Play ("idle");
		else
			animation.Play ("run");
	}
	
	void KinematicArrive(Vector3 target, float t) {
		Vector3 direction = target - transform.position;
		
		if(direction.magnitude > satisfactionRadius) {
			velocity = maxVelocity * direction.normalized;
		}
		else if(direction.magnitude <= 0.25f) {
			velocity = Vector3.zero;
		} else {
			velocity = Mathf.Max(maxVelocity, direction.magnitude) * direction.normalized;
		}

		transform.position += velocity * t;
		
		//transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void SteeringArrive(Vector3 target, float t) {
		Vector3 direction = target - transform.position;
		Vector3 acceleration = maxAcceleration * direction.normalized;
		
		velocity += acceleration * t;
		
		if (velocity.magnitude > maxVelocity) {
			velocity = Mathf.Min (maxVelocity, direction.magnitude / t2t) * velocity.normalized;
		}
		
		transform.position += velocity * t;
		
		//transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void SteeringFlee(Vector3 target, float t) {
		Vector3 direction = transform.position - target;
		Vector3 acceleration = maxAcceleration * direction.normalized;
		
		velocity = velocity + acceleration * t;
		
		if (velocity.magnitude > maxVelocity)
			velocity = maxVelocity * velocity.normalized;
		
		transform.position += velocity * t;
		
		//transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void KinematicFlee(Vector3 target, float t) {
		Vector3 direction = transform.position - target;
		
		velocity = maxVelocity * direction.normalized;
		
		transform.position += velocity * t;
		
		//transform.position = new Vector3(position.x, 1, position.y);
	}
	
	void Align(Vector3 targetOrientation, float t) {
		Quaternion targetRotation = Quaternion.LookRotation (targetOrientation, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t * turnSpeed);
	}

	void LookAt(Vector3 target, float t) {
		Align((target - transform.position).normalized, Time.deltaTime);
	}
}
