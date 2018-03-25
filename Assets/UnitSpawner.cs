using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour {

    public List<GameObject> unitList;
    public GameObject selectedUnit;
    public Path selectedPath;

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
        Unit unit = unitGO.GetComponent<Unit>();
        unit.SetCurrentPath(selectedPath);
        unit.isActive = true;

    }
}
