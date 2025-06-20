using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    [CreateAssetMenu(fileName = "InteractionDropItemData", menuName = "KBS/Interaction/Drop Item Data")]
    public class InteractionDropItemData : ScriptableObject, IInteractionData
    {
        public string ID => itemId;
        public Sprite ActionIcon => itemIcon;
        public string ActionMessage => itemName;
        public int ItemGrade => itemGrade;

        [SerializeField] private string itemId;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private string itemName;
        [SerializeField] private int itemGrade;
    }
}
