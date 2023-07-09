using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconBehaviour : MonoBehaviour
{
    [SerializeField] Text skillName;
    [SerializeField] Image skillIcon;
    [SerializeField] Image skillCooldown;
    [SerializeField] GameObject skillLock;
    [SerializeField] TextMeshProUGUI cooltime;

    [SerializeField] GameObject skillDescPanel;
    [SerializeField] Text skillDescText;

    
    Skill skill;


    internal void SetSkill(Skill _skill) {
        gameObject.SetActive(true);
        this.skill = _skill;
        skillName.text = LocalizationManager.Convert(skill.txt_skill_name);
       // Debug.Log(skillName.text);
        skillIcon.sprite = _skill.skill_icon;
        skillLock.SetActive(skill.isLocked);
        cooltime.text = "";
        switch (skill.skillType)
        {
            case SkillType.TIMED:
                skillCooldown.fillAmount = 1;
                break;
            case SkillType.PASSIVE_CONDITIONAL:
                skillCooldown.fillAmount = 1;
                break;
            case SkillType.PASSIVE_PERMANENT:
                skillCooldown.fillAmount = 0;
                break;
            case SkillType.ACTIVE:
                skillCooldown.fillAmount = 0;
                break;
        }
    }

    private void Update()
    {
        if (skill == null) return;
        if (skill.isLocked) return;
        switch (skill.skillType)
        {
            case SkillType.PASSIVE_CONDITIONAL:
                if (skill.IsActivated())
                {
                    cooltime.text = "!!!";
                    skillCooldown.fillAmount = 1;
                    skillCooldown.color = ConstantStrings.GetColorByHex("#c8777784");
                }
                else
                {

                    cooltime.text = "";
                    skillCooldown.fillAmount = 0;
                }
                break;
            case SkillType.PASSIVE_PERMANENT:
                break;
            case SkillType.ACTIVE:
                float stack = Math.Max(0, (float)Math.Round(skill.Active_CurrentStack() * 10f) / 10f);
                cooltime.text = (int)stack + " / " + (int)skill.Active_GetMaxStack();
 
                break;
            case SkillType.TIMED:
                skillCooldown.color = ConstantStrings.GetColorByHex("#84848484");
                float cooldown = Math.Max(0, (float)Math.Round(skill.GetCooldown() * 10f) / 10f);
                string fcstring = cooldown.ToString();
                if (cooldown % 1.0f == 0)
                {
                    fcstring += "0";
                }
                if (skill.IsActivated() && skill.skillType == SkillType.TIMED)
                {
                    cooltime.text = "!!!";
                    skillCooldown.color = ConstantStrings.GetColorByHex("#c8777784");
                }
                else
                {
                    cooltime.text = fcstring;
                }
                if (cooldown <= 0)
                { //Reached Max activations
                    cooltime.text = "";
                }
                skillCooldown.fillAmount = cooldown / skill.GetActivationFrequency();
                break;
        }
    }

    internal void ClearSkill()
    {
        skill = null;
        gameObject.SetActive(false);
    }



    public void ShowDescription(bool visible) {

        skillDescPanel.SetActive(visible);
        if (visible)
        {
            skillDescText.text = SkillConfig.GetDescription(skill.txt_skill_desc[skill.skillLevel], skill.skillLevel);
        }
    }
}
