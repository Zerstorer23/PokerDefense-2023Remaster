using UnityEngine;

[System.Serializable]
public class Skill_Hibiki : Skill
{
    /*
     히비키 5초마다 동물생산
    동물은 히비키 데미지 계승
    히비키가 타격하는 대상 동시 타격
    게임시작시 동물 다디짐

     */
    //CACHE

    int numUnderlings = 0;
    int underling_limit = 1;
    bool waveStarted = false;
    float underlingDamageMod = 0.5f;
    public Skill_Hibiki(SkillConfig config)
    {
        SetInformation(config);
    }
    protected override void Initialise_child()
    {
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_FINISHED, KillAllUnderlings);
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED,ResetActivationTIme);
    }

    private void ResetActivationTIme(EventObject arg0)
    {
        activatedTime = Time.time;
        waveStarted = true;
    }

    public override bool ProcessAbility()
    {
        if (!waveStarted) return false;
        if (numUnderlings >= underling_limit)
        {
            isActivated = true;
            return true;
        }
        SpawnUnderling();
        return true;
    }

    private void SpawnUnderling()
    {
        int index = RollDice();
        UnitConfig underling = towerComponent.activeSkillManager.GetHibikiUnderlingConfig(index);
        towerComponent.SpawnUnderling(underling,underlingDamageMod);
        numUnderlings++;
        if (numUnderlings == 3) isActivated = true;
    }

    void KillAllUnderlings(EventObject eo) {
        //Kill All
        towerComponent.KillAllUnderling();
        Reset();
    }
    void Reset()
    {
        numUnderlings = 0;
        isActivated = false;
        waveStarted = false;

    }

    int RollDice() {
        if (underling_limit == 1) return 0;
        return Random.Range(0, 3);
    }

    protected override void DoUpgrade_one()
    {
        underlingDamageMod = 1f;
    }

    protected override void DoUpgrade_two()
    {
        underling_limit = 2;
    }

    protected override void DoUpgrade_three()
    {
        underling_limit = 3;
    }

    protected override void DoUpgrade_four()
    {
        //Related to solo skill
    }
}
