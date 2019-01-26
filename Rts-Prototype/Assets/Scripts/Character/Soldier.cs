using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Soldier : Unit, ISelectable
{
	[Header("Soldier")]
	[Range(0, 3)]
	[SerializeField]
	private float shootDuration;
	
	[SerializeField]
	private ParticleSystem muzzleEffect;

	[SerializeField]
	private ParticleSystem ImpactEffect;

	[SerializeField]
	private LayerMask shootingLayerMask;

	private LineRenderer lineEffect;
	private Light lightEffect;

	protected override void Awake()
	{
		base.Awake();
		lineEffect = muzzleEffect.GetComponent<LineRenderer>();
		lightEffect = muzzleEffect.GetComponent<Light>();
		ImpactEffect.transform.SetParent(null);
		EndShootEffect();
	}

	public void SetSeleted(bool seleted)
	{
		hpBar.gameObject.SetActive(seleted);
	}

	private void Command(Vector3 destination)
	{
		nav.SetDestination(destination);
		task = Task.move;
		target = null;
	}
	
	private void Command(Soldier soldierToFollow)
	{
		target = soldierToFollow.transform;
		task = Task.follow;
	}
	
	private void Command(Zombie dragonToKill)
	{
		target = dragonToKill.transform;
		nav.SetDestination(target.position);
		task = Task.chase;
	}

	public override void DealDamage()
	{
		if(Shoot())
		base.DealDamage();	
	}

	private bool Shoot()
	{
		Vector3 start = muzzleEffect.transform.position;
		Vector3 direction = transform.forward;
		
		
		
		RaycastHit hit;
		if(Physics.Raycast(start, direction, out hit, attackDistance, shootingLayerMask))
		{
			StartShootEffect(start, hit.point, true);
			var unit = hit.collider.gameObject.GetComponent<Unit>();
			return unit;
		}
		StartShootEffect(start, start + direction * attackDistance, false);
		return false;
	}

	private void EndShootEffect()
	{	
		lightEffect.enabled = false;
		lineEffect.enabled  = false;
	}

	private void StartShootEffect(Vector3 lineStart, Vector3 lineEnd, bool hitSomthing)
	{
		if(hitSomthing)
		{
			ImpactEffect.transform.position = lineEnd;		
			ImpactEffect.Play();
		}
		
		lineEffect.SetPositions(new Vector3[]{lineStart, lineEnd});
		lightEffect.enabled = true;
		lineEffect.enabled = true;
		muzzleEffect.Play();
		Invoke("EndShootEffect", shootDuration);
	}
}
