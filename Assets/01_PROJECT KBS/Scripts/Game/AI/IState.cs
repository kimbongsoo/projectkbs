using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public interface IState
    {
        public AIBrain AIBrain { get; }

        public void Enter();

        public void Update();

        public void Exit();
    }
}
