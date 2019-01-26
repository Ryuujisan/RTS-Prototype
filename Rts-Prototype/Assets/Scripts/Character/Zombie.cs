using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Zombie : Unit
{
    [Header("Zombie")]
    [SerializeField]
    private float cheasingSpeed = 5f;

    [SerializeField]
    private float patrolRadios;

    [FormerlySerializedAs("roamingColdown")]
    [SerializeField]
    private float idleColdown = 2;
    
    private float normalSpeed;
    private Vector3 startPoint;
    
    private List<Soldier> seenSoldier = new List<Soldier>();
    private float idleTimer;
    
    private Soldier ClosetSoldier
    {
        get
        {
            if(seenSoldier == null && seenSoldier.Count <= 0)
            {
                return null;
            }

            var closeSoldier = seenSoldier
                .Where(s => s.IsAlive)
                .Select(n => new {n, distance = Vector3.Distance(transform.position, n.transform.position)})
                .OrderBy(o => o.distance)
                .FirstOrDefault();

            return closeSoldier == null ? null : closeSoldier.n;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        normalSpeed = nav.speed;
        startPoint = transform.position;
    }

    protected  override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        var soldier = other.gameObject.GetComponent<Soldier>();
        if(soldier != null && !seenSoldier.Contains(soldier))
        {
            seenSoldier.Add(soldier);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        var soldier = other.gameObject.GetComponent<Soldier>();
        if(soldier != null)
        {
            seenSoldier.Remove(soldier);
        }
    }

    protected override void Moving()
    {
        base.Moving();
        nav.speed = normalSpeed;
        UpdateSight();
    }

    protected override void Idling()
    {
        base.Idling();
        UpdateSight();
        
        if(target != null)
        {
            if((idleTimer -= Time.deltaTime) <= 0)
            {
                idleTimer = idleColdown;
                task      = Task.move;
                SetRandomPosition();
            }
        }
    }

    protected override void Chasing()
    {
        base.Chasing();
        nav.speed = cheasingSpeed;
    }

    private void UpdateSight()
    {
        var soldier = ClosetSoldier;
        if(soldier)
        {
            target = soldier.transform;
            task = Task.chase;
            nav.SetDestination(target.position);
        }
    }

    private void SetRandomPosition()
    {
        Vector3 delta = new Vector3(Random.Range(-1, 1), 0 , Random.Range(-1, 1));
        delta.Normalize();
        delta *= patrolRadios;
        
        
        nav.SetDestination(startPoint + delta);
    }

    public override void ReciveDamage(float damage, Vector3 delerPosistion)
    {
        base.ReciveDamage(damage, delerPosistion);
        if(!target)
        {
            task = Task.move;
            nav.SetDestination(delerPosistion);
        }
        if(HealthPercent > .5)
        {
            //animator.SetTrigger("");
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.blue;
        if(!Application.isPlaying)
            startPoint = transform.position;
        Gizmos.DrawWireSphere(startPoint, patrolRadios);
    }
}
