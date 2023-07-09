using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Makoto : Skill
{
    /*
     투사체: 즉시데미지

     -n초마다 n%확률로 광전사모드
     -광전사시 스플뎀, 빠른 연사속도
         //연사버프, 연사마다 스플뎀 투사체 발사
         or change protectile prefab

     */
    //CACHE


    
  
    float effectiveTime;
    float probability ;
    bool doAttackSpeedMod = false;
    bool doAttackPercMod = false;
    float attackMod = 1f;

    ProjectileConfig originalProj, burningProj;

    public Skill_Makoto(SkillConfig config)
    {

        SetInformation(config);

        effectiveTime = config.SuccessBuff_Time;
        probability = config.SuccessPercentage;
        originalProj = GameObject.Instantiate(config.projectile_config);
        burningProj = GameObject.Instantiate(config.burnig_projectile_config);

    }
    private bool RollDice()
    {
        float rand = Random.Range(0f, 1f);
        return rand < probability;
    }

    public override bool ProcessAbility()
    {
        Tower tower = caster.GetComponent<Tower>();
        if (RollDice())
        {
            ProjectileConfig original = originalProj;
            ProjectileConfig burning = burningProj;
            Buff projBuff = new Buff(BuffType.CHANGE_PROJECTILE, effectiveTime, towerComponent, txt_skill_name)
            {
                originalProjectile = original,
                burningProjectile = burning
            };
            tower.AddBuff(projBuff);
            if (doAttackSpeedMod) {
                ApplyAttackSpeedBuff();
            }
            if (doAttackPercMod) {
                ApplyAttackPercBuff();
            }


            tower.DoCoroutine(SetActivationStatus(effectiveTime));
            RequestPostponeTime(effectiveTime);
        }
        

        return true;
    }
    protected override void OnDestroy_child()
    {
        GameObject.Destroy(originalProj);
        GameObject.Destroy(burningProj);
    }
    private void ApplyAttackSpeedBuff()
    {
        Buff buff = new Buff(BuffType.ATTACK_SPEED, -0.5f, effectiveTime, towerComponent, txt_skill_name);
        towerComponent.AddBuff(buff);
    }
    private void ApplyAttackPercBuff()
    {
        Buff buff = new Buff(BuffType.ATTACK_PERC, attackMod, effectiveTime, towerComponent, txt_skill_name);
        towerComponent.AddBuff(buff);
    }
    protected override void DoUpgrade_one()
    {
        probability = 1f;
    }

    protected override void DoUpgrade_two()
    {
        burningProj.splashModifier =1f;
    }

    protected override void DoUpgrade_three()
    {
        doAttackSpeedMod = true;
    }

    protected override void DoUpgrade_four()
    {
        doAttackPercMod = true;
        //Harms original prefab.
    }
}
