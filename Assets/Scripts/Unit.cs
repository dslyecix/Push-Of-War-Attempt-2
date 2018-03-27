using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    public UnitState currentState;
    public enum UnitState
    {
        Inactive, OnPath, InCombat
    }   

    public Team currentTeam;
    public enum Team {
        NoTeam, TeamOne, TeamTwo
    }

    [Header("Health stuff")]
    public int maximumHP;
    private int currentHP;
    
    [Header("Combat stuff")]
    public float aggroRadius;
    public int attackDamage;
    public float attackDistance;
    public float attackCooldown;
    private float nextAttackTime;
    private bool lookingForEnemies;
    private LayerMask enemyLayer;
    public float searchTime;
    Transform attackLocation;

    [Header("Pathfinding stuff")]
    private Path currentPath;
    private int waypointIndex;
    private Transform currentTarget;
    private bool pathCompleted = false;


    public List<Unit> enemies;
    private AttackSlotManager attackSlotManager;
    private NavMeshAgent navAgent;

    bool drawAggro = false;
    bool drawAttack = false;

    Animator animator;
    //public Animation attackAnimation;

    void Awake () 
    {
        currentState = UnitState.Inactive;
        attackSlotManager = GetComponent<AttackSlotManager>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start ()
    {
        Initialize();   
    }

    private void Initialize()
    {
        waypointIndex = 0;
        currentHP = maximumHP;
    }

    void Update () {
        switch (currentState)
        {
            case UnitState.Inactive:
                Debug.Log(name + " is inactive!");
                break;

            case UnitState.OnPath:
                drawAggro = true;
                drawAttack = false;
                // If we aren't already scanning for enemies, begin
                ActivateEnemySearch();

                // Check if new waypoint is required from path
                EvaluateWaypointDistance();

                if (pathCompleted)
                {
                    Die(); //TODO: Don't just die when you run out of path
                    break;
                }
                
                // Nothing else to do, move along path.
                navAgent.destination = currentTarget.position;
                break;

            case UnitState.InCombat:
                drawAggro = false;
                drawAttack = true;
                // An enemy was detected.  Determine what to do.
                EngageEnemy();
                break;

            default:
                Debug.Log("Unit is not in a valid state!");
                break;
        }
	}

    private void EvaluateWaypointDistance()
    {
        if ((currentTarget.position - this.transform.position).magnitude < 0.5)
            UpdatePathTarget();
    }

    private void ActivateEnemySearch()
    {
        if (!lookingForEnemies)
        {
            lookingForEnemies = true;
            Debug.Log("Starting enemy search!");
            StartCoroutine(SearchForEnemies());
        }
    }


    private void EngageEnemy()
    {
        Unit enemyUnit = enemies[0];
        
        if (enemyUnit) {
            
            float distance = (enemyUnit.transform.position - transform.position).magnitude;


            if (distance >= attackDistance) {
                navAgent.enabled = true;
                if (attackLocation == null){
                    attackLocation = enemyUnit.attackSlotManager.ReturnNearestOpenAttackSlot(transform);
                }
                navAgent.destination = attackLocation.position;
            } else {
                navAgent.enabled = false;
                Attack(enemyUnit);
            }
        } else {
            attackLocation = null;
            navAgent.enabled = true;
            currentState = UnitState.OnPath;
        }
    }

    private void Attack(Unit enemyUnit)
    {   
        if (Time.time > nextAttackTime) {
            animator.SetTrigger("AttackTrigger");
            nextAttackTime = Time.time + attackCooldown;
            enemyUnit.TakeDamage(attackDamage);
        }
    }

    public void SetCurrentPath (Path path){
        if (path != null) {
            currentPath = path;
            UpdatePathTarget();
        }
    }

    private void UpdatePathTarget()
    {
        if (currentTeam == Team.TeamTwo) 
            currentTarget = currentPath.GetInverseWaypoint(waypointIndex);
        else
            currentTarget = currentPath.GetWaypoint(waypointIndex);
        if (currentTarget != null) 
            waypointIndex ++;
        else 
            pathCompleted = true;
    }

    // OLD MOVE METHOD, PRIOR TO NAVMESHAGENT IMPLEMENTATION
    // private void Move (Transform target) {
    //     Vector3 direction = (target.position - this.transform.position).normalized;
    //     Vector3 moveAmount = direction * moveSpeed * Time.deltaTime;

    //     this.transform.Translate(moveAmount, Space.World);
    //     this.transform.rotation = Quaternion.LookRotation(direction);
    // }

    public void SetTeam (Team team) {
        currentTeam = team;
        enemyLayer = new LayerMask();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (currentTeam == Team.TeamOne) {
            renderer.material.color = Color.red;
            enemyLayer.value = 1 << LayerMask.NameToLayer("PlayerTwo");
            //this.gameObject.layer = LayerMask.NameToLayer(Team.TeamTwo.ToString());
        }
        else {
            renderer.material.color = Color.blue;
            enemyLayer.value = 1 << LayerMask.NameToLayer("PlayerOne");
            //this.gameObject.layer = LayerMask.NameToLayer(Team.TeamOne.ToString());

        }
    }

    private IEnumerator SearchForEnemies () 
    {
        Debug.Log("Starting Search Coroutine");
        bool foundSomething = false;
        while (!foundSomething) {
            Debug.Log(name + "'s coroutine while loop");
            enemies.Clear();
            Collider[] sphereHits = Physics.OverlapSphere(transform.position, aggroRadius, enemyLayer);

            for (int i = 0; i < sphereHits.Length; i++)
            {
                Unit hitUnit = sphereHits[i].gameObject.GetComponent<Unit>();
                if (hitUnit) {
                    if (hitUnit.currentTeam != currentTeam) {
                        foundSomething = true;
                        enemies.Add(hitUnit);
                    } 
                }
            }
            yield return new WaitForSeconds(searchTime);
        }

        lookingForEnemies = false;
        currentState = UnitState.InCombat;
        yield break;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0) {
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(name + " has died!");
        StopAllCoroutines();
        DestroyImmediate(this.gameObject);
    }   
    
    void OnDrawGizmos() {
        if (drawAggro) {
            Gizmos.color = Color.red * 0.5f;
            Gizmos.DrawWireSphere(this.transform.position, aggroRadius);
        }
        if (drawAttack) {
            Gizmos.color = Color.green  * 0.5f;
            Gizmos.DrawWireSphere(this.transform.position, attackDistance);
        }
    }

}
