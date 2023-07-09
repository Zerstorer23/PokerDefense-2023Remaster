using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    //Main Data
    [Header("Put Config File here")]
    [SerializeField] public UnitConfig myConfig;
    internal BuffManager buffManager;
    [SerializeField] private TowerCharacter characterID = TowerCharacter.None;
    [SerializeField] private string uid = "";
    [SerializeField] private string GameID = "";
    public Vector3 mapPosition;
    [SerializeField] public Owner owner = Owner.NAMCO;
    /// Char Body
    GameObject character;
    internal CharacterBodyManager characterManager;
    CharacterConfig characterConfig;


    [Header("=== Cache ===")]
    //Cache
    [SerializeField] public GameObject mainBody;

    internal double GetFinalAttackDamage()
    {
        return firepowerManager.GetFinalAttackDamage();
    }

    [SerializeField] public TowerTargetFinder targetFinder;
    [SerializeField] Transform[] underlingPositions;

    GameObject[] underlings;
   // public GameSession gameSession;
    bool hasUnderlings = false;
    [Header("=== ===== ===")]


    Sprite portraitSprite;
    internal SkillManager skillManager;
    TowerSpawner towerSpawner;
    public ActiveSkillManager activeSkillManager;
    internal Tower_FirePower firepowerManager;



    internal float attackDistance = 9f;

    //Tower Statistic
    int kills = 0;
    public int sell_Vocal = 0;
    public int sell_Visual = 0;
    public int sell_Dance = 0;
    public int unitMass = 0;

    // Update is called once per frame
    private void Start()
    {
        if (transform.position.z != 0) transform.position = new Vector3(transform.position.x, transform.position.y, 0);


        targetFinder.SetAttackDistance(attackDistance);
        firepowerManager.FindProjectileparent();
        //   SetSortingOrder();
    }
    internal void ChangeAttackDistance(float newDist)
    {
        attackDistance = newDist;
        targetFinder.SetAttackDistance(attackDistance);
    }

    internal Sprite GetPortraitSprite() => portraitSprite;

    public void SetConfig(string _gameID , UnitConfig config, Owner _owner)
    {
        myConfig = config;
        uid = myConfig.uid;
        characterID = myConfig.characterID;
        GameID = _gameID;
        sell_Vocal = myConfig.sellValue_Vocal;
        sell_Visual = myConfig.sellValue_Visual;
        sell_Dance = myConfig.sellValue_Dance;
        owner = _owner;
        unitMass = config.unitMass;
        //Load FindCache
        towerSpawner = FindObjectOfType<TowerSpawner>();
        activeSkillManager = FindObjectOfType<ActiveSkillManager>();
        buffManager = GetComponent<BuffManager>();
        firepowerManager = GetComponent<Tower_FirePower>();
        firepowerManager.SetFirePowerInfo(myConfig);
        //Body
        InstantiateCharacterBody();


        attackDistance = myConfig.attackDistance;
        underlings = new GameObject[underlingPositions.Length];


        portraitSprite = myConfig.GetPortraitSprite();

        //Skills
        skillManager = GetComponent<SkillManager>();
        skillManager.InitialiseSkills(myConfig.skillConfigs, gameObject);

    }



    private void InstantiateCharacterBody()
    {
        characterConfig = myConfig.characterConfig;

        //Instantiate
        character = (Instantiate(characterConfig.characterBody, mainBody.transform.position + new Vector3(0, -0.25f, 0f), Quaternion.identity) as GameObject);
        character.transform.parent = mainBody.transform;
        characterManager = character.GetComponent<CharacterBodyManager>();
        characterManager.SetUID(characterConfig, this, GetUID());
        //Init
        characterManager.SetColors(characterConfig.mainColorHex, characterConfig.subColorHex);
        characterManager.SetHairSkins(characterConfig.hairID);
        characterManager.SetMouthSkin(characterConfig.mouthID);
        if (owner == Owner.NAMCO)
        {
            characterManager.SetEyeSkin(characterConfig.eyeID);

        }
        else {

            characterManager.SetEyeSkin("6");
        }
    }



    public void AddBuff(Buff buff)
    {
        buffManager.AddBuff(buff);
    }


    internal void DoCoroutine(IEnumerator enumerator)
    {
        if (!gameObject.activeSelf) return;
        StartCoroutine(enumerator);
    }
    internal GameObject GetGunObject()
    {
        return firepowerManager.gunObject;
    }

    public Quaternion GetAngle(GameObject start, GameObject end)
    {
        Vector3 diff = start.transform.position - end.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rot_z + 90);
    }




    internal void CheckUnderlings()
    {
        //Underligns
        if (hasUnderlings)
        {
            for (int i = 0; i < underlings.Length; i++)
            {
                if (underlings[i] != null)
                {
                    underlings[i].GetComponent<Tower_Underling>().Fire(firepowerManager.GetFocusedTarget());
                }
            }
        }
    }



    internal void IncrementKill()
    {
        if (owner != Owner.NAMCO) return;
        kills++;
        StatisticsManager.AddToStat("KILL_"+uid, 1);
        StatisticsManager.AddToStat("TOTAL_KILL_"+uid, 1);
    }
    internal bool SpawnUnderling( UnitConfig underling, float damageMod)
    {
        int spawnPos = GetSpawnPos();
        if (spawnPos < 0) return false;

        GameObject  Lexington= Instantiate(activeSkillManager.GetUnderlingPrefab(), underlingPositions[spawnPos].position, Quaternion.identity) as GameObject;
   
            
        Lexington.transform.parent = transform;
        UnitConfig Kaiten = Instantiate(underling) as UnitConfig;
        Lexington.GetComponent<Tower_Underling>().SetUnderlingInfo(gameObject, Kaiten);
        Lexington.GetComponent<Tower_Underling>().SetDamageModifier(damageMod);
        underlings[spawnPos] = Lexington;
        hasUnderlings = true;
        return true;
    }
    internal void KillAllUnderling()
    {
        for (int i = 0; i < underlings.Length; i++)
        {
            if (underlings[i] != null)
            {
                Destroy(underlings[i]);
                underlings[i] = null;
            }
        }
    }
    private int GetSpawnPos()
    {
        int spawnPos = 0;
        while (spawnPos < underlingPositions.Length)
        {
            if (underlings[spawnPos] == null)
            {
                return spawnPos;
            }
            spawnPos++;
        }
        return -1;
    }

    public void KillItself() {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {

        Destroy(myConfig);
        Destroy(character);
        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_REMOVED, new EventObject(transform.position));
        skillManager.DoUnitDestroyed();
    }

    private void OnDisable()
    {

        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_DISABLED, new EventObject(transform.position));
        if (owner == Owner.NAMCO)
        {
            activeSkillManager.RemoveActiveSkill(gameObject, skillManager.GetSkillList());
        }
    }
    public TowerSpawner GetTowerSpawner() => towerSpawner;

    public bool IsInKnockBack() => buffManager.numStun > 0;


    public float GetFinalAttackDelay() => firepowerManager.attackDelay;
    internal float GetFinalAttackRange() => attackDistance;
    public string GetUID() => uid;
    public TowerCharacter GetCharacterID() => characterID;
    public string GetGameID() => GameID;
    public int GetKills() => kills;
    public List<Skill> GetSkills() => skillManager.Skills_All;
}

public enum Owner
{
    NONE, NAMCO, KUROI
}

public enum TowerCharacter { 

   None, Yukiho,Haruka,Ami,Mami,Chihaya,Yayoi,Takane,Hibiki,Makoto,Miki,Iori,Azusa

}