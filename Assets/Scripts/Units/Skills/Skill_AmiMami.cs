using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_AmiMami : Skill
{
    /*

     -타격한 대상에 추가피해 디버프. 발동대상은 아미마미 상호 제한

     */
    //CACHE



    float effectTime ;
    Buff deshieldBuff;
    Buff FutamiBuff;
    ProjectileConfig myProjectileConfig;
    double futami_mod;

    public Skill_AmiMami(SkillConfig config) {
        SetInformation(config);
        effectTime = config.SuccessBuff_Time;
        futami_mod = config.SuccessBuff_Modifier;
    }
    protected override void Initialise_child()
    {
        Debug.Assert(towerComponent != null, "Tower comp null!");
        FutamiBuff = new Buff(BuffType.FUTAMI, futami_mod, effectTime, towerComponent, txt_skill_name);
        deshieldBuff = new Buff(BuffType.DEFENSE_MOD_PERC, -0.2d, effectTime, towerComponent, txt_skill_name);
    }

    public override bool ProcessAbility()
    {
        UpdateCustomBuffs();
        return true;
    }
    protected override void DoUpgrade_one()
    {
        if (myProjectileConfig == null)
        {
            myProjectileConfig = towerComponent.firepowerManager.myProjConfig;

        }

        FutamiBuff.buffAmount *= 2f;
        UpdateCustomBuffs();
    }
    protected override void DoUpgrade_two()
    {
        myProjectileConfig.ResetBuffs();
        UpdateCustomBuffs();
    }
    protected override void DoUpgrade_three()
    {
        FutamiBuff.buffAmount *= 2f;
        UpdateCustomBuffs();
    }
    protected override void DoUpgrade_four()
    {
        //Applied at Projectile.cs
    }

    public void UpdateCustomBuffs() {
        int upLevel = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, towerComponent.GetUID());
        myProjectileConfig = towerComponent.firepowerManager.myProjConfig;
        myProjectileConfig.ResetBuffs();
        myProjectileConfig.AddCustomBuff(FutamiBuff);

        if ( upLevel >= 2)
        {
            myProjectileConfig.AddCustomBuff(deshieldBuff);
        }

    }






}
