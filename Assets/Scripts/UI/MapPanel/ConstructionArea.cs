using UnityEngine;

public class ConstructionArea : MonoBehaviour
{
    public SpriteRenderer focusSprite;

    private void OnMouseDown()
    {
        /*        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);*/
        focusSprite.enabled = true;
        EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PURCHASE_OPTION, new EventObject(gameObject));
    }

}
