using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class InteractionItemBox : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => new List<IInteractionData>()
        {
            boxData,
        };

        [SerializeField] private InteractionItemBoxData boxData;
        [SerializeField] private Transform spawnLocation;

        public void Interact(IInteractionData data)
        {
            //TODO : Drop Item을 드롭한다.
            // var itemBoxData = data as InteractionItemBoxData;
            var dropItemPrefab = Resources.Load<InteractionDropItem>("Interaction Prefabs/Interaction Drop Item");
            boxData.DropItems.ForEach(dropData =>
            {
                var newDropItem = Instantiate(dropItemPrefab);
                newDropItem.Initialize(dropData);
                newDropItem.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);

                Vector3 forward = spawnLocation.forward;

                // 중심축에서 좌우로 퍼지는 각도 설정
                float anngleH = UnityEngine.Random.Range(-60f, 60f);
                float anngleV = UnityEngine.Random.Range(45f, 60f);

                // 각도를 방향 벡터로 변환
                Quaternion rotationH = Quaternion.AngleAxis(anngleH, Vector3.up);
                Quaternion rotationV = Quaternion.AngleAxis(-anngleV, spawnLocation.right);
                Vector3 direction = rotationH * rotationV * forward;

                Vector3 spawnPos = spawnLocation.position + Vector3.up * 0.2f;
                if (newDropItem.TryGetComponent<Rigidbody>(out var rb))
                {
                    float force = UnityEngine.Random.Range(30, 45f); //포물선 탄도에 적당 초기 속도
                    rb.AddForce(direction.normalized * force, ForceMode.Impulse);
                }
            });

            //상자를 파괴한다.
            Destroy(gameObject);
        }

    }
}
