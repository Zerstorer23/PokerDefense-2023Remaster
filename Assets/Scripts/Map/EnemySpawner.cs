using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ConstantStrings;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    //CacheData
    [Header("Initialisations")]

    [Header("Spawn Area system")]
    [SerializeField] Transform startEdge;
    [SerializeField] Transform Xedge;
    [SerializeField] Transform Yedge;
    [SerializeField] PolygonCollider2D myCursor;
    [SerializeField] CompositeCollider2D moveAreaCollider;
    [SerializeField] CompositeCollider2D PlaceAreaCollider;

    [Header("Path system")]
    // [SerializeField] internal PathBuilder pathBuilder;
    [SerializeField] internal GameObject[] pathways;



    Vector2 stepX, stepY;
    int stepSize = 5;
    int currX, currY;

    GameObject enemyPrefab;
    internal EnemyUnitConfig enemyUnitConfig;
    [SerializeField] int totalNumberToSpawn;
    int stageNumber = 0;
    //Data

    [Header("EnemySpawn system")]
    [SerializeField] private Dictionary<string,GameObject> enemies = new Dictionary<string,GameObject>();
    [SerializeField] float spawningDelay = 10f;
    [SerializeField] int numSpawnAtTime = 6;
    [SerializeField] bool active = true;
    [SerializeField] bool DoDebugSpawn = false;
    [SerializeField] int debugSpawnAmount = 300;
    [SerializeField] int debugSpawnHP = 300;
    [SerializeField] int debugSpawnDef = 300;
    // [SerializeField] bool spawnFinished = false;
    public int numSpawned = 0;

    internal GameSession gameSession;
    internal WaveInfoManager waveInfoManager;

    Coroutine spawnCor;



    private void Awake()
    {
    //    Debug.Log("enspawner init start");
        EventManager.StartListening(MyEvents.EVENT_WAVE_TIMEOUT, TImeoutDestroyAll);
        waveInfoManager = GetComponent<WaveInfoManager>();
        InitialiseBoundaries();
      //  Debug.Log("enspawner init end");
    }

    internal int GetEnemyCount()
    {
        return enemies.Count;
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_WAVE_TIMEOUT, TImeoutDestroyAll);

    }
    internal void LoadWaveConfig(int stageNumber)
    {
        this.stageNumber = stageNumber;
        this.enemyPrefab = waveInfoManager.GetEnemyPrefab();
        enemyUnitConfig = waveInfoManager.GetEnemyConfig(stageNumber);
        this.totalNumberToSpawn = enemyUnitConfig.GetEnemyAmount();
    }

    private void TImeoutDestroyAll(EventObject eo)
    {//대신 사무소로 이동
        if (spawnCor != null)
        {
            Debug.Log("Stop coroutine " + spawnCor);
            StopCoroutine(spawnCor);
        }
        totalNumberToSpawn = 0;
    }


    public void StartWave()
    {
        if (!active) return;
        //pathBuilder.ResetMap();
        // spawnFinished = false;
        numSpawned = 0;
        if (DoDebugSpawn)
        {
            DebugSpawn();
        }
        else
        {
            spawnCor= StartCoroutine(WaitAndSpawn());
        }
    }

    private void DebugSpawn()
    {
        for (int i = 0; i < debugSpawnAmount; i++)
        {
            GameObject obj = InstantiateEnemy();
            obj.GetComponentInChildren<HealthPoint>().healthPoint = debugSpawnHP;
            obj.GetComponentInChildren<HealthPoint>().fullHP = debugSpawnHP;
            obj.GetComponentInChildren<HealthPoint>().defensePoint = debugSpawnDef;

        }
    }

    public IEnumerator WaitAndSpawn()
    {
        Debug.Log("Wait and spawn "+totalNumberToSpawn);
        while (totalNumberToSpawn > 0)
        {
            int spawned = 0;

            InitialiseCursorPosition();
            int targetAmount = (totalNumberToSpawn < numSpawnAtTime) ? totalNumberToSpawn : numSpawnAtTime;
            while (spawned < targetAmount)
            {
                IncrementCursor();
                InstantiateEnemy();
                spawned++;
                yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            }

            yield return new WaitForSeconds(spawningDelay);

        }
    }


    GameObject InstantiateEnemy()
    {
        if (totalNumberToSpawn == 0) return null;
        GameObject enemy;
        if (enemyUnitConfig.enemyType == EnemyType.Boss)
        {
            enemy = Instantiate(enemyPrefab, myCursor.transform.position, Quaternion.identity);

        }
        else
        {
            enemy = ObjectPool.PollObject(OBJ_ENEMY_MOB, myCursor.transform.position, Quaternion.identity);
            if (enemy == null)
            {
                enemy = Instantiate(enemyPrefab, myCursor.transform.position, Quaternion.identity);

            }
        }
        string uid = stageNumber + "/" + numSpawned++;
        totalNumberToSpawn--;
        Enemy_Main enemyMain = enemy.GetComponentInChildren<Enemy_Main>();
        enemyMain.SetInformation(this, uid);
        enemy.transform.parent = transform;
        enemy.GetComponentInChildren<Enemy_PathFind>().SetPathInfo(pathways);
        enemies.Add(uid,enemy);

        return enemy;
    }


    private void InitialiseCursorPosition()
    {
        myCursor.transform.position = startEdge.position;
        currX = 0;
        currY = 0;
    }

    private bool IncrementCursor()
    {
        currY++;
        if (currY >= stepSize)
        {
            currY = 0;
            currX++;
            if (currX >= stepSize)
            {
                InitialiseCursorPosition();
            }
        }
        SetCursor();
        return true;
    }
    private void SetCursor()
    {
        Vector2 newPos = new Vector2(startEdge.position.x, startEdge.position.y);
        newPos += stepX * currX;
        newPos += stepY * currY;
        myCursor.transform.position = newPos;
    }

    public void RemoveEnemy(string key_uid)
    {

        // string enemyToRemove = v.GetString();
        if (enemies.ContainsKey(key_uid))
        {
            Enemy_Main enemy_Main = enemies[key_uid].GetComponentInChildren<Enemy_Main>();
            DestroyEnemy(enemy_Main.enemyType == EnemyType.Boss, enemies[key_uid]); // Save to obj pool
            enemies.Remove(key_uid);
            if (enemies.Count == 0 && totalNumberToSpawn == 0)
            {
                EventManager.TriggerEvent(MyEvents.EVENT_WAVE_ALL_DEAD, new EventObject(Time.time));
            }
        }
        else {
            Debug.LogWarning("Removing unexist enemy?");
        }
    }

    private void DestroyEnemy(bool isBoss, GameObject enemyObject)
    {
        if (isBoss)
        {
            Destroy(enemyObject);
        }
        else
        {
            ObjectPool.SaveObject(OBJ_ENEMY_MOB, enemyObject);
        }
    }






    private void InitialiseBoundaries()
    {
        stepX = (Xedge.position - startEdge.position) / stepSize;
        stepY = (Yedge.position - startEdge.position) / stepSize;

    }

    internal GameObject GetMostValuable()
    {
        double highHP = -1;
        GameObject mvp = null;
        foreach ( GameObject obj in enemies.Values)
        {
            double thisHP = obj.GetComponentInChildren<HealthPoint>().GetHP();
            if (thisHP > highHP
                || !mvp)
            {
                if (!obj.GetComponentInChildren<BuffManager>().isTargetOfSkill)
                {
                    highHP = thisHP;
                    mvp = obj;
                }

            }
        }
        if (mvp)
        {
            return mvp.GetComponentInChildren<Enemy_Main>().gameObject;
        }
        else
        {
            return null;
        }

    }
}
