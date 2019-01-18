using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
	const string ANIMATOR_SPEED = "Speed";
	private const string ANIMATOR_ALIVE = "Alive";
	
	public static List<ISelectable> selectablesUnit = new List<ISelectable>();
	
	public Transform target;

	[SerializeField]
	private float hp;

	[SerializeField]
	private float hpMax;

	[SerializeField]
	private GameObject hpBarPrefab;
	
	
	private NavMeshAgent nav;
	private Animator animator;

	protected GUIHealtBar hpBar;
	
	public float HealthPercent
	{
		get { return hp / hpMax; }
	}

	public static List<ISelectable> SeletableUnits
	{
		get { return selectablesUnit; }
	}
	
	private void Awake()
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
		if(target != null)
		{
			nav.SetDestination(target.position);
		}
		
		Animate();
	}

	protected virtual void Animate()
	{
		var speedVector = nav.velocity;
		speedVector.y = 0;
		float speed = speedVector.magnitude;
		
		animator.SetFloat(ANIMATOR_SPEED, speed);
		animator.SetBool(ANIMATOR_ALIVE, hp > 0);
	}
}
