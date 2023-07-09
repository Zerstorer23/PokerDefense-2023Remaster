using UnityEngine;
public class Skill_Chihaya : Skill
{
    /*
         투사체: 음표 / 마이크
        -n초마다 n% 확률로 특수탄 발사 (트럭, 일자형 스플래시)
         발현후 넉백 n초
     */
    //CACHE




    float knockBackTime ;
    bool doKnockback = true;

    public Skill_Chihaya(SkillConfig config)
    {
        SetInformation(config);
        knockBackTime = config.SuccessBuff_Time;
        customProj = GameObject.Instantiate(config.projectile_config);
    }


    public override bool ProcessAbility()
    {
        if (towerComponent.firepowerManager.GetFocusedTarget())
        {
            FireTruck();
            if (doKnockback) {
                Buff knockback = new Buff(BuffType.KNOCKBACK, knockBackTime, towerComponent, txt_skill_name);
                towerComponent.AddBuff(knockback);
            }
            RequestPostponeTime(knockBackTime);
            towerComponent.DoCoroutine(SetActivationStatus(knockBackTime));
            return true;
        }
        else {
            return false;
        }
    }
    public void FireTruck()
    {
        double finalDamage = towerComponent.GetFinalAttackDamage() * customProj.splashModifier;
        towerComponent.firepowerManager.PollProjectile(customProj, towerComponent.firepowerManager.GetFocusedTarget(), finalDamage, false);
    }

    protected override void DoUpgrade_one()
    {
        activationFrequency /= 2f;
    }

    protected override void DoUpgrade_two()
    {
        customProj.splashModifier *= 1.5f;
    }

    protected override void DoUpgrade_three()
    {
        customProj.influenceRange *=2f;
    }

    protected override void DoUpgrade_four()
    {
        doKnockback = false;
    }
}
