using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public interface IInteractionData
    {
        public string ID { get; }
        public Sprite ActionIcon { get; }
        public string ActionMessage { get; }
    }
}
