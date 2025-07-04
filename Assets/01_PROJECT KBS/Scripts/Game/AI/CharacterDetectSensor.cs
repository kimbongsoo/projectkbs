using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class CharacterDetectSensor : MonoBehaviour
    {
        public event System.Action<CharacterBase> OnDetectedCharacter;
        public event System.Action<CharacterBase> OnLostCharacter;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.transform.TryGetComponent(out CharacterBase character))
                {
                    OnDetectedCharacter?.Invoke(character);
                }
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.transform.TryGetComponent(out CharacterBase character))
                {
                    OnLostCharacter?.Invoke(character);
                }
            }
        }
    }
}
