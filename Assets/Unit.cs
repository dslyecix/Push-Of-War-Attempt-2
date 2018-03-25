using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    private Path currentPath;
    private int waypointIndex;
    private Transform currentTarget;

    public bool isActive = false;
    private bool flagForDeath = false;
    public float moveSpeed;

	void Start ()
    {
        waypointIndex = 0;
    }


    void Update () {
        if (isActive) {
		    if ((currentTarget.position - this.transform.position).magnitude < 0.5)
                UpdateTarget();
            if (!flagForDeath)
                Move(currentTarget);
            else DestroySelf();
        }
	}
    
    public void SetCurrentPath (Path path){
        if (path != null) {
            currentPath = path;
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        currentTarget = currentPath.GetWaypoint(waypointIndex);
        if (currentTarget != null)
            waypointIndex ++;
        else
        {
            flagForDeath = true;
        }
    }

    private void DestroySelf()
    {
        Debug.Log("No target left in list! Destroying self.");
        DestroyImmediate(this.gameObject);
    }

    private void Move (Transform target) {
        Vector3 direction = (target.position - this.transform.position).normalized;
        Vector3 moveAmount = direction * moveSpeed * Time.deltaTime;

        this.transform.Translate(moveAmount);
    }
}
