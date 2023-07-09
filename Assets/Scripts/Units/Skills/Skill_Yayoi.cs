using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Yayoi : Skill
{
   /*
    미키: 게임 시작후 3초간 공격 불가
    
    */
    //CACHE



    double buffModifier;
    float effectiveTime ;
    Buff yayoiBuff;
    ProjectileConfig myProjConfig;



    public Skill_Yayoi(SkillConfig config)
    {
        SetInformation(config);

        buffModifier = config.SuccessBuff_Modifier;
        effectiveTime = config.SuccessBuff_Time;

    }

    public override bool ProcessAbility()
    {
        UpdateCustomBuffs();
        return true;
    }

    protected override void DoUpgrade_one()
    {
        buffModifier *= 2f;
        UpdateCustomBuffs();
    }
    protected override void DoUpgrade_two()
    {
        //Bounce
        myProjConfig = towerComponent.firepowerManager.myProjConfig;
        myProjConfig.numBounce = 1;
        myProjConfig.bounceRange = 5f;

    }
    protected override void DoUpgrade_three()
    {
        myProjConfig = towerComponent.firepowerManager.myProjConfig;
        myProjConfig.numBounce = 2;
        myProjConfig.bounceRange = 5f;
    }
    protected override void DoUpgrade_four()
    {

    }
    public void UpdateCustomBuffs()
    {
        myProjConfig = towerComponent.firepowerManager.myProjConfig;
        myProjConfig.ResetBuffs();
        yayoiBuff = new Buff(BuffType.GOLD_BONUS , buffModifier, effectiveTime,towerComponent, txt_skill_name);
        myProjConfig.AddCustomBuff(yayoiBuff);
    }





}
