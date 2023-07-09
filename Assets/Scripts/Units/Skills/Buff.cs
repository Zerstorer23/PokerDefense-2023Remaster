
using System;
using UnityEngine;




public enum BuffType { 
    DEFAULT = default,
    ATTACK_PERC, ATTACK,KNOCKBACK,SLOW,CHANGE_PROJECTILE,GOLD_BONUS,FUTAMI,SKILL_DAMAGE_MOD
        ,DEFENSE_MOD_PERC,ATTACK_SPEED
        ,TREND,ATTACK_PERC_LOW,HEAL_PERC
}
public class Buff 
{
    BuffType buffType;
    public double buffAmount = 0; //%
    float buffEndTime;
    float buffTime; //0보다 작으면 영구
   // public Skill triggerSkill;
    public Tower triggerTower;

    public ProjectileConfig originalProjectile;
    public ProjectileConfig burningProjectile;
    string MutexID ="";
    internal int Mutex_Manifold=0;
    internal string txt_buff_name;

    public Buff(BuffType bType, double _buffAmount, float _buffTime, Tower triggeredBy, string txt_name)
    {
        buffType = bType;
        this.buffAmount = _buffAmount;
        buffTime = _buffTime;
        buffEndTime = Time.time + _buffTime;
        triggerTower = triggeredBy;
        txt_buff_name = txt_name;
        Debug.Assert(triggeredBy != null, "Tower comp null!");
    }
    public Buff(BuffType bType, float effectTime, Tower triggeredBy, string txt_name)
    {
        buffType = bType;
        buffTime = effectTime;
        buffEndTime = Time.time + effectTime;
        triggerTower = triggeredBy;
        txt_buff_name = txt_name;
        Debug.Assert(triggeredBy != null, "Tower comp null!");
    }
    public Buff Clone() {
        Buff clone = new Buff(this.buffType, this.buffAmount,this.buffTime, this.triggerTower, this.txt_buff_name);
        clone.originalProjectile = this.originalProjectile;
        clone.burningProjectile = this.burningProjectile;
        clone.MutexID = this.MutexID;
        clone.Mutex_Manifold = this.Mutex_Manifold;
        buffEndTime = Time.time + buffTime;
        Debug.Assert(clone.triggerTower != null, "Tower comp null!");
        return clone;
    }
    public void SetBuffEndTime(float newtime) {
        buffEndTime = newtime;
    }
    internal string GetBuffDescription()
    {
        string perc = ((int)(buffAmount * 100)).ToString();

        if (buffAmount < 0)
        {
            perc = "-" + perc;
        }
        else {
            perc = "+" + perc;
        }

        switch (buffType)
        {
            case BuffType.ATTACK_PERC:
                return LocalizationManager.Convert("TXT_KEY_ATTACK") + " " + perc+"%";
            case BuffType.ATTACK:
                return LocalizationManager.Convert("TXT_KEY_ATTACK") + " " + (int)(buffAmount);
            case BuffType.KNOCKBACK:
                return LocalizationManager.Convert("TXT_KEY_STAT_STUN");
            case BuffType.SLOW:
                return LocalizationManager.Convert("TXT_KEY_STAT_MOVE_SPEED") + " " + perc + "%";
            case BuffType.CHANGE_PROJECTILE:
                return LocalizationManager.Convert("TXT_KEY_STAT_CHANGE_PROJECTILE");
            case BuffType.GOLD_BONUS:
                return LocalizationManager.Convert("TXT_KEY_GOLD_BONUS") + " " + perc + "%";
            case BuffType.FUTAMI:
                return LocalizationManager.Convert("TXT_KEY_FUTAMI") + " " + perc + "%";
            case BuffType.SKILL_DAMAGE_MOD:
                return LocalizationManager.Convert("TXT_KEY_SKILL_DAMAGE_MOD") + " " + perc + "%";
            case BuffType.DEFENSE_MOD_PERC:
                return LocalizationManager.Convert("TXT_KEY_DEFENSE") + " " + perc + "%";
            case BuffType.ATTACK_SPEED:
                return LocalizationManager.Convert("TXT_KEY_ATTACK_SPEED") + " " + perc + "%";
            case BuffType.TREND:
                return LocalizationManager.Convert("TXT_KEY_TREND");
            case BuffType.ATTACK_PERC_LOW:
                return LocalizationManager.Convert("TXT_KEY_ATTACK") + " -" + (buffAmount * 100f) + "%";
            case BuffType.HEAL_PERC:
                return LocalizationManager.Convert("TXT_KEY_HEAL") + " " + perc + "%";
        }
        return "UNKNOWN";
    }

    public static Sprite GetBuffImage(BuffType buffType)
    {

        switch (buffType)
        {
            case BuffType.ATTACK_PERC:
                return ActiveSkillManager.attBuff;
            case BuffType.ATTACK_PERC_LOW:
                return ActiveSkillManager.attLowBuff;
            case BuffType.ATTACK_SPEED:
                return ActiveSkillManager.attSpd;
            case BuffType.CHANGE_PROJECTILE:
                return ActiveSkillManager.makotoBuff2;
            case BuffType.SKILL_DAMAGE_MOD:
                return ActiveSkillManager.harukaBuff;
            case BuffType.DEFENSE_MOD_PERC:
                return ActiveSkillManager.shieldBreak;
            case BuffType.HEAL_PERC:
                return ActiveSkillManager.damageMod; //TODO need heal perc icon
            case BuffType.FUTAMI:
                return ActiveSkillManager.damageMod;
            case BuffType.GOLD_BONUS:
                return ActiveSkillManager.yayoiBuff;
            case BuffType.SLOW:
                return ActiveSkillManager.slowBuff;
            case BuffType.KNOCKBACK:
                return ActiveSkillManager.stun;
            case BuffType.ATTACK:
                return ActiveSkillManager.attBuff;
            case BuffType.TREND:
                return ActiveSkillManager.attBuff;
        }
        return ActiveSkillManager.attBuff;
    }


    public BuffType GetBuffType() {
        return buffType;
    }
    public bool IsBuffFinished() {
        if (buffTime < 0) return false;
        return (Time.time >= buffEndTime);    
    }

 
    public float GetEndTime() {
        return buffEndTime;
    }
    public float GetEffectTime()
    {
        return buffTime;
    }


    public void StartTimer() {
        buffEndTime = Time.time + buffTime;
    }
    public void SetMutexID(string id) {
        this.MutexID = id;
        Mutex_Manifold = 1;
    }
    public void IncrementMutex() {
        Mutex_Manifold++;
    }
    public void ResetMutex()
    {
        Mutex_Manifold=0;
        this.MutexID = "";
    }
    public int DecrementMutex() {
        Mutex_Manifold--;
        return Mutex_Manifold;    
    }

    public string GetMutexID() {
        return MutexID;
    }
}
