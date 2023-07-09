using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Takane : Skill
{
    //   패시브 게임시작후 3초간 데미지 상승 버프
    //  이후 100%까지 점차적 감소
    //CACHE





    bool auto = false;
   [SerializeField] internal int one_storm_ticks = 3; // 3
    [SerializeField] internal int storm_ticks = 10;
    [SerializeField] internal float storm_delay = 0.5f; // 0.5f
    [SerializeField] internal float range = 1.5f; // 스킬 산개 범위


    float underlingDamageMod = 1f;
    public string objTag = "TAKANE_STORM";

    //스플뎀 범위와 계수는 proj로 설정
    public Skill_Takane(SkillConfig config)
    {
        SetInformation(config);

        canToggleAuto = auto;
        customProj = GameObject.Instantiate(config.projectile_config);
        isTargetLand = true;
        SetEffecRadius(range);
    }

    public override bool ProcessAbility()
    {
        bool possible = ActiveSkill_HasTarget();
        if (possible)
        {
            SpawnUnit(targetLocation);
        }
        return possible;
    }


    public void SpawnUnit(Vector3 targetLocation)
    {
        GameObject Lexington = ObjectPool.PollObject(objTag, targetLocation,Quaternion.identity);
        if (Lexington == null) {
            GameObject underlingBase = towerComponent.activeSkillManager.GetUnderlingPrefab();
            Lexington = GameObject.Instantiate(underlingBase, targetLocation, Quaternion.identity);
        }
        Lexington.transform.SetParent(GameSession.GetGameSession().ProjectileHome);
        Lexington.GetComponent<Tower_Underling>().SetDamageModifier(underlingDamageMod);
        Lexington.GetComponent<Tower_Underling>().SetPsyonicStorm(caster,this,customProj);
    }

    protected override void DoUpgrade_one()
    {
        //데미지
        underlingDamageMod *= 2f;
    }

    protected override void DoUpgrade_two()
    {
        //걍 쎄짐
        one_storm_ticks *= 2;
    }

    protected override void DoUpgrade_three()
    {
        //지속시간 및 더 빠르게
         storm_ticks *= 2;
         storm_delay /= 2f;
        SetEffecRadius(range + 1f);
    }

    protected override void DoUpgrade_four()
    {
        //2배딜
        underlingDamageMod *= 2f;
    }



    void SetEffecRadius(float newRange) {
        range = newRange;
        effectRange_visual = range;
    }
}
