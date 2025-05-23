using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class Door : MonoBehaviour
    {
        public Transform door;
        public float openDoorPosition;
        public float closeDoorPosition;
        public float doorSpeed = 3f;
        private bool isPlayerEnter = false;
    
        void Update()
        {
            float doorAngle = isPlayerEnter ? openDoorPosition : closeDoorPosition;
            Quaternion targetRotation = Quaternion.Euler(0, doorAngle, 0);
            door.localRotation = Quaternion.Lerp(door.localRotation, targetRotation, Time.deltaTime * doorSpeed);
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
