using UnityEngine;
using UnityEngine.UI;
using TMPro;
    public class Vie: MonoBehaviour
    {
        public float vieMax = 100f;
        public float vie = 100f;
        
        public Image vieBar;
        public TextMeshProUGUI vieText;
        
        void Update()
        {
            vieBar.fillAmount = vie / vieMax;
            vieText.text = vie + " / " + vieMax;
        }
    }