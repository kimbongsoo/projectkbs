using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KBS
{
    public class AIController : MonoBehaviour
    {
        public Transform destinationPoint; //임시 목적지 변수
        private CharacterBase characterBase;
        private NavMeshAgent navAgent;

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            navAgent = GetComponent<NavMeshAgent>();

            navAgent.updatePosition = false; //NavMeshAgent의 위치 업데이트를 비활성화
            navAgent.updateRotation = false; //NavMeshAgent의 회전 업데이트를 비활성화
        }

        private void Start()
        {
            navAgent.speed = characterBase.moveSpeed; // NavMeshAgent의 속도를 캐릭터의 이동 속도로 설정
        }

        private void Update()
        {
            navAgent.SetDestination(destinationPoint.position); // NavMeshAgent의 위치를 캐릭터의 위치로 설정
            navAgent.nextPosition = transform.position;

            if (navAgent.hasPath)
            {
                Vector3 moveDirection = (navAgent.steeringTarget - transform.position).normalized; // 목적지 방향 계산
                Vector2 input = new Vector2(moveDirection.x, moveDirection.z); // 2D 입력 벡터 생성

                characterBase.Move(input, 0); // 캐릭터 이동
            }
            else
            {
                characterBase.Move(Vector2.zero, 0);
            }
        }
    }

    
}
