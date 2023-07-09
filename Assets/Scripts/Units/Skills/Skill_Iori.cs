using UnityEngine;

[System.Serializable]
public class Skill_Iori : Skill
{
    /*
     * 이오리
	투사체: 토끼

1- 야마토포 x호마다 x의 데미지 투사체
          	넉백 없음


     */
    //CACHE


    bool auto = false;



    public Skill_Iori(SkillConfig config) {
        SetInformation(config);
        customProj = GameObject.Instantiate(config.projectile_config);
        canToggleAuto = auto;
    }

    public override bool ProcessAbility()
    {
        if (towerComponent.firepowerManager.GetFocusedTarget() != null)
        {
          //  Debug.Log(txt_skill_name + " fired to " + towerComponent.GetFocusedTarget().transform.position);
            Fire_Skill();
            return true;
        }
        else {
            return false;
        }
    }

    protected override void DoUpgrade_one()
    {
        activationFrequency /= 2f;
    }




    protected override void DoUpgrade_two()
    {
        Buff buff = new Buff(BuffType.ATTACK, 5000f, -1f, towerComponent, txt_skill_name);
        towerComponent.buffManager.AddBuff(buff);
    }
    protected override void DoUpgrade_three()
    {
        Buff buff = new Buff(BuffType.ATTACK, 5000f, -1f, towerComponent, txt_skill_name);
        towerComponent.buffManager.AddBuff(buff);
    }

    protected override void DoUpgrade_four()
    {
        customProj.SetSkillDamageModifier(6f);
    }

    void Fire_Skill()
    {
        double finalDamage = towerComponent.GetFinalAttackDamage() 
            * customProj.GetSkillDamageModifier();
            towerComponent.firepowerManager.PollProjectile(customProj, towerComponent.firepowerManager.GetFocusedTarget(), finalDamage, false);
    }

}
