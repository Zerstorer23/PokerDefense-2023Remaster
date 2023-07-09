using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Azusa_Kill : Skill
{
    /*
               스킬 1 - 인스네어
	            n초간 속도감소
     */
    //CACHE



    bool auto = false;

    public Skill_Azusa_Kill(SkillConfig config)
    {
        SetInformation(config);
        canToggleAuto = auto;
        canTargetTower = true;
        customProj = GameObject.Instantiate(config.projectile_config);
    }
  
    public override bool ProcessAbility()
    {
        bool possible = ActiveSkill_HasTarget(mutexTower:TowerCharacter.Azusa);
        if (possible)
        {
        //    Debug.Log(txt_skill_name + " fired to " + skillTarget.transform.position);
            Fire_Skill(); 
        }
        return possible;
    }


    public void Fire_Skill()
    {
        Tower tower = caster.GetComponent<Tower>();
        float finalDamage = 0f;
        tower.firepowerManager.PollProjectile(customProj,skillTarget, finalDamage, true);
    }

    protected override void DoUpgrade_one()
    {
        healPerTurn *= 1.5f;
    }

    protected override void DoUpgrade_two()
    {
    }

    protected override void DoUpgrade_three()
    {
        //인스 감속
    }

    protected override void DoUpgrade_four()
    {
        customProj.influenceRange = 1.75f;
        effectRange_visual = customProj.influenceRange;
        //브루들링 범위딜
    }
}
