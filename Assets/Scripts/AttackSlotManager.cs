using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlotManager : MonoBehaviour {

    public Transform attackSlotHolder; 
    
    private List<Transform> possibleSlots;
    private float slotRadius;

    void Start ()
    {
        possibleSlots = new List<Transform>();
        slotRadius = transform.root.GetComponentInChildren<Unit>().attackDistance;
        AcquireAttackSlots();
    }

    void Update ()
    {
        ArrangeAttackSlots();
    }

    private void ArrangeAttackSlots()
    {
        int n = attackSlotHolder.childCount;
        float angDeg = 360f/n;
        for (int i = 0; i < n; i++)
        {
            Transform child = attackSlotHolder.GetChild(i);
            child.position = transform.position + Quaternion.AngleAxis(i * angDeg, transform.up) * transform.right * slotRadius;
        }
    }

    private void AcquireAttackSlots()
    {
        for (int i = 0; i < attackSlotHolder.childCount; i++)
        {
            Transform child = attackSlotHolder.GetChild(i);
            AttackSlot slot = child.GetComponent<AttackSlot>();
            if (slot != null)
            {
                slot.isOpen = true;
                possibleSlots.Add(child);
            }
        }
    }

     public Transform ReturnNearestOpenAttackSlot (Transform unitTransform)
    {
        float distance = Mathf.Infinity;
        int runningIndex = -1;

        for (int i = 0; i < possibleSlots.Count; i++)
        {
            AttackSlot slot = possibleSlots[i].GetComponent<AttackSlot>();

            if(slot.isOpen) {
                float currentDistance = (possibleSlots[i].position - unitTransform.position).magnitude;

                if (currentDistance < distance) {
                    distance = currentDistance;
                    runningIndex = i;
                }
            }
        }
        
        if(runningIndex < 0)
        {
            Debug.Log("No open spots found!");
            return unitTransform;
        } else {
            possibleSlots[runningIndex].GetComponent<AttackSlot>().isOpen = false;
            return possibleSlots[runningIndex];
        }

        
    }
}
