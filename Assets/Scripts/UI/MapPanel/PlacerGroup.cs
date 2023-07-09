
using UnityEngine;
using static ConstantStrings;

public class PlacerGroup : MonoBehaviour
{
    [SerializeField] bool canPlace = false;
    CircleCollider2D cursorCollider;
    Vector2 oldPos;

    bool isShowingMap = true;
    public MapInitialiser mapInfo;

    bool init = false;
    [SerializeField] float multiFactor = 2f;
     [SerializeField] SpriteRenderer rangeSprite;
    SpriteRenderer cursorSprite;

    private bool doNotUpdateCursorGraphic;

    private void Awake()
    {
     //   Debug.Log("placer init srtart");
        if (init) return;
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        EventManager.StartListening(MyEvents.EVENT_CURSOR_RADIUS_REQUESTED, OnRadiusEventReceived);
        cursorCollider = GetComponent<CircleCollider2D>();
        cursorSprite = GetComponent<SpriteRenderer>();
        init = true;
       // Debug.Log("placer init end");
    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        EventManager.StopListening(MyEvents.EVENT_CURSOR_RADIUS_REQUESTED, OnRadiusEventReceived);
    }

    private void OnPanelChanged(EventObject eo)
    {
        isShowingMap = eo.screenType == ScreenType.MAP;
    }

    void Update()
    {
       if(!doNotUpdateCursorGraphic) UpdateGraphic();
        Vector2 newPos = getTileCenterPosFromMouse();
        if (oldPos != newPos)
        {
             UpdateCursor(newPos);
        }

    }
    

    public Vector2 GetMousePosition() {
        return transform.position;
    }

    private void UpdateCursor(Vector2 newPos)
    {
        transform.position = newPos;
        oldPos = newPos;
    }
    private void UpdateGraphic() {
        canPlace = false;//Mutex
        bool isEmpty = false;
        bool isTouchPlace = cursorCollider.IsTouchingLayers(LayerMask.GetMask(STRING_LAYER_PLACING_AREA));
        bool isTouchSpawnblock = cursorCollider.IsTouchingLayers(LayerMask.GetMask("SpawnBlockers"));

        //Turn function on / off

        if (!isShowingMap) {
            canPlace = false;
        } else if (isTouchPlace && !isTouchSpawnblock) {
            isEmpty = mapInfo.CheckPositionIsValid(transform.position + new Vector3(0,-0.5f), Owner.NAMCO);
            canPlace = isEmpty;
        }
  //      Debug.Log("place " + isTouchPlace + " spawnblock " + isTouchSpawnblock + " isEmpty " + isEmpty);

        //Turn visibility on / off
        ChangeCursorVisibility(isTouchPlace);
        ChangeCursorColor(isTouchPlace &&!isTouchSpawnblock && isEmpty);


    }
    public void ChangeCursorVisibility(bool visibility) {
        if(cursorSprite.enabled != visibility)
        cursorSprite.enabled = visibility;
    }
    public void ChangeCursorColor(bool visibility)
    {
        cursorSprite.color = (visibility) ? Color.green : Color.red;
    }


    public bool CanPlace() {
        return canPlace;
    }
  
    private Vector3 getTileCenterPosFromMouse()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.y = mousePos.y * 2;
        int mPosX = Mathf.FloorToInt(mousePos.x);
        int mPosY = Mathf.FloorToInt(mousePos.y);

        // 홀짝 구분 / 노출되는 타일들의 중앙점은 합이 짝수다.
        int checkEven = ((mPosX + mPosY) & 1);

        // 홀수라면 remainX에 곱하여 양수로 바꿔준다.
        int tempEven = (checkEven * -2) + 1;

        // 홀수라면 짝수로 기준센터 이동.
        mPosX += checkEven;

        float remainX = mousePos.x - mPosX;
        float remainY = mousePos.y - mPosY;

        // 소수점 이하의 수를 더하여, 1보다 크면 이동.
        float remainSum = (tempEven * remainX) + remainY;
        // 더한 값을 내림하여, 1.0 이상이면1로 만들어 최종 계산식에 사용.
        int floorSum = Mathf.FloorToInt(remainSum);

        // 더한 값이 1.0 이상이고, checkEven이 짝수라면 x+1,y+1  (floorSum = 1 , tempEven = 1)
        // 더한 값이 1.0 이상이고, checkEven이 홀수라면 x-1,y+1  (floorSum = 1 , tempEven = -1)
        // 더한 값이 1.0 이하라면, x,y  (floorSum = 0)
        Vector3 result = new Vector3(mPosX + (floorSum * tempEven) , (mPosY + floorSum) * 0.5f );

        //Debug.Log("_mousePos = " +_mousePos + " mPosX = " + mPosX + " mPosY = " + mPosY + " checkEven = " + checkEven + " tempEven = " + tempEven + " remainX = " + remainX + " remainY = " + remainY + " remainSum = " + remainSum +" FloorSum = " + floorSum + " result = " + result);

//        Debug.Log(mousePos.x+","+mousePos.y/2 + " -> " + result);
        return result;
    }


    public void Activate(bool active )
    {
        gameObject.SetActive(active);
        //gameObject.transform.localPosition = Vector3.zero;
    }
    public void SetRadius(float radius) {
        if (radius <= 0f)
        {

            rangeSprite.enabled = false;
        }
        else
        {
            rangeSprite.enabled = true;
            rangeSprite.transform.localScale = new Vector2(radius * multiFactor, radius * multiFactor);
        }
    }

    private void OnRadiusEventReceived(EventObject eo) {
        float r = eo.GetFloat();
        if (r > 0)
        {
            Activate(true);
            ChangeCursorVisibility(false);
            doNotUpdateCursorGraphic =true;
            SetRadius(eo.GetFloat());
        }
        else
        {
            ChangeCursorVisibility(true);
            doNotUpdateCursorGraphic = false;
            Activate(false);
        }

    }

}
