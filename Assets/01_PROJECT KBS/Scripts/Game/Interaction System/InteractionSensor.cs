using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class InteractionSensor : MonoBehaviour
    {
        [SerializeField] private float sensorRadius = 2f;
        private Rigidbody sensorRigidbody;
        private SphereCollider sensorCollider;

        private Collider[] overlappedByPulse = new Collider[32];


        void Awake()
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            sensorRigidbody = gameObject.AddComponent<Rigidbody>();
            sensorRigidbody.isKinematic = true;

            sensorCollider = gameObject.AddComponent<SphereCollider>();
            sensorCollider.radius = sensorRadius;
            sensorCollider.isTrigger = true;
        }

        // 수동으로 센서 기능을 한번 실행해보는 함수
        public void PulseManually()
        {
            int overlappedCount = Physics.OverlapSphereNonAlloc(transform.position, sensorRadius, overlappedByPulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            IInteractionProvider interactionProvider = other.GetComponent<IInteractionProvider>();
            if (interactionProvider != null)
            {
                // TODO : 상호작용 UI에서 해당 항목을 추가
                Debug.Log($"Detect Provider!! : {other.gameObject.name}");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractionProvider interactionProvider = other.GetComponent<IInteractionProvider>();
            if (interactionProvider != null)
            {
                // TODO : 상호작용 UI에서 해당 항목을 제거
                Debug.Log($"Lost Provider Object!! : {other.gameObject.name}");
            }   
        }
    }
}
