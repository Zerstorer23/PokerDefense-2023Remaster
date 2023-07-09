using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ProjectileHandler : MonoBehaviour
{
    [Header("Projectile Panel")]
    [SerializeField] Image projImage;
    [SerializeField] Text projName;
    [SerializeField] Text projUpgrade;
    ProjectileConfig pConfig;

    public void SetProfectileInfo(string towerID, ProjectileConfig _pConfig)
    {
        pConfig = _pConfig;
        SetVisibility(true);
        if (pConfig == null)
        {
            projImage.enabled = false;
            projName.text = "";
            projUpgrade.text = "";
            return;
        }

        projImage.enabled = true;
        projImage.sprite = pConfig.sprite;
        projName.text = LocalizationManager.Convert(pConfig.txt_projectile_name);
        projUpgrade.text = "+" + UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, towerID);
    }
    public void SetVisibility(bool enable)
    {
        gameObject.SetActive(pConfig != null && enable);
    }

}
