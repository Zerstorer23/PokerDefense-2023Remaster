using System;
using UnityEngine;
public class ClickManager : MonoBehaviour
{
   [SerializeField] internal MouseUser currentUser = MouseUser.NONE;
    private static ClickManager prClickManager;

    public static ClickManager instance
    {
        get
        {
            if (!prClickManager)
            {
                prClickManager = FindObjectOfType<ClickManager>();
                if (!prClickManager)
                {
                    Debug.LogWarning("There needs to be one active EventManger script on a GameObject in your scene.");
                }
            }
                return prClickManager;
        }
    }



    public static void ToggleUser(MouseUser userType, bool enable)
    {
     //   Debug.Log("Request user " + userType + " = " + enable);
        if (enable)
        {
            instance.SetUser(userType);
        }
        else
        {
            instance.StopUsing(userType);
        }
        EventManager.TriggerEvent(MyEvents.EVENT_MOUSE_USER_CHANGED, new EventObject((int)GetCurrentUser()));
    }
    public static MouseUser GetCurrentUser() {
        return instance.currentUser;
    }
    private void SetUser(MouseUser userType)
    {
        Debug.Assert(instance.currentUser == MouseUser.NONE || instance.currentUser == userType, "set - User mismatch! curruser " + instance.currentUser + " => requested user " + userType);
        instance.currentUser = userType;
        if (instance.currentUser == MouseUser.RESERVE_SPAWNER
            || instance.currentUser == MouseUser.TOWER_SPAWNER
            || instance.currentUser == MouseUser.RELOCATOR
            ) {
           CameraManager.CameraFocusPlayArea(true);
        }
    }

    private void StopUsing(MouseUser userType)
    {
        Debug.Assert(instance.currentUser == userType, "stop - User mismatch! curruser "+instance.currentUser+" => requested user "+userType);
        instance.currentUser = MouseUser.NONE;
        CameraManager.CameraFocusPlayArea(false);
    }

    public static bool IsNone()
    {
        return instance.currentUser == MouseUser.NONE;
    }


}

public enum MouseUser { 
    NONE,TOWER_SPAWNER,TOWER,ACTIVE_SKILL,RESERVE_SPAWNER,RELOCATOR
}
