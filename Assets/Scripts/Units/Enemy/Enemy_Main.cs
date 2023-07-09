
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static ConstantStrings;

public class Enemy_Main : MonoBehaviour
{
    [SerializeField] Enemy_PathFind pathfinder;
    [SerializeField] Enemy_OnClick onClickManager;
    [SerializeField] internal SpriteRenderer mainBodySprite;
    GameObject characterBody;
    Sprite portraitImage;

    internal Enemy_CharacterManager bodyManager;
    public BuffManager buffManager;
    public HealthPoint healthManager;
    internal bool hasCharBody = false;

    [SerializeField] string txt_name;
    [SerializeField] float unitSize;
    [SerializeField] string uniqueID ;

    EnemyUnitConfig unitConfig;

   public EnemyType enemyType= EnemyType.Boss;

    GameSession gameSession;
    private void Awake()
    {
      //  Debug.Log("eneymain init srtart");
        buffManager = GetComponent<BuffManager>();
        healthManager = GetComponent<HealthPoint>();
       // Debug.Log("eneymain init end");
    }

    public void SetInformation(EnemySpawner spawner, string id)
    {
        this.uniqueID = id;
        gameSession = spawner.gameSession;
       // pathfinder.pathBuilder = spawner.pathBuilder;
        pathfinder.jimusyo = gameSession.lifeManager.gameObject.transform;
        SetUnitConfig(spawner.enemyUnitConfig);
        healthManager.UpdateHP();
    }

  
    public void KillMe(bool giveReward)
    {
        EventManager.TriggerEvent(MyEvents.EVENT_ENEMY_DEAD, new EventObject(this.uniqueID));
        ProcessReward(giveReward);
        if (hasCharBody)
        {
            pathfinder.doMove = false;
            bodyManager.SetTrigger("DoDeath");
        }
        else {
            DestroyCharacter();
        }
    }
    public void DestroyCharacter()
    {
        gameSession.enemySpawner.RemoveEnemy(uniqueID);
    }
    private void OnDestroy()
    {
        Destroy(characterBody);
    }

    private void ProcessReward(bool giveReward)
    {
        if (giveReward)
        {
            if (gameSession == null) return;
            int rewardAmount = 0;
            switch (enemyType)
            {
                case EnemyType.Normal:
                    rewardAmount = (int)(MineralManager.KILL_REWARD * buffManager.rewardModifier);
                    break;
                case EnemyType.Small:
                    rewardAmount = (int)(MineralManager.KILL_REWARD_SMALL * buffManager.rewardModifier);
                    break;
                case EnemyType.Boss:
                    rewardAmount = (int)(MineralManager.KILL_BOSS_REWARD * buffManager.rewardModifier);
                    break;
            }
        //    Debug.Log(uniqueID+"||"+enemyType + " : " + rewardAmount + " (" + buffManager.rewardModifier + ")");
            gameSession.mineralManager.AddResource(UpgradeType.VOCAL,rewardAmount);
   
        }
        else {
            if (enemyType == EnemyType.Boss)
            {
                GooglePlayManager.AddAchievement(IdolDefense.achievement_producer_run);
                gameSession.lifeManager.DecrementLives(gameSession.lifeManager.GetLives());
            }
            else {

                gameSession.lifeManager.DecrementLives();
            }
         
        }
    }
    internal void RemoveFocusUI()
    {
        onClickManager.HideDisplay();
    }

    internal string GetName() => LocalizationManager.Convert(txt_name);

    internal string GetUniqueID() => this.uniqueID;



    private void SetUnitConfig(EnemyUnitConfig config)
    {
        unitConfig = config;
        healthManager.fullHP = config.healthPoint;
        healthManager.defensePoint = config.defensePoint;
        healthManager.healthPoint = healthManager.fullHP;
        portraitImage = config.spriteImage;
        txt_name = config.name;
        enemyType = config.enemyType;
        pathfinder.initial_moveSpeed = Random.Range(config.moveSpeed - 1f, config.moveSpeed + 1f);

        if (config.spriteImage != null)
        {
            mainBodySprite.sprite = config.spriteImage;
            //  GetComponent<Animator>().runtimeAnimatorController = config.animatorController;
        }
        else
        {
            mainBodySprite.enabled = false;
            GetComponent<Animator>().enabled = false;
            hasCharBody = true;
            characterBody = (Instantiate(unitConfig.characterBody, transform.position + new Vector3(0, -0.25f, 0f), Quaternion.identity) as GameObject);
            characterBody.transform.parent = mainBodySprite.gameObject.transform;
            bodyManager = characterBody.GetComponent<Enemy_CharacterManager>();
            bodyManager.enemyMain = this;
        }
    }

    internal Sprite GetPortaitImage()
    {
        return portraitImage;
    }
}

public enum EnemyType { 
    Normal,
    Small,
    Boss

}
