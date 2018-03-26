using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour {

    public Unit.Team currentTeam;
    public List<GameObject> unitList;
    public GameObject selectedUnit;
    public Path selectedPath;

    private int spawnedUnitCounter = 0;

	void Start () {
		selectedUnit = unitList[0];
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)){
            SpawnUnit();
        }
	}

    private void SpawnUnit()
    {
        GameObject unitGO = Instantiate(selectedUnit,this.transform.position, Quaternion.identity);
        if (currentTeam == Unit.Team.TeamTwo) {
            unitGO.name = "P2 unit " + (spawnedUnitCounter + 1);
            unitGO.layer = LayerMask.NameToLayer("PlayerTwo");
        } else {
            unitGO.name = "P1 unit " + (spawnedUnitCounter + 1);
            unitGO.layer = LayerMask.NameToLayer("PlayerOne");
        }
        spawnedUnitCounter ++;
        Unit unit = unitGO.GetComponent<Unit>();
        unit.SetTeam(currentTeam);
        unit.SetCurrentPath(selectedPath);

        unit.currentState = Unit.UnitState.OnPath;
    }
}
