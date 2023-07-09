using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager prCameraManager;
    private Dictionary<string, bool> cameraStates;
   [SerializeField] private Cinemachine.CinemachineVirtualCamera objectVcam;

    private string[] state_keys = { "DoPlayareaFocus", "DoObjectFocus" }; 
    public static CameraManager instance
    {
        get
        {
            if (!prCameraManager)
            {
                prCameraManager = FindObjectOfType<CameraManager>();
                if (!prCameraManager)
                {
                    Debug.LogWarning("There needs to be one active EventManger script on a GameObject in your scene.");
                }
            }
            else
            {
                prCameraManager.Init();
            }

            return prCameraManager;
        }
    }

    private void Init()
    {
        if (cameraStates == null)
        {
            cameraStates = new Dictionary<string, bool>();

            foreach (String key in state_keys)
            {
                cameraStates.Add(key, false);

            }
        }
    }

    public static void CameraFocusPlayArea(bool focus)
    {
    //   Debug.Log("Change to playarea " + focus);
        instance.SetFocusState("DoPlayareaFocus", focus);
        instance.GetComponent<Animator>().SetBool("DoPlayareaFocus", focus);
    }

    public static void CameraZoomAndMove(GameObject focusObject,bool focus)
    {
        if (focusObject) {
            instance.objectVcam.Follow = focusObject.transform;
        }
        instance.SetFocusState("DoObjectFocus", focus);
        instance.GetComponent<Animator>().SetBool("DoObjectFocus", focus);
    }
    private void SetFocusState(string state, bool focus) {
        foreach (String key in state_keys)
        {
            instance.cameraStates[key]=false;
        }
        if (focus) {
            instance.cameraStates[state] = focus;
        }
    }
}
