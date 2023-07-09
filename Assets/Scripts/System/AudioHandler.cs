using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public AudioClip[] audioLists;
    AudioSource audioPlayer;
    int currentPlay = -1;
    private void Awake()
    {
     //   Debug.Log("audio init start");
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnShowPanel);
        audioPlayer = GetComponent<AudioSource>();
     //   Debug.Log("audio init end");
    }
    private void OnDestroy()
    {

        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnShowPanel);
    }

    private void OnShowPanel(EventObject eo)
    {
      //  Debug.Log("audio play start");
        ScreenType screen = eo.screenType;
        switch (screen)
        {
            case ScreenType.MAP:
                currentPlay = 0;
                break;
            case ScreenType.DRAW:
                currentPlay = 1;
                break;
            case ScreenType.LESSON:
                currentPlay = 2;
                break;
        }
        audioPlayer.clip = audioLists[currentPlay];
        audioPlayer.Play();
     //   Debug.Log("audio play finish");

    }
}
