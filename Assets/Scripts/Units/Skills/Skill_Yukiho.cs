using System;
using UnityEngine;

[System.Serializable]
public class Skill_Yukiho : Skill
{
    /*
     * 유키호
     투사체: 삽
     긴 사거리 긴 딜레이

    액티브 : 선택한 적 락다운
    토글 액티브 30초마다 범위 내 가장 체력이 높은 적포박 스킬

     */
    //CACHE


    bool doDeshield = false;


    Buff deshieldBuff;

    float baseEffectTime;
    public static int addPerVocal = 8;


    public Skill_Yukiho(SkillConfig config) {
        SetInformation(config);
        canToggleAuto = false;
        canTargetTower = true;
        baseEffectTime = config.SuccessBuff_Time;
        customProj = GameObject.Instantiate(config.projectile_config);
    }



    public override bool ProcessAbility()
    {
        bool possible = ActiveSkill_HasTarget(baseEffectTime);
        if (possible)
        {
            Fire_Skill();
        }
        return possible;
    }

    


    protected override void DoUpgrade_one()
    {
        //   doKnockBack = false;
        addPerVocal = 3;
    }
    protected override void DoUpgrade_two()
    {
        healPerTurn *= 1.5f;
    }
    protected override void DoUpgrade_three()
    {
        deshieldBuff = new Buff(BuffType.DEFENSE_MOD_PERC, -1f, baseEffectTime, towerComponent, txt_skill_name);
        doDeshield = true;
    }

 
    protected override void DoUpgrade_four()
    {
        canToggleAuto = true;
        EventManager.TriggerEvent(MyEvents.EVENT_ACTIVESKILL_TOGGLE_POSSIBLE, new EventObject(skillSlot));
    }

    void Fire_Skill()
    {
        customProj.ResetBuffs();
        float effectTime = baseEffectTime + UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, towerComponent.GetUID()) / addPerVocal;
        Buff knockback = new Buff(BuffType.KNOCKBACK, effectTime, towerComponent, txt_skill_name);
        customProj.AddCustomBuff(knockback);
        if (doDeshield)
        {
            customProj.AddCustomBuff(deshieldBuff);
        }

        double finalDamage = towerComponent.GetFinalAttackDamage();
        towerComponent.firepowerManager.PollProjectile(customProj, skillTarget, finalDamage, false);

        skillTarget = null;
    }
}
