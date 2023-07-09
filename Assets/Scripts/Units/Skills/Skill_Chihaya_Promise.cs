
public class Skill_Chihaya_Promise : Skill
{
  
    public Skill_Chihaya_Promise(SkillConfig config)
    {
        SetInformation(config);
        isLocked = true;
    }


    public override bool ProcessAbility()
    {
        return true;
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
    }
}
