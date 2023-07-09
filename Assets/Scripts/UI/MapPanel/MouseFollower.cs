using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    bool doFollow = false;
    [SerializeField]float moveSpeed = 6f;
    [SerializeField] Cinemachine.CinemachineVirtualCamera vcam;
    [SerializeField] float y_boundary_perc = 0.1f;
    [SerializeField] float x_boundary_perc = 0.1f;
    [SerializeField] bool debug = false;
    [SerializeField] bool fixCamera = true;

    [SerializeField] Cinemachine.CinemachineConfiner confiner;

    float orthographicSize;
    float widthMax, heightMax;

    float Nbound, Sbound, Lbound, Rbound;

   // bool moveDown = false, moveUp = false, moveLeft = false, moveRight = false;
    IEnumerator moveUp, moveDown, moveLeft, moveRight;
/*    bool isMovingWithKey = false;

    Vector3 prevPos;*/

    //screen size
    //-9 ~ 9 = x = ortho / 5 * 9
    // y : -n ~ n, n = orthographic size
    private void Awake()
    {
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnPanelChanged);

        SetBounds();


    }
    private void OnDestroy()
    {

        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnPanelChanged);
    }


    private void SetBounds()
    {
        orthographicSize = vcam.m_Lens.OrthographicSize;
        widthMax = orthographicSize * 1.8f;
        heightMax = orthographicSize;
        if (debug) Debug.Log("Predicted x " + widthMax + " y " + heightMax);
        Nbound = heightMax - heightMax * y_boundary_perc;
        Sbound = -Nbound;
        Rbound = widthMax - widthMax * x_boundary_perc;
        Lbound = -Rbound;
        if (debug) Debug.Log("Calculated bound x " + Nbound + " y " + Rbound);
    }
 
/*    void Update()
    {
        if (!doFollow || fixCamera ) return;
        DetectKeys();
        if(!isMovingWithKey) DetectMouseMove();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (confiner.CameraWasDisplaced(vcam))
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                // worldPos = new Vector3(worldPos.x, worldPos.y, -10f);

                    vcam.transform.position = worldPos;
            }
        }
          
    }
*/

  
    private void DetectMouseMove()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 relativePosition = vcam.transform.InverseTransformDirection(worldPos - Camera.main.transform.position);
        Vector3 direction = Vector3.zero;

        if (relativePosition.x <= Lbound)
        {
            if (debug) Debug.Log(relativePosition + " is in range Left " + Lbound);
            direction += Vector3.left;

        }
        else if (relativePosition.x >= Rbound)
        {
            if (debug) Debug.Log(relativePosition + " is in range Right " + Rbound);
            direction += Vector3.right;

        }



        if (relativePosition.y <= Sbound)
        {
            if (debug) Debug.Log(relativePosition + " is in range DOwn " + Sbound);
            direction += Vector3.down;

        }
        else if (relativePosition.y >= Nbound)
        {
            if (debug) Debug.Log(relativePosition + " is in range Up " + Nbound);
            direction += Vector3.up;

        }

        vcam.transform.position += direction * Time.deltaTime * moveSpeed;

        
    }
/*
    private void DetectKeys()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(moveUp);
            isMovingWithKey = true;
        }
         if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(moveDown);
            isMovingWithKey = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(moveLeft);
            isMovingWithKey = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(moveRight);
            isMovingWithKey = true;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            StopCoroutine(moveUp);
            isMovingWithKey = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StopCoroutine(moveDown);
            isMovingWithKey = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            StopCoroutine(moveLeft);
            isMovingWithKey = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StopCoroutine(moveRight);
            isMovingWithKey = false;
        }
    }*/
    IEnumerator MoveContinously(KeyCode key)
    {
 
        while (true)
        {
            Vector3 direction = Vector3.zero;
            switch (key)
            {
                case KeyCode.W:
                     direction = Vector3.up; 
                    break;
                case KeyCode.S:
                    direction = Vector3.down;
                    break;
                case KeyCode.A:
                     direction = Vector3.left;
                    break;
                case KeyCode.D:
                 direction = Vector3.right;
                    break;
            }
     
            vcam.transform.position += direction * Time.deltaTime * moveSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }



    void OnPanelChanged(EventObject eo) {
        doFollow = eo.screenType == ScreenType.MAP;
    }
}
