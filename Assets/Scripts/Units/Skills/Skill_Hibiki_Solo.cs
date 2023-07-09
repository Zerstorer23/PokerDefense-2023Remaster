
using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;

[System.Serializable]
public class Skill_Hibiki_Solo : Skill
{
    /*
     히비키 5초마다 동물생산
    동물은 히비키 데미지 계승
    15초 지속
    히비키가 타격하는 대상 동시 타격

     */
    //CACHE



    float influenceRange;
    double attackPercMod;


    Queue<Tower> towersResponsible = new Queue<Tower>();


    public Skill_Hibiki_Solo(SkillConfig config)
    {
        SetInformation(config);
        influenceRange = config.SuccessBuff_InfluenceRange + 0.2f;
        attackPercMod = config.SuccessBuff_Modifier;
    }
    protected override void Initialise_child()
    {
        EventManager.StartListening(MyEvents.EVENT_TOWER_PLACED, ActivateSkill);
        EventManager.StartListening(MyEvents.EVENT_TOWER_DISABLED, OnTowerReserved);
    }
    public override bool ProcessAbility()
    {
        return false;
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

        Buff buff = new Buff(BuffType.ATTACK_PERC_LOW, attackPercMod, -1, towerComponent, txt_skill_name);
        buff.SetMutexID(skill_uid);

        if (newPos == myPos)
        {
            DebuffNearUnits(buff,myPos);
        }
        else {
            Tower targetTower = eo.GetGameObject().GetComponent<Tower>();
            bool hasBuff = targetTower.buffManager.HasBuff(skill_uid);
            if (hasBuff)
            {
                //이미 버프가 있는지 확인
                bool isInRange = (GameSession.GetTileDistance(newPos, myPos) <= influenceRange);
                //새포지션이 범위내
                if (!isInRange)
                {
                    DebuffNearUnits(buff, myPos);
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
                    PutBuff(buff,targetTower);
                }

            }

      
        }
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
        else
        {
            Buff buff = new Buff(BuffType.ATTACK_PERC_LOW, attackPercMod, -1, towerComponent, txt_skill_name);
            buff.SetMutexID(skill_uid);
            DebuffNearUnits(buff, myPos);
        }

    }

    private void PutBuff(Buff buff, Tower target) {
        target.buffManager.AddBuff_Mutex(buff, skill_uid);
        towersResponsible.Enqueue(target);
    }

    private void RemoveBuffs() {
        while (towersResponsible.Count > 0) {
            Tower towerObj = towersResponsible.Dequeue();
            if (towerObj == null) continue;
            towerObj.buffManager.RemoveBuff_Mutex(skill_uid);
        }
    
    }
    private void DebuffNearUnits(Buff buff, Vector3 myPos)
    {
        RemoveBuffs();
        var myTowers = towerComponent.GetTowerSpawner().GetMyTowers().Values;
        foreach (Tower tower in myTowers)
        {
            if (!tower.gameObject.activeSelf) continue;

            double dist = GameSession.GetTileDistance(myPos, tower.transform.position);
            if (dist <= influenceRange && dist != 0)// && !targetTower.GetUID().Equals(towerComponent.GetUID()))
            {
                PutBuff(buff, tower);
            }
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

        RemoveBuffs();
        isLocked = true;
    }
}
