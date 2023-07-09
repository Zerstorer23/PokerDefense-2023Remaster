using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI text;

    public void SetInfo(Color color, int damage, float significance, bool isHeal) {
        text.faceColor = color;
        if (isHeal)
        {

            text.text = "+" + damage.ToString();
        }
        else {
            text.text =  damage.ToString();
        }
        SetSize(significance);
    }
    private void SetSize(float significance) {
        text.fontSize = Mathf.Max(significance,0.25f);
    
    }

    public void Retire() {
        ObjectPool.SaveObject(ConstantStrings.OBJ_DAMAGE_SIGN,gameObject);
    }

}
