using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class CrossHairUI : MonoBehaviour
    {
        public static CrossHairUI Instance { get; private set; } = null;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = this;
            }
        }
        [SerializeField] private RectTransform crosshairTop;
        [SerializeField] private RectTransform crosshairBottom;
        [SerializeField] private RectTransform crosshairLeft;
        [SerializeField] private RectTransform crosshairRight;

        public float maxSpread = 300f;

        public void SetCrosshairSpread(float spread)
        {
            float spreadValue = Mathf.Clamp01(spread) * maxSpread;

            crosshairTop.anchoredPosition = new Vector2(0, spreadValue);
            crosshairBottom.anchoredPosition = new Vector2(0, -spreadValue);
            crosshairLeft.anchoredPosition = new Vector2(-spreadValue, 0);
            crosshairRight.anchoredPosition = new Vector2(spreadValue, 0);
        }
    }
}
