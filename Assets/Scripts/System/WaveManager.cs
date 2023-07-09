using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveManager : MonoBehaviour
{

    float startedTime;
    float endTime;

    internal int waveIndex = 0;
    internal EnemySpawner enemySpawner;
    internal TowerSpawner towerSpawner;
    [SerializeField] internal WaveDisplay waveDisplay;
    public TilemapRenderer kuroiTiles;

    bool isInWave;

    //------- 웨이브 시작 끝 계산

    private void Awake()
    {
     //   Debug.Log("wavemanager init srtart");
        EventManager.StartListening(MyEvents.EVENT_WAVE_ALL_DEAD, OnAllDead);
        EventManager.StartListening(MyEvents.EVENT_WAVE_START_REQUESTED, OnGameStartRequested);
    //    Debug.Log("wavemanager init end");
    }


    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_WAVE_ALL_DEAD, OnAllDead);
        EventManager.StopListening(MyEvents.EVENT_WAVE_START_REQUESTED, OnGameStartRequested);
    }


    private void OnAllDead(EventObject arg0)
    {
        Debug.Log("Received all enemy dead "+waveIndex);
        StartCoroutine(WaitAndDo());
    }
    IEnumerator WaitAndDo() {
        yield return new WaitForFixedUpdate();
        DoGameEnd();
    }

    private void LoadNextWave()
    {
        enemySpawner.LoadWaveConfig(waveIndex++);
        waveDisplay.SetWaveText(waveIndex);
    }

    public void StartGame()
    {
        startedTime = Time.time; 
        enemySpawner.StartWave();
        isInWave = true;
        EventManager.TriggerEvent(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, new EventObject(Time.time));
    }

    void OnGameStartRequested(EventObject eo)
    {
        if (enemySpawner.GetEnemyCount() != 0) return;
        LoadNextWave();
        StartGame();
    }
    public void DoGameEnd()
    {
        EventManager.TriggerEvent(MyEvents.EVENT_GAMESESSION_WAVE_FINISHED, new EventObject(Time.time));
        Debug.Log("Wave finished" +waveIndex);
        endTime = Time.time;
        isInWave = false;
        StatisticsManager.BuildData(towerSpawner);

        //해당스테이지가 끝나고 나옴
        switch (waveIndex)
        {
            case 1:
                TutorialManager.CheckTutorial("AfterFirstStage");
                break;
            case 4:
                TutorialManager.CheckTutorial("LearnBossStage");
                break;
            case 5:
                TutorialManager.CheckTutorial("LearnManyWave");
                break;
            case 6:
                TutorialManager.CheckTutorial("LearnTrendingIdol");
                GooglePlayManager.AddAchievement(IdolDefense.achievement_c_rank_producer);
                break;
            case 10:
                kuroiTiles.enabled = true;
                TutorialManager.CheckTutorial("LearnKuroiProductionIntro");
                TutorialManager.CheckTutorial("LearnKuroiProduction");
                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_10);
                break;
            case 20:
                GooglePlayManager.AddAchievement(IdolDefense.achievement_b_rank_producer);

                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_20);
                break;
            case 30:
                GooglePlayManager.AddAchievement(IdolDefense.achievement_a_rank_producer);
                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_30);
                break;
            case 40:
                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_40);
                break;
            case 50:
                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_50);
                GooglePlayManager.AddAchievement(IdolDefense.achievement_s_rank_producer);
                break;
            case 60:
                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_60);
                break;
            case 70:
                GooglePlayManager.IncrementEvent(IdolDefense.event_clear_70);
                GameSession.GetGameSession().gameOverManager.ShowGameOver(true);
                break;

        }
        if (waveIndex % 5 == 0)
        {
     //       GameSession.GetGameSession().mineralManager.AddResource(UpgradeType.DANCE, 1);
         //   EventManager.TriggerEvent(MyEvents.EVENT_MESSAGE_TRIGGERED,new EventObject( "보스전 클리어 댄스 +1"));
            EventManager.TriggerEvent(MyEvents.EVENT_SHOW_IDOL_RANK, new EventObject(true));
        }

    }

    internal void ResetWave()
    {
        waveIndex = 0;
        kuroiTiles.enabled = false;
    }

    public bool IsInWave() { return isInWave; }
    public float GetStartTime() => startedTime;
    public float GetEndTime() => endTime;
    public int GetCurrentWaveNumber() => waveIndex;
    public float GetLastWaveElapsedTime()
    {
        return endTime - startedTime;
    }
}
