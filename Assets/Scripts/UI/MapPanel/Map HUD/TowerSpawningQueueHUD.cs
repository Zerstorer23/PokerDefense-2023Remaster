using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSpawningQueueHUD : MonoBehaviour
{

    [SerializeField] GameObject ContentContainer;
    [SerializeField] GameObject TowerButtonPrefab;
    [SerializeField] Button sellButton;
    [SerializeField] Button reserveButton;
    //   [SerializeField] ToggleGroup toggleGroup;
    bool init = false;

    TowerSpawner towerSpawner = null;



    private void Awake()
    {
    //    Debug.Log("queue init srtart");
        if (init) return;
        //EventManager.enemyDiedEvent.AddListener(RemoveEnemy);
        EventManager.StartListening(MyEvents.EVENT_OPEN_TOWER_SPAWN_SELECTOR_HUD, ShowPanel);
        EventManager.StartListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseUserChanged);
        init = true;
    //    Debug.Log("queue init end");
        // FindSpawner();
    }
    private void Start()
    {
        towerSpawner = GameSession.GetGameSession().towerSpawner;
    }

    private void OnMouseUserChanged(EventObject arg0)
    {
        MouseUser newUser = (MouseUser)arg0.GetInt();
        if (newUser == MouseUser.RESERVE_SPAWNER) {
            Hide();
        } else if (newUser == MouseUser.NONE && towerSpawner.towerSpawnPool.Count > 0 ) {
            Show();
        }

    }

    private void OnDestroy()
    {

        EventManager.StopListening(MyEvents.EVENT_OPEN_TOWER_SPAWN_SELECTOR_HUD, ShowPanel);
        EventManager.StopListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseUserChanged);
    }


    public GameObject AddButton(UnitConfig config) {
        GameObject towerButton = Instantiate(TowerButtonPrefab, transform.position, Quaternion.identity);
        towerButton.GetComponent<TowerSpawningButton>().SetInfo(config);
       // towerButton.GetComponent<Toggle>().group = toggleGroup;
        towerButton.transform.SetParent(ContentContainer.transform,false);
        return towerButton;
    }

    public void SetExtraOptionsVisibility(bool enable) {
        sellButton.gameObject.SetActive(enable);
        reserveButton.gameObject.SetActive(enable && ReserveManager.HasEmptySpot());

    }

    private void FindSpawner() {
        if (towerSpawner == null) {
            towerSpawner = FindObjectOfType<TowerSpawner>();
        }
    
    }
    private void ShowPanel(EventObject lexington) {
       // SetExtraOptionsVisibility(false);
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        GetComponent<RectTransform>().DOLocalMoveY(-340f, 0.1f).OnComplete(
            () => {
               //
            }
        ); ;
    }


    public void Hide()
    {
        GetComponent<RectTransform>().DOLocalMoveY(-540f, 0.1f).OnComplete(
            () =>
            {
                gameObject.SetActive(false);
            }

        );
    }



}
