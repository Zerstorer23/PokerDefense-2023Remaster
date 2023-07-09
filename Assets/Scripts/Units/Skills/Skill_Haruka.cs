using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;

[System.Serializable]
public class Skill_Haruka : Skill
{
    //   	-5초마다 50%확률로 5초간 주변 1타일에 공격력 버프 10% 
    //	-실패시 넉백
    //CACHE



    float successPercentage; //1.000 확률로
    float influenceRange;


    double attackPercBonus ; //25%화력보너스
    float effectiveTime ; //5초간 지속

   float failKnockBackTime; //n초 넉백

    bool doSkillBuff = false;
    double skillDamageBonus = 0.24f;

    bool doBuffBasedOnDamage = false;
    double attackBaseMod = 0.12f; // 공격력의 n%를 버프에 추가

    public Skill_Haruka(SkillConfig config) {
        SetInformation(config);
        successPercentage = config.SuccessPercentage;
        influenceRange = config.SuccessBuff_InfluenceRange + 0.25f;
        attackPercBonus = config.SuccessBuff_Modifier;
        effectiveTime = config.SuccessBuff_Time;
        failKnockBackTime = config.FailBuff_Time;
    }

    public override bool ProcessAbility()
    {
        if (RollDice())
        {
            Buff attackBuff = new Buff(BuffType.ATTACK_PERC, attackPercBonus, effectiveTime, towerComponent, txt_skill_name);
            AddRangeBuff(caster, attackBuff,towerComponent.GetTowerSpawner());
            if (doSkillBuff)
            {
                Buff actBuff = new Buff(BuffType.SKILL_DAMAGE_MOD, skillDamageBonus, effectiveTime+activationFrequency, towerComponent, txt_skill_name);
                AddRangeBuff(caster, actBuff, towerComponent.GetTowerSpawner());
            }
            if (doBuffBasedOnDamage) {
                double myDamage = towerComponent.firepowerManager.GetRawDamage();
                Buff damageBasedBuff = new Buff(BuffType.ATTACK, myDamage * attackBaseMod, effectiveTime, towerComponent, txt_skill_name);
                AddRangeBuff(caster, damageBasedBuff, towerComponent.GetTowerSpawner());


            }

            RequestPostponeTime(effectiveTime);
            towerComponent.DoCoroutine(SetActivationStatus(effectiveTime));
        }
        else {
            Buff knockback = new Buff(BuffType.KNOCKBACK, failKnockBackTime, towerComponent, txt_skill_name);
            towerComponent.AddBuff(knockback);
            RequestPostponeTime(failKnockBackTime);
            towerComponent.DoCoroutine(SetActivationStatus(failKnockBackTime));
        }
        return true;
    }

    private bool RollDice() {
        float rand = Random.Range(0f, 1f);
        return rand < successPercentage;
    }

    private void AddRangeBuff(GameObject caster, Buff buff,TowerSpawner towerSpawner) {
       
        var myTowers = towerSpawner.GetMyTowers().Values;
      //  Debug.Log("Num tower "+myTowers.Count);
        foreach (Tower tower in myTowers) {
            if (!tower.gameObject.activeSelf) continue;

            double dist = GameSession.GetTileDistance(caster.transform.position, tower.transform.position);
        //    Debug.Log(tower.transform.position + " = distance: " +dist);
            if (dist<= influenceRange) {
          //      Debug.Log(tower.transform.position+" = adding buff...");
                tower.AddBuff(buff);
            }
        }
    }



    protected override void DoUpgrade_one()
    {//화력버프량 상승
        attackPercBonus *= 2f; 
    }

    protected override void DoUpgrade_two()
    {
        successPercentage = 1f;
    }

    protected override void DoUpgrade_three()
    {
        attackPercBonus = 2f;
    }

    protected override void DoUpgrade_four()
    {
        doBuffBasedOnDamage = true;
       
    }

}
