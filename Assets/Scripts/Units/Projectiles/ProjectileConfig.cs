using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Projectile Config")]
public class ProjectileConfig : ScriptableObject
{
   
    //Main Body
    [Header("Projectile Data")]
    [SerializeField] public string proj_tag;
    [SerializeField] public float moveSpeed = 1f;
    [SerializeField] public GameObject explosionPrefab;
    [SerializeField] public Sprite explosionSprite;
    [SerializeField] public RuntimeAnimatorController explosionAnimatorController;

    [Header("애니메이션")]
    [SerializeField] public Sprite sprite;
    [SerializeField] public RuntimeAnimatorController meleeController;
    [SerializeField] public bool verticalExplosion = false;
    [SerializeField] public bool isSkill = false;
    [SerializeField] public string txt_projectile_name;


    [Header("이동방식")]
    [SerializeField] public  ProjectileMoveType moveType;

    [Header("효과")]
    [SerializeField] public bool isPenetration = false;
    [SerializeField] public bool isSplash = false;
    [SerializeField] public float splashModifier = 0.5f;
    [SerializeField] public bool isInstaKill = false;

    [SerializeField] public int numBounce = 0;
    [SerializeField] public float bounceRange = 0f;


    [SerializeField] float skillDamageModifier = 0f;


    [Header("버프 얹기")]
    [SerializeField] List<Buff> customBuff = new List<Buff>();
    [SerializeField] public float influenceRange = 0f;

    public float GetSkillDamageModifier() => skillDamageModifier;
    public void SetSkillDamageModifier(float a) { 
        skillDamageModifier = a; 
    }


    public void AddCustomBuff(Buff custom)
    {
        customBuff.Add(custom);
    }
    public void ResetBuffs( )
    {
        customBuff = new List<Buff>();
    }
    public List<Buff> GetCustomBuffs() => customBuff;
}
public enum ProjectileMoveType { 
NORMAL,INDIRECT,INSTA,BEAM,DIRECTED_VECTOR

}
