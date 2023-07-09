using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Text UI 사용
using UnityEngine.UI;
// 구글 플레이 연동
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;

public class GooglePlayManager : MonoBehaviour
{
    static bool loggedIn = false;
    void Awake()
    {
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public static void DoLogin()
    {
        try
        {
            if (!Social.localUser.authenticated)
            {
                Social.localUser.Authenticate((bool bSuccess) =>
                {
                    if (bSuccess)
                    {
                        Debug.Log("Success : " + Social.localUser.userName);
                        loggedIn = true;
                    }
                    else
                    {
                        Debug.Log("Fall");
                    }
                });
            }
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }
    public static void DoLogout()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }
    public static void AddAchievement(string id, float amount = 100.0f)
    {
        if (!loggedIn) return;
        //  PlayGamesPlatform.Instance.Events.IncrementEvent("YOUR_EVENT_ID", 1)
        try
        {
            Social.ReportProgress(id, amount, (bool bSuccess) =>
            {
                if (bSuccess)
                {
                    Debug.Log("AddAchievement Success " + id + " / " + amount);
                }
                else
                {
                    Debug.Log("AddAchievement Fall");
                }
            }
            );
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    //단계적 달성
    public static void IncrementAchievement(string id, int step = 1)
    {
        if (!loggedIn) return;
        try
        {
            PlayGamesPlatform.Instance.IncrementAchievement(id, step, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("점수 획득 업적 달성");
                }
                else
                {
                    Debug.Log("점수 획득 업적 달성 실패");
                }
            });
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }
    public static void AddToLeaderboard(string id, int amount)
    {
        if (!loggedIn) return;
        try
        {
            Social.ReportScore(amount, id, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("리더보드 추가");
                }
                else
                {
                    Debug.Log("리더보드 실패");
                }
            });
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }
    public static void IncrementEvent(string id, uint amount = 1)
    {
        if (!loggedIn) return;
        try
        {
            PlayGamesPlatform.Instance.Events.IncrementEvent(id, amount);
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    public void OnShowLeaderBoard()
    {
        if (!loggedIn) return;
        try
        {
            // 1000점을 등록
            Social.ReportScore(1000, IdolDefense.leaderboard_haruka_kills, (bool bSuccess) =>
            {
                if (bSuccess)
                {
                    Debug.Log("ReportLeaderBoard Success");
                }
                else
                {
                    Debug.Log("ReportLeaderBoard Fall");
                }
            }
            );
            Social.ShowLeaderboardUI();
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    // 업적보기
    public void OnShowAchievement()
    {
        if (!loggedIn) return;
        try
        {
            Social.ShowAchievementsUI();
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    // 업적추가


}



