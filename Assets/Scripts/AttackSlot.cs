using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlot : MonoBehaviour {

    public bool isOpen;
    
    void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, name, false);
    }
}
