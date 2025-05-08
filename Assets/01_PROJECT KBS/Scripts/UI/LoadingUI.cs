using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class LoadingUI : MonoBehaviour
    {
        public float LoadingPercentage
        {
            set
            {
                currentLoadingPercentage = Mathf.Clamp01(value);
                loadingBar.fillAmount = currentLoadingPercentage;
                percentageText.text = $"{currentLoadingPercentage:0} %";
                
            }
        }

        [SerializeField] private UnityEngine.UI.Image loadingBar;
        [SerializeField] private TMPro.TextMeshProUGUI percentageText;

        private float currentLoadingPercentage = 0f;
    }
}
