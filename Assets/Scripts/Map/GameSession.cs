using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{

    //===Object pools===/
    public Transform ProjectileHome;
    public Transform ExplosionHome;
    public Transform DamageHome;



    [SerializeField] bool debug = false;
    [Range(0f,5f)] [SerializeField] float timeScale = 1f;

    //HUDS
    [SerializeField] internal MineralManager mineralManager;
    [SerializeField] internal HUDManager HUDmanager;
    public UnitConfig[] UnitConfigs;
    Dictionary<string, UnitConfig> UnitDictionary;

    //Objects
    public EnemySpawner enemySpawner;
    public TowerSpawner towerSpawner;
    public WaveManager waveManager;
    [SerializeField] internal LifeManager lifeManager;
    [SerializeField] public AudioHandler audioManager;
    public GameOverManager gameOverManager;
    internal ClickManager clickManager;
    private static GameSession gameSessionPrivate;
    public CameraMoving camera;
    private void Update()
    {
        Time.timeScale = timeScale;
    }
    private void Awake()
    {

        Debug.Log("GameSession init start");
        clickManager = GetComponent<ClickManager>();
        waveManager = GetComponent<WaveManager>();
        InitNames();
        //    LoadTextLibrary();
        LocalizationManager.LoadLocalizedText();
        Time.timeScale = timeScale;
        if (debug) {
            lifeManager.SetLives(999);
        }
        gameSessionPrivate = this;
        enemySpawner.gameSession = this;
        towerSpawner.gameSession = this;
        waveManager.enemySpawner = enemySpawner;
        waveManager.towerSpawner = towerSpawner;
        StatisticsManager.Init();
        Debug.Log("GameSession init success");
    }


    // Start is called before the first frame update
    void Start()
    {
        GooglePlayManager.DoLogin();
        EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.DRAW));
    }
    public static double GetTileDistance(Vector2 start, Vector2 end)
    {
        var vector = start - end;
        double distance = Mathf.Sqrt(Mathf.Pow(vector.x / 2, 2) + Mathf.Pow(vector.y, 2));
        return distance;
    }

    public static GameSession GetGameSession() {
        return gameSessionPrivate;
    }
    private void InitNames()
    {
        if (UnitDictionary != null) return;
        UnitDictionary = new Dictionary<string, UnitConfig>();
        foreach (UnitConfig u in UnitConfigs)
        {
            UnitDictionary.Add(u.uid, u);
        }
    }

    internal void ResetGame()
    {

        lifeManager.SetLives(20);
        waveManager.ResetWave();
        StatisticsManager.Init();
        towerSpawner.ResetTowers();
        UpgradeManager.ResetUpgrades();
        mineralManager.ResetResources();
        ReserveManager.ResetReserves();
        EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.MAP));
        EventManager.TriggerEvent(MyEvents.EVENT_BACKGROUND_CLICKED, new EventObject(Vector3.zero));
        SceneManager.LoadScene(0);
    }

    public Dictionary<string, UnitConfig> GetUnitDictionary() {
        if (UnitDictionary == null) {
            InitNames();
        }
        return UnitDictionary;
    }
}
