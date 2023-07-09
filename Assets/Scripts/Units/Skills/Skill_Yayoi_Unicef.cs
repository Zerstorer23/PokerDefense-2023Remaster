using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill_Yayoi_Unicef : Skill
{
    /*
     미키: 게임 시작후 3초간 공격 불가

     */
    //CACHE



    float influenceRange;
    double attackPercMod;
    

    List<Tower> towersResponsible = new List<Tower>();
    double totalAttack = 0;
    Buff unicef;


    public Skill_Yayoi_Unicef(SkillConfig config)
    {
        SetInformation(config);
        isLocked = true;
        influenceRange = config.SuccessBuff_InfluenceRange + 0.2f;
        attackPercMod = config.SuccessBuff_Modifier;

    }
    protected override void Initialise_child()
    {
        EventManager.StartListening(MyEvents.EVENT_TOWER_PLACED, ActivateSkill);
        EventManager.StartListening(MyEvents.EVENT_TOWER_DISABLED, OnTowerReserved);

        unicef = new Buff(BuffType.ATTACK_PERC_LOW, attackPercMod, -1, towerComponent, txt_skill_name);
        unicef.SetMutexID(skill_uid);
    }

    private void OnTowerReserved(EventObject eo)
    {
        if (isLocked) return;
        Vector3 reservedPosition = eo.GetVector();
        Vector3 myPos = caster.transform.position;
        if (reservedPosition == myPos)
        {
            RemoveBuffs();
        }
        else {
            DebuffNearUnits(myPos);
        }

    }

    public override bool ProcessAbility()
    {
    
        return true;
    }

    protected override void OnDestroy_child()
    {
        EventManager.StopListening(MyEvents.EVENT_TOWER_PLACED, ActivateSkill);
        EventManager.StopListening(MyEvents.EVENT_TOWER_DISABLED, OnTowerReserved);
        RemoveBuffs();
    }
    
    private void ActivateSkill(EventObject eo)
    {
        if (isLocked) return;
        Vector3 myPos = caster.transform.position;
        Vector3 newPos = eo.GetGameObject().transform.position;

        if (newPos == myPos)
        {
            DebuffNearUnits(myPos);
        }
        else
        {
            Tower targetTower = eo.GetGameObject().GetComponent<Tower>();
            if (targetTower.GetUID().Equals(towerComponent.GetUID())) return;
            bool hasBuff = targetTower.buffManager.HasBuff(skill_uid);
            if (hasBuff)
            {
                //이미 버프가 있는지 확인
                bool isInRange = (GameSession.GetTileDistance(newPos, myPos) <= influenceRange);
                //새포지션이 범위내
                if (!isInRange)
                {
                    DebuffNearUnits(myPos);
                }
            }
            else
            {
                //없음
                //이미 버프가 있는지 확인
                bool isInRange = (GameSession.GetTileDistance(newPos, myPos) <= influenceRange);
                //새포지션이 범위내
                if (isInRange)
                {
                    //범위밖
                    PutBuff(targetTower);
                    CalculateYayoiEarn();
                }

            }


        }
    }

    private void PutBuff(Tower target)
    {
        target.buffManager.AddBuff_Mutex(unicef, skill_uid);
        towersResponsible.Add(target);
    }

    private void RemoveBuffs()
    {
       // towerComponent.buffManager.RemoveBuff_Mutex(skill_uid);
        while (towersResponsible.Count > 0)
        {
            Tower towerObj = towersResponsible[0];
            if (towerObj == null) continue;
            towerObj.buffManager.RemoveBuff_Mutex(skill_uid);
            towersResponsible.RemoveAt(0);
        }

    }
    private void DebuffNearUnits(Vector3 myPos)
    {
        RemoveBuffs();
        var myTowers = towerComponent.GetTowerSpawner().GetMyTowers().Values;
        foreach (Tower tower in myTowers)
        {
            if (!tower.gameObject.activeSelf) continue;

            double dist = GameSession.GetTileDistance(myPos, tower.transform.position);
            if (dist <= influenceRange && dist != 0 && !tower.GetUID().Equals(towerComponent.GetUID()))
            {
                PutBuff(tower);
            }
        }
        CalculateYayoiEarn();
    }
    private void CalculateYayoiEarn()
    {

        towerComponent.buffManager.RemoveBuff_Mutex(skill_uid);
        totalAttack = 0f;
        if (towersResponsible.Count > 0) {
            foreach (Tower sacrifice in towersResponsible)
            {
                totalAttack += sacrifice.firepowerManager.GetRawDamage() * 10/(1-attackPercMod);
            }
            Buff buff = new Buff(BuffType.ATTACK, totalAttack * attackPercMod, -1f, towerComponent, txt_skill_name);
            buff.SetMutexID(skill_uid);
            towerComponent.buffManager.AddBuff_Mutex(buff, skill_uid);

        }



    }

    protected override void DoUpgrade_one()
    {
    }
    protected override void DoUpgrade_two()
    {

    }
    protected override void DoUpgrade_three()
    {
    }
    protected override void DoUpgrade_four()
    {
        isLocked = false;
        if(unicef != null)
        ActivateSkill(new EventObject(caster));
    }


 


}
