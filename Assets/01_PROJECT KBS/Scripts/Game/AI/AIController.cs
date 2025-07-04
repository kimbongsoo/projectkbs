using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KBS
{
    public class AIController : MonoBehaviour
    {
        private CharacterBase characterBase;
        private NavMeshAgent navAgent;

        public event System.Action OnDestinationReached;

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
            navAgent.nextPosition = transform.position;

            if (navAgent.hasPath && !navAgent.pathPending)
            {
                Vector3 moveDirection = (navAgent.steeringTarget - transform.position).normalized; // 목적지 방향 계산
                Vector2 input = new Vector2(moveDirection.x, moveDirection.z); // 2D 입력 벡터 생성

                characterBase.Move(input, 0); // 캐릭터 이동

                // 목적지에 도착했는지 판단
                if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    navAgent.isStopped = true;
                    navAgent.ResetPath();
                    OnDestinationReached?.Invoke();
                }
            }
            else
            {
                characterBase.Move(Vector2.zero, 0);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(destination);
        }

        public void Stop()
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }

        public void EquipWeapon()
        {
            characterBase.EquipWeapon();
            characterBase.IsAiming = true;
        }

        public void UnequipWeapon()
        {
            characterBase.HolsterWeapon();
            characterBase.IsAiming = false;
        }

        public void SetAiming(Vector3 aimingPoint)
        {
            characterBase.AimingPoint = aimingPoint;
        }

        public void Fire()
        {
            characterBase.Fire();
        }
    }

    
}
