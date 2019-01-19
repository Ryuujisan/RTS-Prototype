using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
	public enum Task
	{
		idle, move, follow, chase, attack
	}
	
	const string ANIMATOR_SPEED = "Speed";
    const string ANIMATOR_ALIVE = "Alive";
    const string ANIMATOR_Attack = "Shoot";
	
	public static List<ISelectable> selectablesUnit = new List<ISelectable>();
	
	protected Transform target;
	
	[Header("Unit")]
	[SerializeField]
	private float hp;

	[SerializeField]
	private float hpMax;

	[SerializeField]
	private GameObject hpBarPrefab;

	[SerializeField]
	protected float stopingDistance;

	[SerializeField]
	protected float attackDistance;

	[FormerlySerializedAs("attackSpeed")]
	[SerializeField]
	protected float attackColdown;

	[SerializeField]
	protected float attackDamage;
	
	protected NavMeshAgent nav;
	protected Animator animator;
	private float attackTimer;
	protected Task task = Task.idle;
		
	
	protected GUIHealtBar hpBar;
	
	public float HealthPercent
	{
		get { return hp / hpMax; }
	}

	public static List<ISelectable> SeletableUnits
	{
		get { return selectablesUnit; }
	}

	public bool IsAlive
	{
		get { return hp > 0; }
	}
	
	protected virtual void Awake()
	{
		nav = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();

		hp = hpMax;
		hpBar = Instantiate(hpBarPrefab, transform).GetComponent<GUIHealtBar>();
	}

	private void Start()
	{
		if(this is ISelectable)
		{
			selectablesUnit.Add(this as ISelectable);
			(this as ISelectable).SetSeleted(false);
		}
	}

	private void OnDestroy()
	{
		if(this is ISelectable)
		{
			selectablesUnit.Remove(this as ISelectable);
		}
	}

	void Update () 
	{
		if(IsAlive)
		{
			switch(task)
			{
				case Task.idle:
					Idling();
					break;
				case Task.move:
					Moving();
					break;
				case Task.follow:
					Following();
					break;
				case Task.chase:
					Chasing();
					break;
				case Task.attack:
					Attacking();
					break;
			}
		}

		Animate();
	}

	protected virtual void Idling()
	{
		nav.velocity = Vector3.zero;
	}

	protected virtual void Moving()
	{
		float distance = Vector3.Magnitude(nav.destination - transform.position);
		if(distance <= stopingDistance)
		{
			task = Task.idle;
		}
	}

	protected virtual void Following()
	{
		if(target != null)
		{
			nav.SetDestination(target.position);
		}
		else
		{
			task = Task.idle;
		}
	}

	protected virtual void Chasing()
	{
		if(target != null)
		{
			float distance = Vector3.Magnitude(target.position - transform.position);
			if(distance <= attackDistance)
			{
				task = Task.attack;				
			}
		}
		else
		{
			task = Task.idle;
		}
	}

	protected virtual void Attacking()
	{
		nav.velocity = Vector3.zero;
		
		if(target != null)
		{
			nav.velocity = Vector3.zero;
			transform.LookAt(target);
			
			float distance = Vector3.Magnitude(target.position - transform.position);
			if(distance < attackDistance)
			{
				if((attackTimer -= Time.deltaTime) <= 0)
				{
					Attack();
				}
			}
			else
			{
				task = Task.chase;
			}
		}
		else
		{
			task = Task.idle;
		}
	}
	protected virtual void Animate()
	{
		var speedVector = nav.velocity;
		speedVector.y = 0;
		float speed = speedVector.magnitude;
		
		animator.SetFloat(ANIMATOR_SPEED, speed);
		animator.SetBool(ANIMATOR_ALIVE, IsAlive);
	}

	public virtual void Attack()
	{
		animator.SetTrigger(ANIMATOR_Attack);
		attackTimer = attackColdown;
	}

	public virtual void DealDamage()
	{
		if(target)
		{
			Unit unit = target.GetComponent<Unit>();
			if(unit && unit.IsAlive)
			{
				unit.ReciveDamage(attackDamage);
			}
			else
			{
				target = null;
			}
		}
	}

	public virtual void ReciveDamage(float damage)
	{
		if(IsAlive)
			hp -= damage;
	}
	
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackDistance);
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		
	}
}
