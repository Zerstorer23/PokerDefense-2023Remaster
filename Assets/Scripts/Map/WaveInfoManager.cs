using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveInfoManager : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;


    List<Dictionary<string, object>> library;

    List<EnemyUnitConfig> waves = new List<EnemyUnitConfig>();

    [Header("Graphic Data")]
    public Sprite[] enemySprites;
    public string[] enemyNames;
    // [SerializeField] public RuntimeAnimatorController normalMobAnimator;
    public GameObject[] bossCharacters;
    public string[] bossNames;

    /*
                                        0       5       10      15
                                        20      25      30      35
                                        40      45      50      55
                                        60      65      70
     */
    float[] hpModifiers = new float[14] {1  ,   0.8f,  0.9f,   0.9f,
                                        1.25f   ,   1.25f,  2f,     2f,
                                        2f   ,   2,  2,      1,
                                        1   ,   1};
    float[] defModifiers = new float[14]{1  ,   0.8f,  1f,     1,
                                        1.5f   ,   1.5f,  1.25f,      1.25f,
                                        1.25f   ,   1.25f,  1.25f,      1.25f,
                                        1   ,   1};
    private void Awake()
    {

      //  Debug.Log("waveinfo init start");
        LoadWaveInformations();
      //  Debug.Log("waveinfo init end");
    }

    private void LoadWaveInformations()
    {
        library = CSVReader.Read("waveInfo");
        foreach (Dictionary<string, object> data in library)
        {
            int stage = (int)data["stage"];
            int hp = (int)data["hp"];
            int defense = (int)data["defense"];
            int amount = (int)data["amount"];
            float moveSpeed = Random.Range(5f, 12f);
            int index = Mathf.Min(stage / 5, hpModifiers.Length - 1);
  

            hp = (int)(hp * hpModifiers[index]);
            defense = (int)(defense * defModifiers[index]);
            EnemyUnitConfig config;
            if (stage % 10 == 0)
            {
                string name = bossNames[stage % bossNames.Length];
                config = new EnemyUnitConfig(stage, name, amount, hp, defense, moveSpeed);
                config.SetSpriteAnimation(bossCharacters[stage % bossCharacters.Length], null);//SetPortraitImage TODO
            }
            else
            {
                string name = enemyNames[stage % enemyNames.Length];
                config = new EnemyUnitConfig(stage, name, amount, hp, defense, moveSpeed);
                config.SetSpriteAnimation(enemySprites[stage % enemySprites.Length]);
            }

            waves.Add(config);
        }

    }

    public EnemyUnitConfig GetEnemyConfig(int stageNumber)
    {
        return waves[stageNumber];
    }

    internal GameObject GetEnemyPrefab()
    {
        return enemyPrefab;
    }

    internal int GetSize()
    {
        return waves.Count;
    }
}