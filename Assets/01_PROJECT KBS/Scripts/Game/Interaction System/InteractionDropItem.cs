using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class InteractionDropItem : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => interactionDatas;

        [SerializeField] private Material[] itemGradeMaterials = new Material[5];
        [SerializeField] private MeshRenderer visualRenderer;
        

        private List<IInteractionData> interactionDatas = new();

        public void Initialize(InteractionDropItemData itemData)
        {
            int index = Mathf.Clamp(itemData.ItemGrade - 1, 0, itemGradeMaterials.Length -1);
            visualRenderer.material = itemGradeMaterials[index];
        }

        public void Interact(IInteractionData data)
        {

        }
    }
}
