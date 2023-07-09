using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffIndicator : MonoBehaviour
{

    [SerializeField] SpriteRenderer[] buffSprites;
    [SerializeField] GameObject superBuff;
    [SerializeField] GameObject stunBuff;
    [SerializeField] GameObject kuroiMarker;

    Dictionary<BuffType, int> currentBuffs = new Dictionary<BuffType, int>();
    private void OnEnable()
    {
        currentBuffs = new Dictionary<BuffType, int>();
        UpdateUI();
    }
    public void AddBuffIndicator(BuffType bufftype) {
        if (bufftype == BuffType.ATTACK) {
            bufftype = BuffType.ATTACK_PERC;
        }

        if (!currentBuffs.ContainsKey(bufftype))
        {
            currentBuffs.Add(bufftype, 1);
        }
        else {
            currentBuffs[bufftype]++;
        }
        UpdateUI();
    }
    public void RemoveBuffIndicator(BuffType bufftype)
    {
        if (bufftype == BuffType.ATTACK)
        {
            bufftype = BuffType.ATTACK_PERC;
        }
        if (!currentBuffs.ContainsKey(bufftype)) return;
        Debug.Assert(currentBuffs.ContainsKey(bufftype), "없는 버프를 지우노.,," + bufftype);

        currentBuffs[bufftype]--;
        UpdateUI();
    }


    private void UpdateUI() {
        int 색인 = 0;
        bool doStun = false;

        foreach (KeyValuePair<BuffType, int> entry in currentBuffs)
        {
            if (entry.Value <= 0) continue;
            if (색인 >= 3) break;
            if (entry.Key == BuffType.KNOCKBACK)
            {
                doStun = true;
            }
            else {
                buffSprites[색인].sprite = Buff.GetBuffImage(entry.Key); 
                buffSprites[색인].enabled = true;
                색인++;
            }
        }

        for (; 색인 < 3; 색인++) {
            buffSprites[색인].enabled = false;
        }
        stunBuff.SetActive(doStun);

    }
    public void TriggerSuperbuff(bool enable)
    {
        superBuff.SetActive(enable);
    }
    public void MarkAsKuroi() {
        kuroiMarker.SetActive(true);
    
    }
}
