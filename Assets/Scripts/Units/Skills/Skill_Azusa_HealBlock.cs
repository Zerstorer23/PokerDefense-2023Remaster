using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class Skill_Azusa_HealBlock : Skill
{
    /*
               스킬 1 - 인스네어
	            n초간 속도감소
     */
    //CACHE


    bool auto = false;
    float effectTime = 8f;
    float range;
    int baseCount;
    int additionalCount=0;
   public static int onePerVocal = 8;

    string objTag = "SKILL_AZUSA_HEALBLOCK";
    public Skill_Azusa_HealBlock(SkillConfig config)
    {
        NotForKuroi = true;
        canToggleAuto = auto;
        isTargetLand = true;
        SetInformation(config);
        effectRange_visual = range;

    }
    protected override void Initialise_child()
    {

    }

    public override bool ProcessAbility()
    {
        if (isLocked) return false;
        bool possible = ActiveSkill_HasTarget();
        if (possible)
        {
            SpawnUnit(targetLocation);
            Debug.Log(txt_skill_name + " fired to " + targetLocation);
        }
        return possible;
    }
    public void SpawnUnit(Vector3 _location)
    {
        Vector3 targetLocation = new Vector3(_location.x, _location.y, 0);
        GameObject Lexington = ObjectPool.PollObject(objTag, targetLocation, Quaternion.identity);
        if (Lexington == null)
        {
            GameObject underlingBase = towerComponent.activeSkillManager.GetAzusaUnderlingPrefab();
            Lexington = GameObject.Instantiate(underlingBase, targetLocation, Quaternion.identity);
            Lexington.transform.parent = GameSession.GetGameSession().ProjectileHome;
        }
        int currVocal = UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, towerComponent.GetUID());
        additionalCount = (int)(currVocal / onePerVocal);
        Lexington.GetComponent<Azusa_SkillUnit>().SetInformation(objTag,additionalCount, effectTime);
    }

    protected override void DoUpgrade_one()
    {
        Debug.Log("Unlock skill " + this.txt_skill_name);
        EventManager.TriggerEvent(MyEvents.EVENT_ADD_ACTIVE_SKILL, new EventObject(this));

    }

    protected override void DoUpgrade_two()
    {
        onePerVocal = 3;
        effectTime += 4f;
    }

    protected override void DoUpgrade_three()
    {
        healPerTurn *= 1.5f;
    }

    protected override void DoUpgrade_four()
    {

    }
}
/*
 
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Azusa_HealBlock : Skill
{



bool auto = false;
double buffModifier;
float effectiveTime;
Buff healDebuff;

public Skill_Azusa_HealBlock(SkillConfig config)
{
    NotForKuroi = true;
    canToggleAuto = auto;
    isTargetLand = true;
    isLocked = true;
    SetInformation(config);
    buffModifier = config.SuccessBuff_Modifier;
    effectiveTime = config.SuccessBuff_Time;

    customProj = GameObject.Instantiate(config.projectile_config);
    effectRange_visual = customProj.influenceRange;

}
protected override void Initialise_child()
{
    healDebuff = new Buff(BuffType.HEAL_PERC, buffModifier, effectiveTime, towerComponent, txt_skill_name);
    customProj.AddCustomBuff(healDebuff);
}

public override bool ProcessAbility()
{
    if (isLocked) return false;
    bool possible = ActiveSkill_HasTarget();
    if (possible)
    {
        Debug.Log(txt_skill_name + " fired to " + targetLocation);
        Fire_Skill(targetLocation);
    }
    return possible;
}
public void Fire_Skill(Vector3 targetLocation)
{
    float finalDamage = 0f;
    towerComponent.firepowerManager.PollProjectile(customProj, null, finalDamage, false, isLand: true, xPos: targetLocation.x, yPos: targetLocation.y);
}

protected override void DoUpgrade_one()
{
    isLocked = false;
    Debug.Log("Unlock skill " + this.txt_skill_name);
    EventManager.TriggerEvent(MyEvents.EVENT_ADD_ACTIVE_SKILL, new EventObject(this));

}

protected override void DoUpgrade_two()
{
}

protected override void DoUpgrade_three()
{
    healDebuff.buffAmount *= 2f;
    customProj.ResetBuffs();
    customProj.AddCustomBuff(healDebuff);
}

protected override void DoUpgrade_four()
{

}
}

 */