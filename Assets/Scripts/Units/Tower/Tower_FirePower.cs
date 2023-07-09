using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;

public class Tower_FirePower : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Tower tower;
    [SerializeField] bool doAttack = true;
    [SerializeField] public GameObject gunObject;
    [SerializeField] public GameObject projectile;
    public Transform projectileParent;

    public ProjectileConfig myProjConfig;
    [SerializeField] GameObject focusedTarget = null;

    //Calculation Data
    internal float nextAttackTime = 0;
   internal float attackDelay;
   internal float attackDamage;

    float attackDamageIncrement;
    float visualDamageIncrement;


    internal bool isMelee = false;


    // Queue<GameObject> projectilePool = new Queue<GameObject>();




    private void Update()
    {

        if (focusedTarget && doAttack && attackDelay >= 0f) Fire();
    }

    internal void SetFirePowerInfo(UnitConfig myConfig) {
        attackDamage = myConfig.attackDamage;
        attackDamageIncrement = myConfig.attackUpgradeIncrement;
        visualDamageIncrement = myConfig.VisualUpgradeIncrement;
        attackDelay = myConfig.attackDelay;
        myProjConfig = Instantiate(myConfig.myProjectileConfig);
        //Gun
        gunObject.transform.localPosition = myConfig.myGunPos;
        isMelee = myConfig.isMelee;
        if (isMelee)
        {
            InitGun();
        }
    }

    void Fire()
    {
        if (Time.time < nextAttackTime || tower.IsInKnockBack()) return;
        if (focusedTarget.GetComponent<HealthPoint>().GetHP() <= 0 || !focusedTarget.activeSelf) {
            focusedTarget=null;
            return; 
        }
        //Instantiate Projectile
        double finalDamage = GetFinalAttackDamage();
        PollProjectile(myProjConfig, focusedTarget, finalDamage,false);
        FaceEnemyDirection(tower.mainBody.transform, focusedTarget.transform);
        nextAttackTime = Time.time + (float)(attackDelay * tower.buffManager.GetAttackSpeedModifier());
        tower.CheckUnderlings();



        //  Debug.Log("Next Attack at " + nextAttackTime);
        SetAnimationAttack();
    }
    public GameObject PollProjectile(ProjectileConfig customProj, GameObject target, double finalDamage, bool doInit, bool isLand = false, float xPos = 0f, float yPos = 0f)
    {
        if (customProj.isSkill)
        {
            finalDamage *= tower.buffManager.GetSkillDamageModifier();
        }
        GameObject proj = ObjectPool.PollObject(OBJ_PROJECTILE + "_" + customProj.proj_tag, Vector3.zero, Quaternion.identity);
        if (!proj)
        {
            proj = CreateProjectile();
            doInit = true;
        }
        if (tower.GetUID().Equals(ConstantStrings.MAKOTO)) doInit = true;
        if (isLand)
        {
            BrainwashObject_Land(doInit, proj, customProj, new Vector3(xPos, yPos, 0), finalDamage);
        }
        else
        {
            BrainwashObject(doInit, proj, customProj, target, finalDamage);
        }
        return proj;
    }

    private void BrainwashObject(bool doInit,GameObject projectileObject,ProjectileConfig _projectileConfig, GameObject target, double finalDamage)
    {
        projectileObject.SetActive(true);
        Vector3 startPosition = gunObject.transform.position;
        Quaternion angle = tower.GetAngle(gunObject, target);
        if (_projectileConfig.moveType == ProjectileMoveType.INDIRECT)
        {
            startPosition = new Vector3(target.transform.position.x, 3);
        }
        projectileObject.transform.position = startPosition;
        projectileObject.transform.rotation = angle;
        Projectile projectileComp = projectileObject.GetComponent<Projectile>();
        projectileComp.SetInformation(doInit, _projectileConfig, tower, target, finalDamage);
        FaceEnemyDirection(projectileObject.transform, target.transform);
    }

    private void BrainwashObject_Land(bool doInit, GameObject projectileObject, ProjectileConfig _projectileConfig, Vector3 target, double finalDamage)
    {
        projectileObject.SetActive(true);
        Vector3 startPosition = gunObject.transform.position;
        if (_projectileConfig.moveType == ProjectileMoveType.INDIRECT)
        {
            startPosition = target + new Vector3(0, 3);
        }
        projectileObject.transform.position = startPosition;
        projectileObject.transform.rotation = Quaternion.identity;
        Projectile projectileComp = projectileObject.GetComponent<Projectile>();
        projectileComp.SetInformation(doInit, _projectileConfig, tower, null, finalDamage);
        projectileComp.SetTargetToLand(target);
    }



    GameObject CreateProjectile() {
        GameObject proj = (Instantiate(projectile, Vector3.zero, Quaternion.identity) as GameObject);
        proj.transform.parent = projectileParent;
        return proj;
    }

    private void FaceEnemyDirection(Transform transform , Transform target)
    {
        if (target.position.x >= transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    internal void InitGun()
    {
        SpriteRenderer gun = gunObject.GetComponent<SpriteRenderer>();
        gun.sprite = myProjConfig.sprite;
        gunObject.GetComponent<Animator>().enabled = true;
        gunObject.GetComponent<Animator>().runtimeAnimatorController = myProjConfig.meleeController;
        gun.enabled = true;

    }
    internal void ChangeAttackDelayBy(float modifier) {
        attackDelay *= modifier;
    }

    void SetAnimationAttack()
    {
        tower.characterManager.SetAnimationTrigger("DoAttack");
        if (isMelee)
        {
            gunObject.GetComponent<Animator>().SetTrigger("DoAttack");
        }
    }
    public double GetRawDamage() {
        double vocalMod = GetFinalVocalDamage();
        double visualMod = GetFinalVisualDamage();
        double rawDamage = vocalMod * visualMod; // + tower.buffManager.GetBonusAttack();
        return rawDamage;
    }
    public double GetFinalAttackDamage()
    {
        double vocalMod = GetFinalVocalDamage();
        double visualMod = GetFinalVisualDamage();
        if (tower.owner == Owner.KUROI)
        {
          //  visualMod /= 2;
        }
        double rawDamage = vocalMod * visualMod + tower.buffManager.GetBonusAttack();
        return rawDamage * (1+tower.buffManager.GetModifiedAttackBonus());

    }
    internal double GetFinalVocalDamage()
    {
        return attackDamage + (attackDamageIncrement * UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, tower.GetUID()));

    }
    internal float GetFinalVisualDamage()
    {
        return 1+ (visualDamageIncrement * UpgradeManager.GetUpgradeValue(UpgradeType.VISUAL, tower.GetUID()));

    }
    public double GetDPS() {
        double dam = GetFinalAttackDamage();
        return dam / attackDelay;
    }


    public void FindProjectileparent()
    {
        projectileParent = GameSession.GetGameSession().ProjectileHome;
    }
    private void OnDestroy()
    {
        Destroy(myProjConfig);
    }
    public GameObject GetFocusedTarget()
    {
        return focusedTarget;
    }
    public void SetFocusedTarget(GameObject obj)
    {
        focusedTarget = obj;
    }


}
