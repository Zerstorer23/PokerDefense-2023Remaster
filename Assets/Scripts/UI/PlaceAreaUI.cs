using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAreaUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isPlaceArea = true;
    [SerializeField] bool debugOut = false;
    [SerializeField] bool enable = true;
    private void OnMouseUp()
    {
        if (!enable) return;
       if(debugOut) Debug.Log("Play area up");
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        EventManager.TriggerEvent(MyEvents.EVENT_BACKGROUND_CLICKED, new EventObject(worldPos));
        if (isPlaceArea)
        {

            EventManager.TriggerEvent(MyEvents.EVENT_PLACE_AREA_CLICKED, new EventObject(worldPos));
        }
    }
}
