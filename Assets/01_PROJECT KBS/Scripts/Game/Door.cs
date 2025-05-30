using UnityEngine;

namespace KBS
{
    public class Door : MonoBehaviour
    {
        public Transform doorAxis; 
        public float openDoorAngle = -90f;
        public float closeDoorAngle = 0f;
        public float doorSpeed = 3f;
        private bool isPlayerEnter = false;

        private void Update()
        {
            float targetAngle = isPlayerEnter ? openDoorAngle : closeDoorAngle;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            doorAxis.localRotation = Quaternion.Lerp(doorAxis.localRotation, targetRotation, Time.deltaTime * doorSpeed);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerEnter = true;
                Debug.Log("Player Entered");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerEnter = false;
                Debug.Log("Player Exit");
            }
        }
    }
}
