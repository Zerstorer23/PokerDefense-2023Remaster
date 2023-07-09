using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //맨 아래 Instantiate추가 잊지말것
    public const string HARUKA_0 = "Haruka 0";
    public const string MIKI_0 = "Miki 0";
    public const string CHIHAYA_0 = "Chihaya 0";
    public const string CHIHAYA_1 = "Chihaya 1";
    public const string YAYOI_0 = "Yayoi 0";
    public const string YAYOI_1 = "Yayoi 1";
    public const string YUKIHO_0 = "Yukiho 0";
    public const string AZUSA_0 = "Azusa 0";
    public const string AZUSA_1 = "Azusa 1";
    public const string AZUSA_2 = "Azusa 2";
    public const string HIBIKI_0 = "Hibiki 0";
    public const string HIBIKI_1 = "Hibiki 1";
    public const string IORI_0 = "Iori 0";      
    public const string MAKOTO_0 = "Makoto 0";
    public const string MAKOTO_1 = "Makoto 1";
    public const string AMIMAMI_0 = "AmiMami 0";
    public const string TAKANE_0 = "Takane 0";


    [SerializeField] internal List<Skill> Skills_timed = new List<Skill>();
    [SerializeField] internal List<Skill> Skills_others = new List<Skill>();
    [SerializeField] internal List<Skill> Skills_All = new List<Skill>();
    Tower tower;
    GameSession session;
    private void Awake()
    {
     //   Debug.Log("skillmanager init start");
        session = GameSession.GetGameSession();
       // Debug.Log("skillmanager init end");
    }
    private Skill InstantiateSkill(SkillConfig config)
    {
        string skill_ID = config.skill_uid;
        switch (skill_ID)
        {
            case HARUKA_0:
                return new Skill_Haruka(config);
            case MIKI_0:
                return new Skill_Miki(config);
            case YAYOI_0:
                return new Skill_Yayoi(config);
            case YAYOI_1:
                return new Skill_Yayoi_Unicef(config);
            case CHIHAYA_0:
                return new Skill_Chihaya(config);
            case CHIHAYA_1:
                return new Skill_Chihaya_Promise(config);
            case TAKANE_0:
                return new Skill_Takane(config);
            case IORI_0:
                return new Skill_Iori(config);
            case MAKOTO_0:
                return new Skill_Makoto(config);
            case AMIMAMI_0:
                return new Skill_AmiMami(config);
         /*   case AZUSA_0:
                return new Skill_Azusa_Slow(config);*/
            case AZUSA_1:
                return new Skill_Azusa_Kill(config);
            case AZUSA_2:
                return new Skill_Azusa_HealBlock(config);
            case HIBIKI_0:
                return new Skill_Hibiki(config);
            case HIBIKI_1:
                return new Skill_Hibiki_Solo(config);
            case YUKIHO_0:
                return new Skill_Yukiho(config);
            default:
                return null;
        }
    }
    void Update()
    {
        CheckSkillActivations();
    }

    public void InitialiseSkills(List<SkillConfig> skillConfigs, GameObject caster)
    {
        tower = GetComponent<Tower>();

        foreach (SkillConfig config in skillConfigs)
        {
            Skill skill = InstantiateSkill(config);
            skill.SetCaster(caster);
            skill.Initialise();
            if (skill.skillType == SkillType.ACTIVE) {
                if (tower.owner == Owner.NAMCO && !skill.isLocked)
                {
                    EventManager.TriggerEvent(MyEvents.EVENT_ADD_ACTIVE_SKILL, new EventObject(skill));
                }
                else if (tower.owner == Owner.KUROI) {
                    Skills_timed.Add(skill);
                }
            }
            if (skill.skillType == SkillType.PASSIVE_PERMANENT)
            {
             //   Debug.Log("Activate permanent skill " + skill.GetName());
                skill.Activate();
                Skills_others.Add(skill);
            }
            else if (skill.skillType == SkillType.PASSIVE_CONDITIONAL) {
                Skills_others.Add(skill);
            }
            else if (skill.skillType == SkillType.TIMED){
             //   Debug.Log("Add timed skill " + skill.GetName());
                Skills_timed.Add(skill);
            }
            Skills_All.Add(skill);
        }
    }

    public void ReinitialiseActiveSkills()
    {
        tower = GetComponent<Tower>();
        foreach (Skill skill in Skills_All)
        {

            if (skill.skillType == SkillType.ACTIVE)
            {
                if (tower.owner == Owner.NAMCO && !skill.isLocked)
                {
                    EventManager.TriggerEvent(MyEvents.EVENT_ADD_ACTIVE_SKILL, new EventObject(skill));
                }
            }
        }
    }
    public void CheckSkillActivations()
    {
        if (GetComponent<Tower>().IsInKnockBack()) return;
        for (int i = 0; i < Skills_timed.Count; i++)
        {
            Skill skill = Skills_timed[i];
            if (skill.isLocked) continue;
            //Only kuroi has active here
            if (skill.skillType == SkillType.ACTIVE) {
                if(!CheckKuroiSkillValid(skill))
                        continue;
                   // Debug.Log("Kuroi activates " + skill.txt_skill_name+" by "+tower.GetUID());
                   skill.Activate();
            }else if (Time.time >= skill.GetActivatedTIme() + skill.GetActivationFrequency())
            {
                skill.Activate();
            }
        }
    }
    bool CheckKuroiSkillValid(Skill skill) {
        if (!session.waveManager.IsInWave() || Time.time < session.waveManager.GetStartTime() + 3f) return false;
        if (skill.NotForKuroi) return false;
        if (skill.Active_CurrentStack() < 1) return false;
        return true;
    }

    public List<Skill> GetSkillList() { return Skills_All; }

    public bool HasSKill(string skillID)
    {
        foreach (Skill sk in Skills_All) {
            if (sk.GetName().Equals(skillID)) return true;
        }
        return false;
    }

    internal void DoUnitDestroyed()
    {
        foreach (Skill sk in Skills_All)
        {
            sk.OnDestroy();
        }
    }
}
