using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [Range(0,20)] [SerializeField] int lives = 20;
    [SerializeField] Image[] imageLevels;
    [SerializeField] TextMeshProUGUI lifeDisplay;

    private void Start()
    {
        UpdateUI();
    }

    public int DecrementLives() {
        
        lives--;
        CheckLives();
        GameSession.GetGameSession().camera.DoShake(7, 0.4f);
        UpdateUI();
        return lives;
    }
    public int DecrementLives(int a) 
    {
        lives -=a;
        Handheld.Vibrate();
        GameSession.GetGameSession().camera.DoShake(7, 0.4f);
        CheckLives();
        UpdateUI();
        return lives;
    }

    void CheckLives() {
        if (lives <= 0)
        {
            GameSession.GetGameSession().gameOverManager.ShowGameOver(false);
        }
    }


    void UpdateUI() {
        //0 0~4
        //1 5~9
        //2 10~14
        //3 15~20
        int stepSize = 20 / imageLevels.Length;
        int level = lives / stepSize;
        float fill = (float)(lives % (stepSize)) / (stepSize);
        for (int i = 0; i < imageLevels.Length; i++) {
            if (i < level)
            {
                imageLevels[i].fillAmount = 1;
            }
            else if (i == level)
            {
                imageLevels[i].fillAmount = fill;
            }
            else if (i > level)
            {
                imageLevels[i].fillAmount = 0;
            }
        
        }
        lifeDisplay.text = lives.ToString();
    }
    public int GetLives() => lives;
    public void SetLives(int a) { 
        lives = a;
        UpdateUI();
    }
}
