using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;



    public class GameManager : MonoBehaviour
    {
        public GameObject objToSlice;
        public GameObject objToSliceStartPoint;
        public float objToSliceSpeed = 5;



        
        public GameObject knife;
        [SerializeField]
        float knifeSpeed = 5;
        
        public Transform knifeFinalPos;
        [SerializeField]
        Vector3 knifeStartPos;

        public bool inProcess;
        public bool inFinishPos;
        Vector3 objFinalPos;

        public static GameManager instance;

        public GameObject slicePart;
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            objFinalPos = objToSlice.transform.position + new Vector3(0, 0, -15);
            knifeStartPos = knife.transform.position;
        }

        // Update is called once per frame
        void Update()
        {

            if (knife.transform.position == knifeFinalPos.position)
            {
                inFinishPos = true;
            }
            if (knife.transform.position == knifeStartPos)
            {
                if (inFinishPos == true)
                {
                    inProcess = false;
                    inFinishPos = false;
                }
            }
            if (inProcess == false)
            {
                objMove(objFinalPos);
            }
       
#if PLATFORM_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                inProcess = true;
                knifeMove(knifeFinalPos.position);
                //EventsManager.SliceStartEvent.Invoke();
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(knife.transform.DOMove(knifeStartPos, 0.5f));

                //EventsManager.SliceCanceledEvent.Invoke();
            }
        }
#endif

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
            {
                inProcess = true;
                knifeMove(knifeFinalPos.position);
                //EventsManager.SliceStartEvent.Invoke();
            }
            if (Input.GetMouseButtonUp(0))
            {
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(knife.transform.DOMove(knifeStartPos, 0.5f));

                //EventsManager.SliceCanceledEvent.Invoke();
            }
#endif


        }
        void knifeMove(Vector3 transformFinal)
        {
            knife.transform.position = Vector3.MoveTowards(knife.transform.position, transformFinal, Time.deltaTime * knifeSpeed);
       

        }
        void objMove(Vector3 transformFinal)
        {
            objToSlice.transform.position = Vector3.MoveTowards(objToSlice.transform.position, transformFinal, Time.deltaTime * objToSliceSpeed);
        }


    }

