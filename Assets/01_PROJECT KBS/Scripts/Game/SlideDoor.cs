using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class SlideDoor : MonoBehaviour
    {
        public Transform slideDoor;
        public Vector3 openDoorPosition;
        public Vector3 closeDoorPosition;
        public float doorSpeed = 3f;
        private bool isPlayerEnter = false;
        void Start()
        {
            closeDoorPosition = slideDoor.localPosition;
            openDoorPosition = closeDoorPosition + new Vector3(0, 5, 0);
        }

    
        void Update()
        {
            Vector3 targetPos = isPlayerEnter ? openDoorPosition : closeDoorPosition;
            slideDoor.localPosition = Vector3.MoveTowards(slideDoor.localPosition, targetPos, Time.deltaTime * 5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                isPlayerEnter = true;
                Debug.Log("Player Entered");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                isPlayerEnter = false;
                Debug.Log("Player Exit");
        }
    }
}
