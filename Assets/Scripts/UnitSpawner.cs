using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour {

    public bool playerTwo;
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
        if (playerTwo) {
            unitGO.name = "Player Two unit " + (spawnedUnitCounter + 1);
            unitGO.layer = LayerMask.NameToLayer("PlayerTwo");
        } else {
            unitGO.name = "Player One unit " + (spawnedUnitCounter + 1);
            unitGO.layer = LayerMask.NameToLayer("PlayerOne");
        }
        spawnedUnitCounter ++;
        Unit unit = unitGO.GetComponent<Unit>();
        unit.SetAllegiance(playerTwo);
        unit.SetCurrentPath(selectedPath);

        unit.isActive = true;

    }
}
