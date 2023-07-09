using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Miki : Skill
{
   /*
    미키: 게임 시작후 3초간 공격 불가
    
    */
    //CACHE



    float effectTime = 3f;

    public Skill_Miki(SkillConfig config)
    {
        SetInformation(config);
        effectTime = config.SuccessBuff_Time;
    }
    protected override void Initialise_child()
    {
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, ActivateSkill);
    }

   
    protected override void DoUpgrade_one()
    {
        isLocked = true;
    }
    protected override void DoUpgrade_two()
    {//범위 1.5q배
        towerComponent.firepowerManager.myProjConfig.influenceRange *= 1.5f;
    }
    protected override void DoUpgrade_three()
    {
        //사거리 2배
        towerComponent.buffManager.attackDelayModifier -= 0.25f;
    }
    protected override void DoUpgrade_four()
    {
        //공속 25%증가
        towerComponent.buffManager.attackDelayModifier -= 0.25f;
    }

    protected override void OnDestroy_child()
    {
        EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, ActivateSkill);
    }

    public override bool ProcessAbility()
    {
        return false;
    }
    void ActivateSkill(EventObject eo)
    {
        if (isLocked) return;
        Buff knockback = new Buff(BuffType.KNOCKBACK, effectTime, towerComponent, txt_skill_name);
        towerComponent.AddBuff(knockback);
        towerComponent.DoCoroutine(SetActivationStatus(effectTime));
    }

}
