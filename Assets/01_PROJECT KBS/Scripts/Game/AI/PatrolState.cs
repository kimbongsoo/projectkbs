using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KBS
{
    [System.Serializable]
    public class PatrolState : IState
    {
        // < Patrol State의 생성자>
        public PatrolState(AIBrain brain, Vector3[] arrPatrolPoint)
        {
            this.brain = brain;
            this.patrolPoints = arrPatrolPoint;
        }
        public AIBrain AIBrain => brain;
        public Vector3[] patrolPoints;
        public float randomWaitTime = 2f;
        public int patrolIndex = 0;

        private float patrolTimer = 0;
        private bool isWaiting = false;

        private AIBrain brain;

        public void Enter()
        {
            // Patrol State에 진입한 첫 순간을 의미
            brain.AIController.OnDestinationReached += OnDestinationReachted; // 목적지 도착 이벤트 구독
            ExecutePartolPlan(0); // 첫 번째 인덱스의 포인트로 목적지 설정
        }


        public void Update()
        {
            if (false == isWaiting)
                return;
            if (Time.time > patrolTimer + randomWaitTime)
            {
                int nextIndex = (patrolIndex + 1) % patrolPoints.Length; //다음 인덱스 계산 (순환)
                ExecutePartolPlan(nextIndex);
            }
        }

        public void Exit()
        {
            brain.AIController.OnDestinationReached -= OnDestinationReachted; // 목적지 도착 이벤트 구독 해제
        }

        private void ExecutePartolPlan(int index)
        {
            isWaiting = false; // 대기 상태 해제
            patrolIndex = index; // Patrol 인덱스 지정
            randomWaitTime = Random.Range(3f, 5f); // 랜덤 대기시간 설정
            brain.AIController.SetDestination(patrolPoints[patrolIndex]); // 첫 번째 목적지 설정
        }

        void OnDestinationReachted()
        {
            isWaiting = true;
            patrolTimer = Time.time;
        }

    }
}
