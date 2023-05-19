using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TriggerEvent : MonoBehaviour
{
        public UnityEvent eventOnTrigger;
        public bool destroy;

        private void OnTriggerEnter(Collider other)
        {
        eventOnTrigger.Invoke();
        if ( destroy )   
        {
          Destroy(other.gameObject);
        }

    }

    public void ChangeSpeed(int speedpoints)
    {
        GameManager.instance.objToSliceSpeed = speedpoints;
    }
    public void SpawnNew()
    {
        GameObject ObjToMove = GameManager.instance.objToSliceStartPoint;

        foreach (Transform item in ObjToMove.transform)
        {
           Destroy(item.gameObject);
        }
        GameObject obj =  Instantiate(Resources.Load("CupWithLiquid"), ObjToMove.transform.position,Quaternion.identity, ObjToMove.transform) as GameObject;
        GameManager.instance.objToSlice = obj;
    }
}

