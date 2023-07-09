using UnityEngine;
using UnityEngine.UI;

public class LessonPanel_SkillDisplay : MonoBehaviour
{
    [SerializeField] Text skillDesc;
    [SerializeField] Text skillNameText;
    [SerializeField] Image skillImage;

    public void SetDisplayInfo(SkillConfig config, int skillLevel)
    {
        if (config == null)
        {
            gameObject.SetActive(false);

        }
        else
        {
            gameObject.SetActive(true);
            skillImage.sprite = config.skill_icon;
            string key = config.skill_desc_key[skillLevel];
            skillDesc.text = SkillConfig.GetDescription(key, skillLevel);
            skillNameText.text = LocalizationManager.Convert(config.skill_name_key);



        }
    }
}
