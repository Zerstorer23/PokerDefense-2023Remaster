using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static ConstantStrings;
using UnityEngine.UI;
using System;

public class HealthPoint : MonoBehaviour
{
    BuffManager buffManager;
    [SerializeField] GameObject damageIndicator;
    public UnitType unitType;
    [SerializeField] Slider hpSlider;

    Enemy_Main enemyMain;

    internal double defensePoint;
    internal double fullHP = 0;
    internal double healthPoint;
    public bool invincible = false;
    private bool isDead = false;

    private void Awake()
    {
        //   Debug.Log("hp init srtart");
        buffManager = GetComponent<BuffManager>();
        if (unitType == UnitType.Tower)
        {
        }
        else
        {
            enemyMain = GetComponent<Enemy_Main>();
        }
        //  Debug.Log("hp init end");

    }
    private void OnEnable()
    {
        isDead = false;
    }

    public void ProcessDamage(bool isSkill, double damage, Tower attackedBy)
    {
        if (invincible || isDead) return;
        Color color = Color.black;
        if (attackedBy != null)
        {
            color = GetColorByHex(attackedBy.myConfig.colorHex);
        }
        double expectedDamage = (isSkill) ? damage : damage * GetDefenseMod();
        if (attackedBy.owner == Owner.KUROI)
        {
            double modHeal = (-expectedDamage) * buffManager.healModifier;
            DamageHP(modHeal);
            InstantiateDamageSign(GetColorByHex("#40FF40"), transform, (int)expectedDamage, true);
            //            Debug.Log("Heal be " + damage + " / expectedDamage " + expectedDamage +"/ healbuff "+buffManager.healModifier+" / mod "+modHeal+" / skill "+isSkill+ " =" + attackedBy.GetComponent<Tower>().GetUID());
        }
        else
        {
            DamageHP(expectedDamage);
            InstantiateDamageSign(color, transform, (int)expectedDamage, false);
            if (attackedBy != null)
            {
           //     Debug.Log("Damage be " + damage + " / expectedDamage " + expectedDamage + "/ defenseMod " + GetDefenseMod() + " / skill " + isSkill + " =" + attackedBy.GetComponent<Tower>().GetUID());
            }
        }
        if (healthPoint <= 0)
        {
            DoDeath(attackedBy);
        }
        if (attackedBy != null)
        {
            StatisticsManager.AddDamage(attackedBy.GetUID(), expectedDamage);
        }
    }

    double GetDefenseMod()
    {
        double defense = GetFinalDefense();

        /*
        10 = 0.16
        50 = 0.5
        150 = 0.75
        200 = 0.8
        500 = 0.9
         */
        return (1 - (defense / (defense + 100)));
    }
    public double GetFinalDefense() => defensePoint * buffManager.defenseMod;

    public void DoDeath(Tower attackedBy)
    {
        if (isDead) return;
        isDead = true;
        if (unitType == UnitType.Mob)
        {
           enemyMain.KillMe(true);
        }
        else
        {
            EventManager.TriggerEvent(MyEvents.EVENT_TOWER_SELL_REQUESTED, new EventObject(gameObject) { boolObj = false });
        }
        if (attackedBy != null)
        {
            attackedBy.IncrementKill();
        }
    }

    private void DamageHP(double amount)
    {
        healthPoint -= amount;
        if (healthPoint > fullHP)
        {
            healthPoint = fullHP;
        }
        UpdateHP();
    }
    public void UpdateHP()
    {
        if (unitType != UnitType.Mob) return;
        double val = healthPoint / fullHP;
        if (val > 0 && val < 1)
        {
            hpSlider.gameObject.SetActive(true);
            hpSlider.value = (float)val;
        }
        else
        {
            hpSlider.gameObject.SetActive(false);
        }
    }

    internal double GetHP() => healthPoint;
    internal double GetFullHP() => fullHP;

    const float critical_threshold = 0.33f;
    public void InstantiateDamageSign(Color color, Transform myTransform, int finalDamage, bool isHeal = false)
    {
        if (finalDamage == 0) return;
        float randX = Random.Range(-1f, 1f);
      //  Debug.Log("Damage instantiate");
        GameObject damObj = GetDamageSignObject(OBJ_DAMAGE_SIGN, damageIndicator, myTransform.position + new Vector3(randX, 1), Quaternion.identity);

        float sig = Math.Min(critical_threshold, finalDamage / (float)fullHP) / critical_threshold;

        //      Debug.Log("Damage:"+finalDamage + " / " + fullHP + " => " + Math.Max(0.25f, finalDamage / fullHP) + " / 0.25 = " + sig);
        damObj.GetComponent<DamageIndicator>().SetInfo(color, finalDamage, sig, isHeal);
        damObj.transform.SetParent(GameSession.GetGameSession().DamageHome);

    }
    GameObject GetDamageSignObject(string tag, GameObject prefab, Vector3 position, Quaternion angle)
    {
        GameObject damageSign = ObjectPool.PollObject(tag, position, angle);
        if (damageSign == null)
        {
            damageSign = Instantiate(prefab, position, angle);
        }
        else
        {
            damageSign.transform.position = position;
            damageSign.transform.rotation = angle;
        }
        return damageSign;
    }

    internal void HealCompletely()
    {
        healthPoint = fullHP;
        UpdateHP();
    }
    public bool IsDead() => isDead;
}
