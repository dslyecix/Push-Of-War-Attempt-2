using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

    private List<Transform> waypoints;

	void Start ()
    {
        SetWaypoints();
        Debug.Log(waypoints.ToString());
    }


    void Update () {
		for (int i = 0; i < waypoints.Count-1; i++)
        {
            Debug.DrawLine(waypoints[i].position, waypoints[i+1].position,Color.red);
        }
	}

    private void SetWaypoints()
    {
        waypoints = new List<Transform>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            if (child.GetComponent(typeof(Waypoint)) != null)
                waypoints.Add(child);
        }
    }

    public Transform GetWaypoint (int index) {
        if (index < waypoints.Count)
            return waypoints[index];
        else {
            Debug.Log("Error, no waypoints in list!");
            return null;
        }
    }

}
