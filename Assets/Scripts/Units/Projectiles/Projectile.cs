using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;

public class Projectile : MonoBehaviour
{
    ProjectileConfig myConfig;
    Projectile_PathFinder pathFinder;
    // Start is called before the first frame update
    [SerializeField] internal GameObject targetObject;
    internal Vector3 targetLocation;

    internal Tower tower;
    public Owner owner;
    public string objTag;

    [SerializeField] internal SpriteRenderer spriteRenderer;
    [SerializeField] GameObject healParticle;
    //  ProjectileConfig myConfig;


    bool isPenetration = false;
    bool isSplash = false;
    bool isInstakill = false;
    int numBounce = 0;

    bool isSkill = false;
    internal float effectTime = 0f;
    internal float influenceRange = 0f;
    internal double splashModifier = 0.5f;

    List<Buff> cloned_buff = new List<Buff>();

    GameObject explosionPrefab;

   [SerializeField] double damage;

    public void SetInformation(bool doInit, ProjectileConfig config, Tower source, GameObject target, double finalDamage)
    {
        //Config data = static
        if (doInit)
        {
            InitialiseConfig(config);
        }
        //ONew info
        tower = source;
        this.damage = finalDamage;
        this.targetObject = target;
        this.owner = source.owner;
        pathFinder.sourceOfFire = source.transform;
        SetBuffInformation();
        if (pathFinder.moveType == ProjectileMoveType.DIRECTED_VECTOR)
        {
            pathFinder.moveVector = target.transform.position - source.gameObject.transform.position;
        }

    }
    public void SetTargetToLand(Vector3 target)
    {
        pathFinder.isLandTarget = true;
        targetObject = null;
        targetLocation = target;
    }

    private void SetBuffInformation()
    {
        numBounce = myConfig.numBounce;
        influenceRange = myConfig.influenceRange;
        splashModifier = myConfig.splashModifier;
        cloned_buff = new List<Buff>();
        List<Buff> rawbuffInfo = myConfig.GetCustomBuffs();
        foreach (Buff buff in rawbuffInfo)
        {
            if (buff.triggerTower == null) {
                //투사체에 등록한 타워가 삭제되고 그 투사체가 재활용되면 벌어지는 일 투사체의 버프는 이전걸 쓰고있음
                //Debug.LogWarning(tower.GetUID() + " is sending null buff "+tower);
                buff.triggerTower = tower;
            }
            cloned_buff.Add(buff.Clone());
            
        }


        if (isPenetration)
        {
            CircleCollider2D collider2D = GetComponent<CircleCollider2D>();
            collider2D.radius = influenceRange;
          //  collider2D.enabled = true;
        }
    }


    private void InitialiseConfig(ProjectileConfig config)
    {
        myConfig = config;
        objTag = OBJ_PROJECTILE + "_" + config.proj_tag;
        pathFinder = GetComponent<Projectile_PathFinder>();
        pathFinder.moveSpeed = myConfig.moveSpeed;
        explosionPrefab = myConfig.explosionPrefab;
        spriteRenderer.sprite = myConfig.sprite;
        isSkill = config.isSkill;
        isPenetration = myConfig.isPenetration;
        isSplash = myConfig.isSplash;
        isInstakill = myConfig.isInstaKill;
        pathFinder.SetMoveType(config.moveType);

    }



    private void OnTriggerEnter2D(Collider2D c)
    {//This is penetration damage.
        if (!isPenetration) return;
        if (c.gameObject.CompareTag(TAG_ENEMY))
        {
            c.gameObject.GetComponent<HealthPoint>().ProcessDamage(isSkill, this.damage, tower);
            InstantiateExplosionAt(c.gameObject.transform, false);
        }
    }

    // Update is called once per frame




    public void DealDamage(bool doExplosion)
    {
        bool requireSacrifice = false;
        //단일 타겟
        if (targetObject)
        {
            HealthPoint health = targetObject.GetComponent<HealthPoint>();
            if (!health.IsDead())
            {
                // 1. 스킬적용
                if (isInstakill || cloned_buff.Count > 0)
                {
                    requireSacrifice = (health.unitType== UnitType.Tower && isInstakill);
                    ApplyASkill(targetObject.GetComponent<BuffManager>(), tower);
                }
                // 2. 살았으면 데미지 적용
                if (!health.IsDead())
                {
                    CheckFutamiDamage();
                    health.ProcessDamage(isSkill, this.damage, tower);
                }
            }
        }
        //스플래시 확인
        //1. 범위스킬 적용
        if (influenceRange > Mathf.Epsilon && (isInstakill || cloned_buff.Count > 0))
        {
            ApplySkillMultipleTargets(); // <- 대상타겟은 미포함. 타워도 미포함/ 죽은애도 미포함
        }

        //2. 스플레시 데미지 적용.
        if (isSplash)
        {
            ApplySplashDamage();
        }

      //  3. 야요이 바운스
        if (numBounce > 0)
        {
            DoBounce();
            InstantiateExplosionAt(transform, false);
        }
        else if (doExplosion)
        {
            DestroyMyself();
            InstantiateExplosionAt(transform, false);
            if (requireSacrifice)
            {
                EventManager.TriggerEvent(MyEvents.EVENT_TOWER_SELL_REQUESTED, new EventObject(tower.gameObject) { boolObj = false });
            }
        }

    }


    private void DoBounce()
    {

        targetObject = FindBounceTarget();
        this.damage *= splashModifier;
        numBounce--;
        if (targetObject == null)
        {
            numBounce = 0;
        }


    }
    internal GameObject FindBounceTarget()
    {
        List<GameObject> targets = GetEnemyNear(transform.position, myConfig.bounceRange);
        return targets.Count > 0 ? targets[0] : null;
    }
    internal Vector3 GetTargetLocation()
    {
        return (pathFinder.isLandTarget) ? targetLocation : targetObject.transform.position;
    }
    private List<GameObject> GetEnemyNear(Vector3 sourceOfExplosion, float range)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask(STRING_LAYER_UNITS));
        List<GameObject> targets = new List<GameObject>();
        foreach (Collider2D c in hitColliders)
        {
            GameObject target = c.gameObject;
            if (sourceOfExplosion == target.transform.position) continue;
            try
            {
                if (target.GetComponent<HealthPoint>().IsDead()) continue;
                if (target.CompareTag(TAG_ENEMY))
                {
                  //  Debug.Log("Found target " + target.transform.position);
                    targets.Add(target);
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
            }
        }
      //  Debug.Log("Num targets " + targets.Count);
        return targets;
    }

    private void CheckFutamiDamage()
    {
        TowerCharacter character = tower.GetCharacterID();
        int level = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, AMI);
        bool apply;
        if (level == 4)
        {
            apply = true;
        }
        else
        {
            apply = (character == TowerCharacter.Ami || character == TowerCharacter.Mami);
        }
        if (apply)
        {
          //  Debug.Log("Apply futami " + character + " mod " + targetObject.GetComponent<BuffManager>().Futami_receiveDamageModifier);
            this.damage *= targetObject.GetComponent<BuffManager>().Futami_receiveDamageModifier;
        }


    }

    private void ApplySplashDamage()
    {
        List<GameObject> targets = GetEnemyNear(transform.position, influenceRange);
        foreach (GameObject target in targets)
        {
            try
            {
                HealthPoint health = target.GetComponent<HealthPoint>();
                if (!health.IsDead()) {
                    health.ProcessDamage(isSkill, this.damage * this.splashModifier, tower);
                    InstantiateExplosionAt(target.transform, true);
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
    private void ApplySkillMultipleTargets()
    {
        List<GameObject> targets = GetEnemyNear(transform.position, influenceRange);
        foreach (GameObject target in targets)
        {
            try
            {
                ApplyASkill(target.GetComponent<BuffManager>(), tower);
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
    private void ApplyASkill(BuffManager enemyBuffManager, Tower tower)
    {
        if (isInstakill)
        {
            enemyBuffManager.InstaKill(tower,owner);
            Debug.Log("Instakill " + enemyBuffManager.gameObject.transform.position + " by " + tower.GetCharacterID());
        }
        if (cloned_buff.Count > 0)
        {

            foreach (Buff buff in cloned_buff)
            {
                buff.StartTimer();
                enemyBuffManager.AddBuff(buff);
            }
        }
    }
    public void DestroyMyself()
    {
        ObjectPool.SaveObject(objTag, gameObject);
    }

    public void InstantiateExplosionAt(Transform myTransform, bool randAngle)
    {
        Quaternion angle = GetAngle(randAngle);

        GameObject explosion;
        if (tower.owner == Owner.NAMCO)
        {
            explosion = GetExplosionObject(OBJ_EXPLOSION, explosionPrefab, myTransform.position, angle);
            explosion.GetComponent<Explosion>().SetInformation(myConfig);
        }
        else
        {
            if (pathFinder.moveType == ProjectileMoveType.INSTA) {
                GameObject sub = GetExplosionObject(OBJ_EXPLOSION, explosionPrefab, myTransform.position, angle);
                sub.GetComponent<Explosion>().SetInformation(myConfig);
            }
            explosion = GetExplosionObject(OBJ_EXPLOSION_HEAL, healParticle, myTransform.position, angle);
            explosion.GetComponent<Explosion>().SetTimer(Random.Range(0.3f, 0.7f));
        }
    }


    private Quaternion GetAngle(bool randAngle)
    {
        Quaternion angle = Quaternion.identity;
        if (!myConfig.verticalExplosion)
        {
            if (randAngle)
            {
                angle = Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f));
            }
            else
            {
                if (tower != null)
                {
                    angle = tower.GetAngle(tower.gameObject, gameObject);
                }
            }
        }
        return angle;
    }

    GameObject GetExplosionObject(string tag, GameObject prefab, Vector3 position, Quaternion angle)
    {

        string objTag = tag;
        if (tag.Equals(OBJ_EXPLOSION))
        {
            objTag += "_" + myConfig.proj_tag;

        }
        // Debug.Log("Requested " + objTag + " by " + tower.GetUID() + " / " + tower.owner);
        GameObject explosion = ObjectPool.PollObject(objTag, position, angle);


        if (explosion == null)
        {
            explosion = Instantiate(prefab, position, angle);
            explosion.transform.SetParent(GameSession.GetGameSession().ExplosionHome);
            //  Debug.Log("New " + objTag + " by " + tower.GetUID() + " / " + tower.owner);
        }
        else
        {
            explosion.transform.position = position;
            explosion.transform.rotation = angle;
            // Debug.Log("Pulled " + objTag + " by " + tower.GetUID() + " / " + tower.owner);
        }
        explosion.GetComponent<Explosion>().SetTag(objTag);
        return explosion;
    }

}
