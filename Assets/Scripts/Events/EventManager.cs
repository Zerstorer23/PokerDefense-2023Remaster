﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventManager : MonoBehaviour
{
    private static EventManager prEvManager;

    public static EventManager eventManager
    {
        get
        {
            if (!prEvManager)
            {
                prEvManager = FindObjectOfType<EventManager>();
                if (!prEvManager)
                {
                  //  prEvManager = Instantiate(EventManager) as EventManager;
                  
                  // prEvManager = FindObjectOfType<GameSession>().GetComponent<EventManager>();
                    // Debug.LogWarning("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    prEvManager.Init();
                }
            }

            return prEvManager;
        }
    }
    private Dictionary<string, EventOneArg> eventDictionary;

    void Init() {

        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, EventOneArg>();
        }
    }


    public EventOneArg GetEvent(string eventName) {

        EventOneArg thisEvent = null;
        eventDictionary.TryGetValue(eventName, out thisEvent);
        return thisEvent;
//       bool found= eventDictionary.TryGetValue(eventName,out thisEvent);

    }
    public void AddEvent(string eventName, EventOneArg thisEvent) {


        eventDictionary.Add(eventName, thisEvent);
    }

    public static void StartListening(string eventName, UnityAction<EventObject> listener)
    {
        if (eventManager == null) return;
        EventOneArg thisEvent = eventManager.GetEvent(eventName);
        if (thisEvent != null)
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new EventOneArg();
            thisEvent.AddListener(listener);
            eventManager.AddEvent(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<EventObject> listener)
    {
        if (eventManager == null) return;
        EventOneArg thisEvent = eventManager.GetEvent(eventName);
        if (thisEvent != null)
        {
            thisEvent.RemoveListener(listener);
        }
    }

/*    private void OnDestroy()
    {
        foreach (KeyValuePair<string,EventOneArg> e in eventDictionary) {
            e.Value.RemoveAllListeners();
        }
    }*/

    public static bool TriggerEvent(string eventName, EventObject variable)
    {
        if (eventManager == null) {
            Debug.LogWarning("On Destroy no EventManager.");
            return false;
        }
        EventOneArg thisEvent =  eventManager.GetEvent(eventName);
        if (thisEvent != null)
        {
            thisEvent.Invoke(variable);
            return true;
        }
        return false;
    }

}




