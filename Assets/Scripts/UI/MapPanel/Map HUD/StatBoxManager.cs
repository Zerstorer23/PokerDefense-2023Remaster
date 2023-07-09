using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBoxManager : MonoBehaviour
{
    [SerializeField] Text[] statNames;
    [SerializeField] Text[] statVals;

    string[] tower_stat_names = { "TXT_KEY_ATTACK", "TXT_KEY_ATTACK_SPEED", "TXT_KEY_ATTACK_RANGE" };
    string[] enemy_stat_names = { "TXT_KEY_HP", "TXT_KEY_DEFENSE", "TXT_KEY_STAT_MOVE_SPEED" };
    bool isShowingTower = true;
    bool isDisplayOff = false;

    public void SetTowerStatDisplay(Tower tower)
    {
        if (!isShowingTower || isDisplayOff)
        {
            SetStatNameFields(tower_stat_names);
            isShowingTower = true;
        }
        int attack = (int)tower.GetFinalAttackDamage(), delay = (int)tower.GetFinalAttackDelay(), range = (int)tower.GetFinalAttackRange();

        statVals[0].text = attack.ToString();
        statVals[1].text = delay.ToString();
        statVals[2].text = range.ToString();

    }


    public void SetEnemyStatDisplay(Enemy_Main enemy)
    {
        if (isShowingTower || isDisplayOff)
        {
            SetStatNameFields(enemy_stat_names);
            isShowingTower = false;
        }
        int hp = (int)enemy.GetComponent<HealthPoint>().GetHP();
        int defense = (int)enemy.GetComponent<HealthPoint>().GetFinalDefense();
        int speed = (int)enemy.GetComponent<Enemy_PathFind>().GetMoveSpeed();
        statVals[0].text = hp.ToString();
        statVals[1].text = defense.ToString();
        statVals[2].text = speed.ToString();
    }
    public void HideNameFields()
    {

        for (int i = 0; i < statNames.Length; i++)
        {
            statNames[i].enabled = false;
            statVals[i].enabled = false;
        }
        isDisplayOff = true;
    }


    void SetStatNameFields(string[] fieldNames)
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            bool enable = (i < fieldNames.Length);
            if (i < fieldNames.Length)
            {
                statNames[i].text = LocalizationManager.Convert( fieldNames[i]);
            }
            statNames[i].enabled = enable;
            statVals[i].enabled = enable;

        }
        isDisplayOff = false;

    }




}
