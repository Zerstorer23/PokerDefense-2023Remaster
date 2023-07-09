/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Azusa_Slow : Skill
{
    *//*
               스킬 1 - 인스네어
	            n초간 속도감소
     *//*
    //CACHE


    bool auto = false;


    public Skill_Azusa_Slow(SkillConfig config)
    {
        canToggleAuto = auto;
        SetInformation(config);
        customProj = GameObject.Instantiate(config.projectile_config);
        isLocked = true;
        isTargetLand = true;
        effectRange_visual = customProj.influenceRange;
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
        //브루들링 쿨감
    }

    protected override void DoUpgrade_three()
    {
       // customProj.b *= 2f;
    }

    protected override void DoUpgrade_four()
    {

    }
}
*/