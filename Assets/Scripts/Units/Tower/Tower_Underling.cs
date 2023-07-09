using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tower_Underling : MonoBehaviour
{
    //Main Data
    [Header("Put Config File here")]
    [SerializeField] UnitConfig myConfig;
   // [SerializeField] public GameObject projectile;
    public ProjectileConfig myProjConfig;
    bool configLoaded = false;

    [Header("=== Cache ===")]
    //Cache
    [SerializeField] public Tower parentTower;
    [SerializeField] public GameObject mainBody;
    [SerializeField] public GameObject gunPosition;
    [SerializeField] public GameObject d_caster;


     GameObject focusedTarget = null;

    //STAT DATA
    float damageModifier = 1f;

    public void SetUnderlingInfo(GameObject parent, UnitConfig config) {
        myConfig = config;
        parentTower = parent.GetComponent<Tower>();
        LoadConfig();
    }

    public void SetDamageModifier(float a) {
        damageModifier = a;
    }

    internal void SetPsyonicStorm(GameObject caster, Skill_Takane skillInfo, ProjectileConfig _pconfig)
    {
        d_caster = caster;
         myProjConfig = _pconfig;
        parentTower = caster.GetComponent<Tower>();
        mainBody.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(WaitAndStorm(skillInfo));
    }

    private IEnumerator WaitAndStorm(Skill_Takane skillInfo)
    {
        int numStormed = 0;
        while (numStormed < skillInfo.storm_ticks) {
            DoStorm(skillInfo);
            numStormed++;
            yield return new WaitForSeconds(skillInfo.storm_delay);
        }
        ObjectPool.SaveObject(skillInfo.objTag, gameObject);
    }

    private void DoStorm(Skill_Takane skillInfo)
    {
        for (int i = 0; i < skillInfo.one_storm_ticks; i++)
        {
            float xPos = transform.position.x + Random.Range(-1f, 1f) * skillInfo.range;
            float yPos = transform.position.y+ Random.Range(-1f, 1f) * skillInfo.range;
            StartCoroutine(FireStorm(xPos,yPos));
        }
    }

    private IEnumerator FireStorm(float _xPos, float _yPos)
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        if (parentTower == null) yield break;
        GameObject thunderbolt = parentTower.firepowerManager.PollProjectile(myProjConfig, null, parentTower.GetFinalAttackDamage() * damageModifier, false, isLand:true, xPos:_xPos, yPos:_yPos);
        thunderbolt.transform.position = new Vector3(_xPos,transform.position.y+6f);
    }

    void LoadConfig()
    {
        if (configLoaded) return;
        myProjConfig = Instantiate(myConfig.myProjectileConfig);

        mainBody.GetComponent<SpriteRenderer>().sprite = myConfig.myBodySprite;
        //Gun
        gunPosition.transform.localPosition = myConfig.myGunPos;
        configLoaded = true;
    }

    public void Fire(GameObject target)
    {
        SetFocusedTarget(target);
        //Instantiate Projectile
        double finalDamage = parentTower.GetFinalAttackDamage() * damageModifier;
        GameObject projectile = parentTower.firepowerManager.PollProjectile(myProjConfig, target, finalDamage, true);
        projectile.transform.position = transform.position;
        SetAnimationAttack();
    }
    private void FaceEnemyDirection(Transform transform)
    {
        if (focusedTarget.transform.position.x >= transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void SetFocusedTarget(GameObject obj)
    {
        focusedTarget = obj;
        FaceEnemyDirection(transform);
    }


    public Quaternion GetAngle(GameObject start, GameObject end)
    {

        Vector3 diff = start.transform.position - end.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rot_z + 90);
    }
    private void OnDestroy()
    {
        Destroy(myConfig);
        Destroy(myProjConfig);
    }


    void SetAnimationAttack() {
      //  GetComponent<Animator>().SetTrigger("Attack");
    }

}
