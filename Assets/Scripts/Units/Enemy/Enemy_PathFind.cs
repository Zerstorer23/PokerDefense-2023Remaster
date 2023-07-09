using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;
using Random = UnityEngine.Random;

public class Enemy_PathFind : MonoBehaviour
{

    Transform[,] waypoints;
    [SerializeField] GameObject parentBody;
    [SerializeField] internal bool debugOut = false;
    [SerializeField] internal bool doMove = false;
    [SerializeField] SpriteRenderer[] environment_probe;

    Enemy_Main mainBody;
    //*************
    //유닛정보
    internal double initial_moveSpeed = 6f;
    [SerializeField] double moveSpeed;


    //***********

    [SerializeField] int destinationDirection = 0;
    int wIndex_first = 0;
    int wIndex_second = 0;


    double[] distSpace;
    float[,] directions = new float[8, 2] { { -0.353f, 0.353f },    { 0, 0.5f },    { 0.353f, 0.353f },
                                            { -0.5f, 0 },                       { 0.5f, 0 }, 
                                            { -0.353f, -0.353f },   {0, -0.5f },    { 0.353f, -0.353f }};

    //    internal PathBuilder pathBuilder;
    BuffManager buffManager;

   internal Transform jimusyo;
    bool headJimusyo = false;
    private void Awake()
    {
     //   Debug.Log("enpath init srtart");
        buffManager = GetComponent<BuffManager>();
     //   Debug.Log("enpath init end");
    }
    private void OnEnable()
    {
        EventManager.StartListening(MyEvents.EVENT_WAVE_TIMEOUT, OnTimeOut);
        mainBody = GetComponent<Enemy_Main>();
        headJimusyo = false;
        wIndex_first = 0;
        wIndex_second = 0;
        destinationDirection = 0;
        mainBody.mainBodySprite.transform.localScale = new Vector3(1, 1, 1);
    }
    private void OnDisable()
    {

        EventManager.StartListening(MyEvents.EVENT_WAVE_TIMEOUT, OnTimeOut);
    }

    private void OnTimeOut(EventObject arg0)
    {
        moveSpeed = initial_moveSpeed;
        headJimusyo = true;
    }

    private void Start()
    {
        SetInitialSpeed();
        distSpace = new double[8];

/*        if (pathBuilder == null) {
           pathBuilder= FindObjectOfType<PathBuilder>();
        }*/
        wIndex_second = Random.Range(0, waypoints.GetLength(1));
        UpdateFacingDirection();
    }


    internal void SetInitialSpeed()
    {
        this.moveSpeed = initial_moveSpeed;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (!doMove || buffManager.numStun > 0) return;
        if (headJimusyo)
        {
            var target = Vector2.MoveTowards(parentBody.transform.position, jimusyo.position, (float)moveSpeed * (float)buffManager.speedModifier * Time.deltaTime);
            parentBody.transform.position = target;
            double distance = Vector2.Distance(jimusyo.position, transform.position);
            if (distance < 0.8) {
                GetComponent<Enemy_Main>().KillMe(false);
            }
        }
        else
        {
            DoMove();
        }


    }

    private void DoMove()
    {
        EvaluateMoves();
        if (debugOut) ShowEmptyTiles();
        var nextPoint = waypoints[wIndex_first, wIndex_second].position;
            //GetBestMove();
        MoveTowards(nextPoint);

        if (CheckWaypoint())
        {
            UpdateWaypoint();
        }
    }

    private void MoveTowards(Vector2 heading)
    {
        var previous = transform.position;
     //   pathBuilder.UnOccupy(previous);
        var target = Vector2.MoveTowards(parentBody.transform.position, heading, (float)(moveSpeed * buffManager.speedModifier) * Time.deltaTime);
      //  if (debugOut) Debug.Log("Moving from " + transform.position+" to "+ target+" with speed "+(moveSpeed * speedModifier * Time.deltaTime) + "/ms "+moveSpeed+" /sm "+speedModifier+" / tdt"+Time.deltaTime);
        parentBody.transform.position = target;
     //   pathBuilder.Occupy(TileType.UNIT, target);
    }

    private void UpdateWaypoint()
    {
        wIndex_first = (wIndex_first + 1) % waypoints.GetLength(0);
        wIndex_second = Random.Range(0, waypoints.GetLength(1));
        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        float xDir = 1f;
        if (waypoints[wIndex_first,wIndex_second].transform.position.x > transform.position.x)
        {
            xDir = -1f;
        }
        mainBody.mainBodySprite.transform.localScale = new Vector3(xDir, 1, 1);
    }

    void EvaluateMoves()
    {
        for (int i = 0; i < distSpace.Length; i++)
        {
            float tarX = transform.position.x + directions[i, 0];
            float tarY = transform.position.y + directions[i, 1];

            //    bool isValid =  pathBuilder.IsEmptyAt(tarX, tarY);
            // if(debugOut)

            // distSpace[i] = (isValid) ? GetDistance(new Vector2(tarX, tarY), waypoints[wIndex_first,wIndex_second].position) : -1d;
            distSpace[i] = Vector2.Distance(new Vector2(tarX, tarY), waypoints[wIndex_first, wIndex_second].position);
            if (debugOut) {
                Debug.Log((DEBUG_DIRECTION)i + " = " + distSpace[i]);
            }
            //Debug.Log(isTouching[i]);
        }
    }

    private Vector2 GetBestMove() {
        int minIndex = 0;
        bool existValidMove = false;

        for (int i = 0 ; i < distSpace.Length; i++) {
            if (distSpace[i] >= 0) {
                if (!existValidMove)
                {
                    minIndex = i;
                    existValidMove = true;
                }
                if (distSpace[i] < distSpace[minIndex])
                    {
                        minIndex = i;
                        existValidMove = true;
                    }

            }
        }

        if (!existValidMove ) {
            if (debugOut) Debug.Log("No move available");
            return transform.position;
        }
        if (debugOut) Debug.Log("Chose " +(DEBUG_DIRECTION)minIndex);
        return transform.position+new Vector3(directions[minIndex,0], directions[minIndex, 1],0); 
    }
    private void OnDestroy()
    {
     //  if(!headJimusyo) pathBuilder.UnOccupy(transform.position);
    }

    private bool CheckWaypoint()
    {
        double distance = Vector2.Distance(waypoints[wIndex_first,wIndex_second].position, transform.position);
        if(debugOut) Debug.Log(wIndex_first+" ~ Distance: " +distance);
        return distance <= 0.25;
    }


    public void UpdateDestination() {
        destinationDirection = (destinationDirection+1) % 4;
    }

    internal void SetPathInfo(GameObject[] paths) {
        waypoints = new Transform[paths.Length, 3];
        for (int i = 0; i < paths.Length; i++) {
            GameObject pathSet = paths[i];
            int wIndex = 0;
            foreach (Transform t in pathSet.transform)
            {
                waypoints[i, wIndex++] = t;
            }

        }

    } 
    private void ShowEmptyTiles()
    {
        for (int i = 0; i < distSpace.Length; i++)
        {
            environment_probe[i].enabled = distSpace[i] >= 0;
        }
    }

    internal double GetMoveSpeed() { return moveSpeed; }
}
public enum DEBUG_DIRECTION { 
NW = 0, N =1, NE =2, W=3,E=4, SW=5,S=6,SE=7

}