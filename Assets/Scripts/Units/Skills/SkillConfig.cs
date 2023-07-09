using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Skill Config")]
public class SkillConfig : ScriptableObject
{
    [Header("기본 정보")]
    public string skill_uid;
    public string skill_name_key;

    [Header("Skill Desc Text")]
    public string[] skill_desc_key = new string[4];

    public Sprite skill_icon;
    public bool canAffectKuroi;

    public SkillType skillType;
    [SerializeField] public float ActivationFrequency; //n초마다
    [SerializeField] public int ActiveSkill_Slot;
    [Header("추가 정보")]
    [SerializeField] public float SuccessPercentage ; //1.000 확률로


    [Header("기본 버프")]

    [SerializeField] public BuffType SuccessBuff_Type;
    [SerializeField] public float SuccessBuff_InfluenceRange; //1타일범위 inclusive에
    [SerializeField] public double SuccessBuff_Modifier; //25%화력보너스
    [SerializeField] public float SuccessBuff_Time; //5초간 지속
    [Header("예비 버프")]
    [SerializeField] public float FailBuff_Time ;


    [Header("특수탄 1")]
    public ProjectileConfig projectile_config;
    [Header("특수탄 2")]
    public ProjectileConfig burnig_projectile_config;


    internal static string GetDescription(string key, int skillLevel)
    {
      
        string converted = LocalizationManager.Convert(key);
        if (key.Contains("TXT_KEY_AZUSA_SKILL_SLOW_DESC"))
        {
            converted= DoAzusaSkill(key, skillLevel);
        }
        else if (key.Contains("TXT_KEY_YUKIHO_SKILL_DESC"))
        {
            converted = DoYukihoSkill(key, skillLevel);
        }
        return converted;
    }
    static string DoAzusaSkill(string txtKey, int danceLevel)
    {
        int vocalLevel = UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, ConstantStrings.AZUSA);
        int perVariable = Skill_Azusa_HealBlock.onePerVocal;
        int countVariable = 16 + vocalLevel / perVariable;
       return LocalizationManager.Convert(txtKey, countVariable.ToString(), perVariable.ToString());
    }
    static string DoYukihoSkill(string txtKey, int danceLevel)
    {
        int vocalLevel = UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, ConstantStrings.YUKIHO);
        int perVariable = Skill_Yukiho.addPerVocal;
        int countVariable = 16 + vocalLevel / perVariable;
        return  LocalizationManager.Convert(txtKey, countVariable.ToString(), perVariable.ToString());
    }
}
