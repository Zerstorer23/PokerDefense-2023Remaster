using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GooglePlayButton : MonoBehaviour
{
    Text text;
    void Awake()
    {
        text = GetComponentInChildren<Text>();  
    }

    public void DoLogin()
    {
        GooglePlayManager.DoLogin();
    }
    public void DoLogout()
    {
        GooglePlayManager.DoLogout();
    }

    public void OnShowLeaderBoard()
    {
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
        try
        {
            Social.ShowAchievementsUI();
        }
        catch (Exception e) { Debug.Log(e.Message); }
    }

}
