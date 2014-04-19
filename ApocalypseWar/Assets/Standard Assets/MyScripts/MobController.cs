using UnityEngine;
using System.Collections;

public class MobController : MonoBehaviour {

	public GameObject textPrefab;
	public GameObject fireballPrefab;

	public Vector3 target; // actual target, change this when changing target
	private Vector3 targetPoint;/*Kevin*/ // modified by flocking, don't change this
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
	private float attackTimer; // attack cooldown time
	public float preAttackMaxDelay; // max delay before attack hits
	private float preAttackDelay; // actual delay (randomized)
	private float preAttackTimer; // pre-attack cooldown time
	public float attackRange; // attack range
	public float aggroRange; // range to react to an enemy
	public float dodgeChance; // % chance to avoid an attack, range [0, 1]
	public float armor; // % damage reduction, range [0, 1]
	public float idleDelay;
	private float idleTimer;
	public float deathDelay;
	private float deathTimer;

	public float separationDistance;
	public float separationWeight;
	public float alignmentWeight;
	public float cohesionWeight;
	public float seekWeight;
	public float flockDistance;

	public GameObject closestEnemy;
	public Vector3 separation;
	public Vector3 alignment;
	public Vector3 cohesion;

	public Vector3 projectionVelocity;
	public Vector3 projectionGravity;

	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
		projectionVelocity = Vector3.zero;
		target = transform.position;
		preAttackDelay = Random.Range(0f, preAttackMaxDelay);
	}

	public void Init(bool friendly) {
		this.friendly = friendly;
		MeshRenderer[] meshRenderers = gameObject.transform.FindChild("Bip01").gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mr in meshRenderers) {
			mr.material.color = friendly ? Color.green : Color.red;
		}
	}
	
	// Update is called once per frame
	void Update () {

		float t = Time.deltaTime;

		UpdateProjection();

		if (currentHitPoints <= 0) {
			Die();
		} else {
			
			closestEnemy = FindClosestEnemy ();

			// decision tree, seekWeight added after demo so mobs can flock while attacking
			if (closestEnemy != null) {
				float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);
				if (distance < attackRange) {
					Attack(closestEnemy, t);
					Flock(0f);
				} else {
					if (distance < aggroRange) {
						target = closestEnemy.transform.position;
					}
					Flock(seekWeight);
				}
				Move(t, distance >= attackRange);
			} else {
				animation.Play("dance");
			}
		}
	}

	// added seek weight after demo so mobs can flock while attacking
	private void Flock(float seekWeight) {
		separation = Vector3.zero;
		alignment = Vector3.zero;
		cohesion = Vector3.zero;
		Vector3 v = Vector3.zero;

		GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
		int friendlyCount = 0;

		foreach (GameObject mob in mobs) {
			if (!gameObject.Equals(mob.gameObject) && mob.GetComponent<MobController>() != null) {
				v = (transform.position - mob.transform.position);
				v.y = 0f;
				float mag = v.magnitude;
				if (mag < flockDistance) {
					if (mag < separationDistance) {
						// separation
						v = v.normalized * separationDistance / mag;
						separation += v;
					}
					
					// friendly mobs only
					if (friendly == mob.GetComponent<MobController>().friendly) {
						// alignment
						alignment += mob.transform.forward;
						
						// cohesion
						cohesion += mob.transform.position;
						
						++friendlyCount;
					}
				}
			}
		}
		
		separation = Vector3.ClampMagnitude(separation, maxVelocity);

		if (alignment != Vector3.zero) {
			alignment.Normalize();
		}
		
		if (friendlyCount > 0) {
			cohesion /= friendlyCount;
			cohesion -= transform.position;
			cohesion = Vector3.ClampMagnitude(cohesion, maxVelocity);
		}

		velocity = Vector3.ClampMagnitude(target - transform.position, maxVelocity) * seekWeight;
		velocity += separation * separationWeight + alignment * alignmentWeight + cohesion * cohesionWeight;
		velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
		targetPoint = transform.position + velocity;

	}

	private GameObject FindClosestEnemy() {
		GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
		GameObject enemy = closestEnemy;
		float distance = float.MaxValue;
		if (mobs.Length > 0) {
			foreach (GameObject m in mobs) {
				if (m != null && m.GetComponent<MobController>() != null && friendly != m.GetComponent<MobController>().friendly &&
				    m.GetComponent<MobController>().currentHitPoints > 0) {
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

	// added random pre-attack delay after demo so mobs don't all attack at once 
	private void Attack(GameObject target, float t) {
		MobController enemy = target.GetComponent<MobController>();
		LookAt (target.transform.position, t);
		if (attackTimer <= 0) {
			preAttackDelay = Random.Range (0f, preAttackMaxDelay);
			if (preAttackTimer <= 0) {
				animation.Play ("attack");
				attackTimer += attackDelay;
				preAttackTimer += preAttackDelay;
				if (gameObject.name.Equals("skeletonMage(Clone)")) {
					GameObject fireball = (GameObject)Instantiate (fireballPrefab, transform.position + (transform.forward + transform.up) / 2f, Quaternion.identity);
					fireball.GetComponent<FireballScript>().Init(gameObject, enemy.transform.position, friendly);
				} else {
					if (Random.Range(0f, 1f) > enemy.dodgeChance) {
						enemy.TakeDamage(CalcDamage(enemy.armor), false);
					} else {
						enemy.Dodge();
					}
				}
			}
			preAttackTimer -= t;
		}
		attackTimer -= t;
	}

	public int CalcDamage(float enemyArmor) {
		// variable damage, minimum 1
		float variance = Random.Range(1f - damageVariance, 1f + damageVariance);
		return Mathf.Max(1, Mathf.RoundToInt(attackPower * variance * (1f - enemyArmor)));
	}

	public void Dodge() {
		GameObject damageDisplay = (GameObject)Instantiate (textPrefab, transform.position, Quaternion.identity);
		damageDisplay.GetComponent<TextScript>().Init("miss", Color.white);
	}

	public void TakeDamage(int amount, bool fire) {
		currentHitPoints -= amount;
		GameObject damageDisplay = (GameObject)Instantiate (textPrefab, transform.position, Quaternion.identity);
		damageDisplay.GetComponent<TextScript>().Init(amount.ToString(), fire ? Color.Lerp(Color.red, Color.yellow, 0.5f) : Color.yellow);
	}

	public void HealDamage(int amount) {
		amount = Mathf.Min(amount, maxHitPoints - currentHitPoints);
		if (amount > 0) {
			currentHitPoints += amount;
			GameObject damageDisplay = (GameObject)Instantiate (textPrefab, transform.position, Quaternion.identity);
			damageDisplay.GetComponent<TextScript>().Init(amount.ToString(), Color.blue);
		}
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

	public void Project(Vector3 force) {
		projectionVelocity = force;
	}

	private void UpdateProjection() {
		if (projectionVelocity != Vector3.zero) {
			projectionVelocity += projectionGravity * Time.deltaTime;
			if (transform.position.y > 0) {
				transform.position += projectionVelocity * Time.deltaTime;
			} else {
				transform.position.Scale(new Vector3(1f, 0f, 1f));
				projectionVelocity = Vector3.zero;
			}
		}
	}

	private void Move(float t, bool animate) {

		attackTimer = 0f;

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
		if (animate){
			if (idleTimer >= idleDelay)
				animation.Play ("waitingforbattle");
			else
				animation.Play ("run");
		}
	}
	
	void KinematicArrive(Vector3 target, float t) {
		Vector3 direction = target - transform.position;
		
		if(direction.magnitude > satisfactionRadius) {
			velocity = maxVelocity * direction.normalized;
		}
		else if(direction.magnitude <= 1f) {
			velocity = Vector3.zero;
		} else {
			velocity = Mathf.Min(maxVelocity, direction.magnitude) * direction.normalized;
		}

		transform.position += velocity * t;
	}
	
	void SteeringArrive(Vector3 target, float t) {
		Vector3 direction = target - transform.position;
		Vector3 acceleration = maxAcceleration * direction.normalized;
		
		velocity += acceleration * t;
		
		if (velocity.magnitude > maxVelocity) {
			velocity = Mathf.Min (maxVelocity, direction.magnitude / t2t) * velocity.normalized;
		}
		
		transform.position += velocity * t;
	}
	
	void SteeringFlee(Vector3 target, float t) {
		Vector3 direction = transform.position - target;
		Vector3 acceleration = maxAcceleration * direction.normalized;
		
		velocity = velocity + acceleration * t;
		
		if (velocity.magnitude > maxVelocity)
			velocity = maxVelocity * velocity.normalized;
		
		transform.position += velocity * t;
	}
	
	void KinematicFlee(Vector3 target, float t) {
		Vector3 direction = transform.position - target;
		
		velocity = maxVelocity * direction.normalized;
		
		transform.position += velocity * t;
	}
	
	void Align(Vector3 targetOrientation, float t) {
		Quaternion targetRotation = Quaternion.LookRotation (targetOrientation, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t * turnSpeed);
	}

	void LookAt(Vector3 target, float t) {
		Align((target - transform.position).normalized, t);
	}
}
