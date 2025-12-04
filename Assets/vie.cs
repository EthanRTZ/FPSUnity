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
            //calcul de la vie
            vieBar.fillAmount = vie / vieMax;
            vieText.text = vie + " / " + vieMax;
        }
    }