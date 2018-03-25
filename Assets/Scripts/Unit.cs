using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IAttackable {


    private int currentHP;
    private bool playerTwo;
    private Path currentPath;
    private int waypointIndex;
    private Transform currentTarget;
    private bool flagForDeath = false;
    public float aggroRadius;
    public LayerMask enemyMask;
    private bool isAggro;

    public bool isActive = false;
    public float moveSpeed;
    public float attackDistance;
    public int attackDamage;
    public float searchTime;

    public float attackCooldown;
    private float nextAttackTime;
    Transform attackLocation;

    public int maximumHP;

    public List<Unit> enemies;

    public Transform attackSlotParent;

    void Start ()
    {
        waypointIndex = 0;
        currentHP = maximumHP;
        StartCoroutine(SearchForEnemies());
    }

    void Update () {
    
        if (isActive) {
            if (isAggro) {
                StopCoroutine(SearchForEnemies());
                EngageEnemy();
                
            } else {
                if ((currentTarget.position - this.transform.position).magnitude < 0.5)
                    UpdatePathTarget();
                if (!flagForDeath) {
                    Move(currentTarget);
                    //transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                    Debug.DrawLine(transform.position, currentTarget.position,Color.green);
                }
                else Die();
            }
        }
	}

    void OnDrawGizmos() {
        Gizmos.color = Color.red * 0.5f;
        Gizmos.DrawWireSphere(this.transform.position, aggroRadius);
        Gizmos.color = Color.green  * 0.5f;
        Gizmos.DrawWireSphere(this.transform.position, attackDistance);
    }

    private void EngageEnemy()
    {
        Unit enemyUnit = enemies[0];
        
        if (enemyUnit) {
            if (attackLocation == null){
                attackLocation = enemyUnit.ReturnNearestOpenAttackPos(transform.position);
            }
            float distance = (attackLocation.position - transform.position).magnitude;

            if (distance > 0.2f) {
                Move(attackLocation);
                Debug.Log("Moving to " + attackLocation.name);
            } else if (Time.time > nextAttackTime) {
                Attack(enemyUnit);
            }
        } else {
            attackLocation = null;
            isAggro = false;
            StartCoroutine(SearchForEnemies());
        }
    }

    private void Attack(Unit enemyUnit)
    {   
        nextAttackTime = Time.time + attackCooldown;
        enemyUnit.TakeDamage(attackDamage);
    }

    public void SetCurrentPath (Path path){
        if (path != null) {
            currentPath = path;
            UpdatePathTarget();
        }
    }

    private void UpdatePathTarget()
    {
        if (playerTwo) 
            currentTarget = currentPath.GetInverseWaypoint(waypointIndex);
        else
            currentTarget = currentPath.GetWaypoint(waypointIndex);
        if (currentTarget != null) 
            waypointIndex ++;
        else 
            flagForDeath = true;
    }

    private Vector3 Move (Transform target) {
        Vector3 direction = (target.position - this.transform.position).normalized;
        Vector3 moveAmount = direction * moveSpeed * Time.deltaTime;

        this.transform.Translate(moveAmount, Space.World);
        this.transform.rotation = Quaternion.LookRotation(direction);
        return direction.normalized;
    }

    public void SetAllegiance (bool P2) {
        playerTwo = P2;
        enemyMask = new LayerMask();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (P2) {
            renderer.material.color = Color.red;
            enemyMask.value = 1 << LayerMask.NameToLayer("PlayerOne");
        }
        else {
            renderer.material.color = Color.blue;
            enemyMask.value = 1 << LayerMask.NameToLayer("PlayerTwo");
        }
    }

    private IEnumerator SearchForEnemies () 
    {
        enemies.Clear();
        bool foundSomething = false;
        while (!foundSomething) {
            Collider[] sphereHits = Physics.OverlapSphere(transform.position, aggroRadius, enemyMask);
            Debug.Log(sphereHits.Length);
            for (int i = 0; i < sphereHits.Length; i++)
            {
                Unit hitUnit = sphereHits[i].gameObject.GetComponent<Unit>();
                if (hitUnit) {
                    if (!playerTwo) {
                        if (hitUnit.playerTwo){
                            
                            foundSomething = true;
                            enemies.Add(hitUnit);
                        }
                    } else {
                        if (!hitUnit.playerTwo){
                            enemies.Add(hitUnit);
                            foundSomething = true;
                        }
                    }  
                }
            }
            yield return new WaitForSeconds(searchTime);
        }
        isAggro = foundSomething;
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

    public Transform ReturnNearestOpenAttackPos (Vector3 position)
    {
        float distance = Mathf.Infinity;
        Transform closestPosition = null;
        int runningIndex = 0;
        Debug.Log("Status before:");
        for (int i = 0; i < attackSlotParent.childCount; i++)
        {
            Debug.Log(attackSlotParent.GetChild(i).name + attackSlotParent.GetChild(i).GetComponent<AttackSlot>().isOpen);
        }

        //For each child of the attack slot holder
        for (int i = 0; i < attackSlotParent.childCount; i++)
        {
            Transform child = attackSlotParent.GetChild(i);
            //If the slot is open
            if (child.GetComponent<AttackSlot>().isOpen){
                float currentDistance = (child.position - position).magnitude;
                //And closer than the existing current distance
                if (currentDistance < distance) {
                    distance = currentDistance;
                    runningIndex = i;
                }
            }
        }
        
        closestPosition = attackSlotParent.GetChild(runningIndex);
        Debug.Log("Closest position is " + closestPosition.name);
        Debug.Log("Setting " + closestPosition.name + " bool to false");
        closestPosition.GetComponent<AttackSlot>().isOpen = false;
        Debug.Log("Status after:");
        for (int i = 0; i < attackSlotParent.childCount; i++)
        {
            Debug.Log(attackSlotParent.GetChild(i).name + attackSlotParent.GetChild(i).GetComponent<AttackSlot>().isOpen);
        }
        
        return closestPosition;
    }

    // private void SetAttackPositions() {
    //     float angle = 360f / (transform.childCount + 1);

    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         Vector3 currentPosition = this.transform.position + new Vector3(Mathf.Cos(i * angle) * 0.75f * attackDistance, 0, Mathf.Sin(i * angle) * 0.75f * attackDistance);
    //         transform.GetChild(i).position = currentPosition;
    //     }
    // }
}
