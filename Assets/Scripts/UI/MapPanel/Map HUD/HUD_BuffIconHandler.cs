using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_BuffIconHandler : MonoBehaviour
{
    internal HUD_BuffDescriptionHandler descriptionBox;
    Buff thisBuff = null;

    public void EnableBuffDescription(bool enable) {
        if (enable)
        {
            if (thisBuff != null)
            descriptionBox.SetDisplay(thisBuff);
        }
        descriptionBox.SetVisibility(enable);
    }

    internal void SetInformation(Buff buff)
    {
        thisBuff = buff;
        GetComponent<Image>().sprite = Buff.GetBuffImage(buff.GetBuffType());
    }
}
