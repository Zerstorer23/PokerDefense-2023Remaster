using System;
using System.Collections;
using UnityEngine;
using static ConstantStrings;
[System.Serializable]
public abstract class Skill
{
    //Config
    public SkillType skillType = SkillType.TIMED;
    public string txt_skill_name;
    public string[] txt_skill_desc;
    public int skillLevel = 0;
    public string skill_uid;
    public Sprite skill_icon;
    public bool isLocked = false;
    public bool activeAdded = false;

    //  [Header("특수탄 1")]
    //  public ProjectileConfig projectile_config;
    //  [Header("특수탄 2")]
    //  public ProjectileConfig burnig_projectile_config;


    /// </summary>
    protected float activatedTime;
    protected float activationFrequency;

    protected GameObject caster;
    public string skill_user_uid;
    public Tower towerComponent;

    //Active info
    protected bool canToggleAuto = false;
    //protected bool isAutoToggled = false;
    protected int skillSlot = -1;
    protected GameObject skillTarget;
    protected ProjectileConfig customProj;
    protected float currentStack = 1;
    protected float maxStack = 3;
    protected float healPerTurn = 0;


    //조건식 패시브등등이 지금 활성화 되었는지. SetActivation Status함수로 조정
    protected bool isActivated = false;
    public bool NotForKuroi = false;
    public bool canTargetTower = false;
    public bool isTargetLand = false;//지상 타겟은 vector3만 사용함
    protected Vector3 targetLocation; // 지상 타겟팅류
    protected float effectRange_visual = -1f;

    //Flag
    protected bool requirePostpone = false;
    protected float postponeTime = 0f;


    protected bool[] upgrade_mutex = { false, false, false, false, false };

    protected void SetInformation(SkillConfig config)
    {
        txt_skill_name = config.skill_name_key;
        txt_skill_desc = config.skill_desc_key;
        skill_uid = config.skill_uid;
        skillType = config.skillType;
        activationFrequency = config.ActivationFrequency;
        skillSlot = config.ActiveSkill_Slot;
        skill_icon = config.skill_icon;

        if (skillType == SkillType.ACTIVE)
        {
            EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, IncrementActiveStack);
            healPerTurn = config.ActivationFrequency;
        }
    
}

    private void IncrementActiveStack(EventObject eo)
    {
        currentStack += healPerTurn;
        if (currentStack > maxStack)
            currentStack = maxStack;
    }

    public abstract bool ProcessAbility();
    public float GetActivationFrequency()
    {
        return activationFrequency;
    }

    public void Initialise()
    {
        Initialise_child();
        Initialise_base();
    }

    protected virtual void Initialise_child()
    {

    }

    protected void Initialise_base()
    {
        EventManager.StartListening(MyEvents.EVENT_SKILL_UPGRADED, OnSkillUpgraded);
        InitUpgrades();
    }

    public void OnDestroy()
    {
        OnDestroy_base();
        OnDestroy_child();
    }

    protected virtual void OnDestroy_child()
    {
    }

    protected void OnDestroy_base()
    {
        EventManager.StopListening(MyEvents.EVENT_SKILL_UPGRADED, OnSkillUpgraded); 
        if (skillType == SkillType.ACTIVE)
        {
            EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, IncrementActiveStack);
        }
        if (customProj != null)
        {
            GameObject.Destroy(customProj);
        }
    }

    public void Activate()
    {
        bool result = ProcessAbility();
        if (result)
        {
            activatedTime = Time.time;
            if (skillType == SkillType.ACTIVE)
            {
                currentStack--;
            }
        }
        if (requirePostpone)
        {
            activatedTime = Time.time + postponeTime;
            requirePostpone = false;
        }
    }

    public float GetSkillEffectRadius() => effectRange_visual;


    public bool CanToggleAuto() => canToggleAuto;


    public void ActivateActiveSkill()
    {

    }
    protected void OnSkillUpgraded(EventObject eo)
    {
        string upgradedUnit = eo.GetString();
        bool isFutami = upgradedUnit.Equals(ConstantStrings.AMI) && skill_user_uid.Equals(MAMI);
        if (upgradedUnit.Equals(skill_user_uid) || isFutami)
        {
            skillLevel = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, skill_user_uid);
            switch (skillLevel)
            {
                case 1:
                    DoUpgrade_one();
                    break;
                case 2:
                    DoUpgrade_two();
                    break;
                case 3:
                    DoUpgrade_three();
                    break;
                case 4:
                    DoUpgrade_four();
                    break;

            }
            CheckUpgradeMutex();

        }
    }
    protected void InitUpgrades()
    {
        int currUp = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, skill_user_uid);
        if (skill_user_uid.Equals(MAMI))
        {
            currUp = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, AMI);
        }

        // Debug.Log(skill_user_uid+" curr up " + currUp);
        if (currUp >= 1)
        {
            DoUpgrade_one();
        }
        if (currUp >= 2)
        {
            DoUpgrade_two();
        }
        if (currUp >= 3)
        {
            DoUpgrade_three();
        }
        if (currUp >= 4)
        {
            DoUpgrade_four();
        }


         CheckUpgradeMutex();
    }

    private void CheckUpgradeMutex()
    {
        Debug.Assert(!upgrade_mutex[skillLevel], "Upgrade Mutex broken!!" + skill_user_uid);
        upgrade_mutex[skillLevel] = true;
    }


    protected abstract void DoUpgrade_one();
    protected abstract void DoUpgrade_two();
    protected abstract void DoUpgrade_three();
    protected abstract void DoUpgrade_four();

    public float GetCooldown()
    {
        float nextTime = activatedTime + GetActivationFrequency();
        float cool = nextTime - Time.time;
        return cool;
    }
    public void SetSkillTarget(GameObject target) => skillTarget = target;
    public void SetSkillTarget(Vector3 _targetLocation) => targetLocation = _targetLocation;

    #region Tools for children skills

    protected bool ActiveSkill_HasTarget(float mutexTime = 5f, TowerCharacter mutexTower = 0)//Mutex with buffType. enemy already in influence of this buff wont be selected.
    {
        //KUROI AUTO
        if (towerComponent.owner == Owner.KUROI)
        {
            skillTarget = FindMVPTarget(mutexTime, mutexTower);
            if (!skillTarget) return false;
            //KUROI AUTO MODE
            if (isTargetLand)
            {
                SetSkillTarget(skillTarget.transform.position);
            }
            else
            {
                SetSkillTarget(skillTarget);
            }
            return true;
        }

        //PLAYER ACTIVE
        if (isTargetLand)
        {
            return true;
        }
        return (skillTarget != null);
    }
    public GameObject FindMVPTarget(float mutexTime =5f, TowerCharacter mutexTower = 0)
    {
        //적 타겟팅이면 적 우선순위
        GameObject target = null;
        if (canTargetTower)
        {
            target = GameSession.GetGameSession().towerSpawner.GetMostValuable(towerComponent.owner, mutexTower);
        }
        if(!target) {
            target = GameSession.GetGameSession().enemySpawner.GetMostValuable();
        }
        if (target) {
            target.GetComponent<BuffManager>().SetSkillTargetMutex(mutexTime);
            return target;
        }
        //아니면 체력 높은 적
        return null;
    }


    protected IEnumerator SetActivationStatus(float seconds)
    {
        ToggleActivationStatus(true);
        yield return new WaitForSeconds(seconds);
        ToggleActivationStatus(false);
    }
    #endregion

    #region Getter and Setter
    protected void ToggleActivationStatus(bool status) => isActivated = status;
    protected void RequestPostponeTime(float seconds)
    {
        requirePostpone = true;
        postponeTime = seconds;
    }

    public bool IsActivated() => isActivated;
    public void SetCaster(GameObject caster)
    {
        this.caster = caster;
        this.towerComponent = caster.GetComponent<Tower>();
        this.skill_user_uid = towerComponent.GetUID();

    }
    public GameObject GetCaster() => caster;
    public int GetSkillSlot() => skillSlot;
    public float GetActivatedTIme() => activatedTime;
    public bool IsCasterActive() => caster.activeSelf;
    public float Active_GetMaxStack() => maxStack;
    public float Active_CurrentStack() => currentStack;
    public float Active_HealPerTurn() => healPerTurn;
    public string GetName() => txt_skill_name;
    #endregion
}

public enum SkillType
{
    PASSIVE_CONDITIONAL, PASSIVE_PERMANENT, ACTIVE, TIMED
}
public enum SkillActivationResult
{
    SUCCESS, FAIL, REQUIRE_POSTPONE
}
