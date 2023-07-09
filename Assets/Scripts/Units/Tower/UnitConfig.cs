using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PokerHand;

[CreateAssetMenu(menuName = "My Unit Config")]
public class UnitConfig : ScriptableObject
{

    [SerializeField] public string uid;
    [SerializeField] public TowerCharacter characterID;
    [SerializeField] public string txt_name;
    [SerializeField] public string txt_tutorial_key;
    [SerializeField] public string colorHex;    
    [SerializeField] public int sellValue_Dance = 1;
    [SerializeField] public int sellValue_Visual = 0;
    [SerializeField] public int sellValue_Vocal = 100;
    [SerializeField] public int unitMass;
    public PokerHandType baseHand;

    //Main Body
    [Header("Main Body")]
  //  [SerializeField] public RuntimeAnimatorController myAnimController;
    [SerializeField] public CharacterConfig characterConfig;



    [SerializeField] public ProjectileConfig myProjectileConfig;

    //Body
    [Header("Body")]
    [SerializeField] public Sprite myPortraitSprite;
    [SerializeField] public Sprite myBodySprite; 



     [Header("Body")]
    [SerializeField] public Vector3 myGunPos;
    [SerializeField] public bool isMelee;

    [Header("Data")]
    [SerializeField] public float attackDamage;


    [SerializeField] public float attackDelay;
    [SerializeField] public float attackDistance;
    [SerializeField] public float attackUpgradeIncrement;
    [SerializeField] public float VisualUpgradeIncrement;
    [SerializeField] public float AI_Flavour=1;

    //Skill
    [Header("Skill")]
    [SerializeField] public List<SkillConfig> skillConfigs;
    internal string GetUID() => uid;
    internal TowerCharacter GetCharacterID() => characterID;
    internal float GetAttackUpgradeIncrement() => attackUpgradeIncrement;
    internal float GetAttackDamage() => attackDamage;
    internal Sprite GetPortraitSprite() => myPortraitSprite;
    //    [SerializeField] public List<GameObject> skills;

    internal float GetFinalVocalDamage() { 
        return attackDamage + (attackUpgradeIncrement * UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, uid));
    
    }
    internal float GetFinalVisualDamage()
    {
        return 1 + (VisualUpgradeIncrement * UpgradeManager.GetUpgradeValue(UpgradeType.VISUAL, uid));
    }
    internal float GetFinalRawDPS()
    {
        float vocalMod = GetFinalVocalDamage();
        float visualMod = GetFinalVisualDamage();
        float final = (vocalMod * visualMod) * AI_Flavour;

        return final / attackDelay;
    }

    [Header("Skill Upgrade Text")]
    public string dance_upgrade_text_1 = "TXT_KEY_DANCE_UPGRADE_1_";
    public string dance_upgrade_text_2 = "TXT_KEY_DANCE_UPGRADE_2_";
    public string dance_upgrade_text_3 = "TXT_KEY_DANCE_UPGRADE_3_";
    public string dance_upgrade_text_4 = "TXT_KEY_DANCE_UPGRADE_4_";


    
}
