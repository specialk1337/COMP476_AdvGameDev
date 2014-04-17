using UnityEngine;
using System.Collections;

public class FireballScript : MonoBehaviour {

	public GameObject textPrefab;

	private GameObject caster;

	public Vector3 target;
	public bool friendly;
	public float radius;
	public float triggerDistance;
	public float projectionPower;
	public float maxSpeed;
	public float deathDelay;
	private float deathCounter;
	private bool dying;

	// Use this for initialization
	void Start () {
		dying = false;
	}
	
	// Update is called once per frame
	void Update () {
		target.y = transform.position.y;
		if ((target - transform.position).magnitude <= triggerDistance) {
			if (dying) {
				deathCounter += Time.deltaTime;
				if (deathCounter >= deathDelay) {
					Destroy(gameObject);
				}
			} else {
				Explode();
				ParticleEmitter[] emitters = gameObject.GetComponentsInChildren<ParticleEmitter>();
				foreach (ParticleEmitter emitter in emitters) {
					emitter.emit = false;
				}
				dying = true;
			}
		} else {
			Move();
		}
	}

	public void Init(GameObject caster, Vector3 target, bool friendly) {
		this.caster = caster;
		this.target = target;
		this.friendly = friendly;
	}

	public void Move() {
		Vector3 velocity = target - transform.position;
		velocity.Normalize();
		transform.position += velocity * maxSpeed * Time.deltaTime;
	}

	private void Explode() {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
		if(hitColliders.Length>0)
		{
			foreach(Collider inRangeUnit in hitColliders)
			{
				if(inRangeUnit.transform.tag == "Mob")
				{
					MobController enemy = inRangeUnit.gameObject.GetComponent<MobController>();
					if(enemy != null && friendly != enemy.friendly) 
					{
						if (Random.Range(0f, 1f) > enemy.dodgeChance) {
							enemy.Project(((enemy.transform.position + Vector3.up * 10f) - transform.position).normalized * projectionPower);
							enemy.TakeDamage(caster.GetComponent<MobController>().CalcDamage(enemy.armor), true);
						} else {
							enemy.Dodge();
						}
					}
				}
			}
		}
	}
}
