using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField] public UnitType unitType;
    //COMMON
    [SerializeField] List<Buff> Buffs_active = new List<Buff>();
    [SerializeField] public BuffIndicator buffIndicator;


    //TOWER
    [SerializeField] Tower tower;
    [SerializeField] double attackPercModifier = 0f;
    [SerializeField] double attackModifier = 0f;
    [SerializeField] double skillDamageModifier = 1f;
    internal double attackDelayModifier = 1f;


    //Status
    [SerializeField] internal int numStun = 0;
    [SerializeField] int numYayoi = 0;
    //   internal int numFutami = 0;
    public double rewardModifier = 1f;
    public double Futami_receiveDamageModifier = 1f;
    public double defenseMod = 1f;
    public double speedModifier = 1.0f;
    public double healModifier = 1.0f;

    internal bool isTargetOfSkill = false;
    public float targetEndTime=0f;

    void Awake()
    {
       // Debug.Log("buffmanager init start");
        if (unitType == UnitType.Tower)
        {
            tower = GetComponent<Tower>();
            //skillmanager, firepower..
            EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, OnWaveStarted);
        }
     //   Debug.Log("buffmanager init end");
    }
   
    private void OnDestroy()
    {
        if (unitType == UnitType.Tower)
        {
            EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, OnWaveStarted);
        }
    }


    private void Update()
    {
        CheckBuffDeactivations();
    }
    private void OnEnable()
    {
        if (unitType == UnitType.Mob)
        {
            RemoveAllBuff();
        }
    }

    private void OnWaveStarted(EventObject eo)
    {
        bool isTrend = UpgradeManager.IsTrending(tower.GetUID());
        buffIndicator.TriggerSuperbuff(isTrend);
    }



    private void CheckBuffDeactivations()
    {

        for (int i = 0; i < Buffs_active.Count; i++)
        {
            if (Buffs_active[i].IsBuffFinished())
            {
                RemoveBuff(Buffs_active[i]);
                Buffs_active.RemoveAt(i);
                i--;
            }

        }

        if (isTargetOfSkill) {
            if (Time.time > targetEndTime) isTargetOfSkill = false;
        }
    }


    internal void AddBuff_Mutex(Buff buff, string skill_id)
    {
        for (int i = 0; i < Buffs_active.Count; i++)
        {
            if (Buffs_active[i].GetMutexID().Equals(skill_id))
            {
                Buffs_active[i].IncrementMutex();
                return;
            }
        }
        AddBuff(buff);
    }
    internal void RemoveBuff_Mutex(string skill_id)
    {
        for (int i = 0; i < Buffs_active.Count; i++)
        {
            if (Buffs_active[i].GetMutexID().Equals(skill_id))
            {
                int remainUser = Buffs_active[i].DecrementMutex();
                if (remainUser <= 0)
                {
                    RemoveBuff(Buffs_active[i]);
                    Buffs_active.RemoveAt(i);
                }
                return;
            }
        }
    }
    internal void AddBuff(Buff buff)
    {
        //Some buffs just need to be overrided. Like Stun time.
        bool isTower = (tower != null);
        switch (buff.GetBuffType())
        {
            case BuffType.ATTACK:
                attackModifier += buff.buffAmount;
                break;
            case BuffType.ATTACK_PERC:
                attackPercModifier += buff.buffAmount;
                break;
            case BuffType.ATTACK_PERC_LOW:
                attackPercModifier -= buff.buffAmount;
                break;
            case BuffType.ATTACK_SPEED:
                attackDelayModifier += buff.buffAmount;
                break;

            case BuffType.TREND:
                DoTrendBuff(buff, true);
                break;
            case BuffType.KNOCKBACK:
                ApplyKnockBack();
                break;
            case BuffType.CHANGE_PROJECTILE:
                ChangeProjectile(buff, true);
                break;
            case BuffType.SKILL_DAMAGE_MOD:
                skillDamageModifier += buff.buffAmount;
                break;

            //For enemy
            case BuffType.SLOW:
                ChangeSpeedModifier(buff.buffAmount);
                break;

            case BuffType.HEAL_PERC:
                healModifier += buff.buffAmount;
                break;
            case BuffType.GOLD_BONUS:
                ApplyGoldBonus(buff,true);
                break;
            case BuffType.FUTAMI:
                ApplyFutami(buff, true);
                break;
            case BuffType.DEFENSE_MOD_PERC:
                if (isTower)
                {
                    return;
                }
                defenseMod += buff.buffAmount;
                break;
        }

      //  buff.StartTimer();
        Buffs_active.Add(buff);
        buffIndicator.AddBuffIndicator(buff.GetBuffType());


        //  Debug.Log(Time.time+" / Buff activated " + buff.GetType()+" by "+buff.buffAmount+" end time "+(buff.GetEndTime())+"Current size "+Buffs_active.Count);
    }

    private void RemoveAllBuff()
    {
        for (int i = 0; i < Buffs_active.Count; i++)
        {
            Buffs_active[i].ResetMutex();
            RemoveBuff(Buffs_active[i]);
        }
        Buffs_active = new List<Buff>();
    }

    private void DoTrendBuff(Buff buff, bool enable)
    {
        if (enable)
        {

            attackModifier += buff.buffAmount;
        }
        else
        {

            attackModifier -= buff.buffAmount;
        }



    }

    private void ChangeProjectile(Buff buff, bool enable)
    {
        if (tower == null) return;
        if (enable)
        {

            tower.firepowerManager.myProjConfig = buff.burningProjectile;
        }
        else
        {
            tower.firepowerManager.myProjConfig = buff.originalProjectile;
        }
        if (tower.firepowerManager.isMelee)
        {
            tower.firepowerManager.InitGun();
        }
    }

    public void RemoveBuff(Buff buff)
    {
        switch (buff.GetBuffType())
        {
            case BuffType.ATTACK:
                attackModifier -= buff.buffAmount;
                break;
            case BuffType.ATTACK_PERC:
                attackPercModifier -= buff.buffAmount;
                break;
            case BuffType.ATTACK_PERC_LOW:
                attackPercModifier += buff.buffAmount;
                break;
            case BuffType.ATTACK_SPEED:
                attackDelayModifier -= buff.buffAmount;
                break;
            case BuffType.KNOCKBACK:
                RemoveKnockback();
                break;
            case BuffType.TREND:
                DoTrendBuff(buff, false);
                break;
            case BuffType.CHANGE_PROJECTILE:
                ChangeProjectile(buff, false);
                break;
            case BuffType.SKILL_DAMAGE_MOD:
                skillDamageModifier -= buff.buffAmount;
                break;

            //Enemy
            case BuffType.SLOW:
                ChangeSpeedModifier(-buff.buffAmount);
                break;
            case BuffType.HEAL_PERC:
                healModifier -= buff.buffAmount;
                break;
            case BuffType.GOLD_BONUS:
                ApplyGoldBonus(buff, false);
                break;
            case BuffType.FUTAMI:
                ApplyFutami(buff, false);
                break;
            case BuffType.DEFENSE_MOD_PERC:
                defenseMod -= buff.buffAmount;
                break;
        }
        buffIndicator.RemoveBuffIndicator(buff.GetBuffType());

        // Debug.Log("Buff deactivated " + buff.GetType());
    }


    public void InstaKill(Tower attackBy, Owner _owner)
    {
        HealthPoint hpComp = GetComponent<HealthPoint>();
        if (_owner == Owner.KUROI && unitType == UnitType.Mob)
        {
            hpComp.HealCompletely();
        }
        else {
            hpComp.DoDeath(attackBy);
        }

        if (attackBy != null && !hpComp.invincible)
        {
            Color color = ConstantStrings.GetColorByHex(attackBy.myConfig.colorHex);
            hpComp.InstantiateDamageSign(color, transform, (int)hpComp.GetHP(), false);  
        }
    }
    internal void ChangeSpeedModifier(double _mod)
    {
        speedModifier += _mod;
        if (speedModifier < 0) speedModifier = 0.01f;
    }
    private void ApplyKnockBack()
    {
        numStun++;
        if (unitType == UnitType.Tower)
        {
            DoKnockBackAnimation(true);
        }
    }
    private void ApplyGoldBonus(Buff buff, bool apply)
    {
        if (apply)
        {
            numYayoi++;
            if (1+buff.buffAmount >= rewardModifier)
            {
                rewardModifier += buff.buffAmount;
            }
        }
        else {
            numYayoi--;
            if (numYayoi <= 0)
            {
                rewardModifier = 1f;
            }
        }

    }
    private void ApplyFutami(Buff buff, bool apply)
    {
        if (apply)
        {
            Futami_receiveDamageModifier += buff.buffAmount;
        }
        else
        {

            Futami_receiveDamageModifier -= buff.buffAmount;
        }

    }

    

    private void DoKnockBackAnimation(bool enabled)
    {
        tower.characterManager.SetAnimationBool("IsKnockBack", enabled);
    }


    private void RemoveKnockback()
    {
        numStun--;
        if (numStun <= 0)
        {
            if (unitType == UnitType.Tower)
            {
                //Tower
                DoKnockBackAnimation(false);

            }
        }
    }

    internal double GetModifiedAttackBonus()
    {
        return (attackPercModifier);
    }
    internal double GetBonusAttack() => attackModifier;
    internal double GetAttackSpeedModifier() => attackDelayModifier;
    internal double GetSkillDamageModifier() => skillDamageModifier;
    internal List<Buff> GetActiveBuffs() => Buffs_active;

    internal bool HasBuff(string skill_id)
    {
        for (int i = 0; i < Buffs_active.Count; i++)
        {
            if (Buffs_active[i].GetMutexID().Equals(skill_id))
            {
                return true;
            }
        }
        return false;
    }

    public void SetSkillTargetMutex(float amountTime) {
        isTargetOfSkill = true;
        targetEndTime = Time.time + amountTime;    
    }
}

public enum UnitType
{
    Tower, Mob
}
