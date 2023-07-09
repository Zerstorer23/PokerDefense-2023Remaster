using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRuleHelper : MonoBehaviour
{
    public int currentIndex = 0;
    public GameObject[] panels;
    public GameObject mainPanel;
    bool panelOpen = false;
    public Text pageText;

    public void OnClickPanelButton() {
        panelOpen = !panelOpen;
        mainPanel.SetActive(panelOpen);
        ShowCurrentPanel();
    }
    public void OnClickNext() {
        currentIndex++;
        currentIndex %= panels.Length;
        ShowCurrentPanel();
    }
    public void OnClickPrevious() {
        currentIndex--;
        if (currentIndex < 0) {
            currentIndex = panels.Length - 1;
        }
        ShowCurrentPanel();
    }

    void ShowCurrentPanel() {
        pageText.text = (currentIndex + 1) + " / " + panels.Length;
        for (int i = 0; i < panels.Length; i++) {
            panels[i].SetActive(i == currentIndex);
        }
    }

}
