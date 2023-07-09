using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveDisplay : MonoBehaviour
{
    [SerializeField] Text waveText;


    public void SetWaveText(int n) {
        waveText.text = LocalizationManager.Convert("TXT_KEY_WAVE")+": "+ n;
    }
}
