using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUI : MonoBehaviour
{
  public static bool backgroundTouch = false;
  public static int numTouch = 0;
    private void OnMouseDown()
    {
        backgroundTouch = true;
        numTouch++;
      //  Debug.Log("Backgroun Touch Down " + numTouch + " / " + backgroundTouch);
    }
    private void OnMouseUp()
    {
        numTouch--;
        if (numTouch <= 0) backgroundTouch = false;
     //   Debug.Log("Backgroun Touch Up " + numTouch + " / " + backgroundTouch);
        // Debug.Log("Background area up");
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        EventManager.TriggerEvent(MyEvents.EVENT_BACKGROUND_CLICKED, new EventObject(worldPos));
    }
}
