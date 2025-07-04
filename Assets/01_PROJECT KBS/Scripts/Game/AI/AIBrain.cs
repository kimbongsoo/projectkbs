using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KBS
{
    public enum AIState
    {
        Combat,
        Patrol,
    }

    public class AIBrain : MonoBehaviour
    {
        public AIController AIController => controller;
        public CharacterBase TargetCharacter => targetCharacter;

        public AIState initState = AIState.Patrol;
        public Transform[] patrolPoints;

        private AIController controller;
        private CharacterDetectSensor detectSensor;

        private AIState currentAIState;
        private IState currentState;
        private CombatState combatState;
        private PatrolState patrolState;
        private Vector3[] patrolPositions;

        private CharacterBase targetCharacter;


        private void Awake()
        {
            patrolPositions = patrolPoints.Select(p => p.position).ToArray(); //Transform 배열을 Vector3 배열로 변환

            controller = GetComponent<AIController>();
            combatState = new CombatState(this);
            patrolState = new PatrolState(this, patrolPositions);

            detectSensor = GetComponentInChildren<CharacterDetectSensor>();
            detectSensor.OnDetectedCharacter += OnDetectedCharacter;
            detectSensor.OnLostCharacter += OnLostChatacter;
        }

        private void Start()
        {
            SetState(initState); // 초기 상태 설정
        }

        private void Update()
        {
            currentState?.Update(); // 현재 상태 업데이트
        }

        public void SetState(AIState newState)
        {
            currentState?.Exit(); // 현재 상태 종료

            //새로운 상태 설정
            currentAIState = newState;
            switch (newState)
            {
                case AIState.Patrol: currentState = patrolState; break;
                case AIState.Combat: currentState = combatState; break;
            }

            currentState?.Enter(); // 새로운 상태 시작
        }

        void OnDetectedCharacter(CharacterBase character)
        {
            // AI Brain 입장에서, Sensor를 통해서, "Player" 를 감지했을 때
            targetCharacter = character; //타겟 캐릭터 설정

            SetState(AIState.Combat);
        }

        void OnLostChatacter(CharacterBase character)
        {
            targetCharacter = null; //타겟 캐릭터 해제

            SetState(AIState.Patrol);
        }
    }
}
