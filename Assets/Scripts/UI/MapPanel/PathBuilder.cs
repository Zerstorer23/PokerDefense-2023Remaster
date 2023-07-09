/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ConstantStrings;

public class PathBuilder : MonoBehaviour
{
    [SerializeField] GameObject marker;
    [SerializeField] Transform Right, Top;
    SpriteRenderer[,] markers;
    [SerializeField]    PolygonCollider2D myCursor;

    [SerializeField] Tilemap tilemap;


    float startXoffset;
    float startYoffset;
    float stepSizeX = 0.5f;
    float stepSizeY = 0.5f;

    [SerializeField] int width, height;
    TileType[,] board;

    bool init = false;
    // Start is called before the first frame update
    void Start()
    {
         width = GetLength(0f, Right.localPosition.x , stepSizeX);
        height = GetLength(0f, Top.localPosition.y , stepSizeY);
        startXoffset = transform.position.x;
        startYoffset = transform.position.y;

        board = new TileType[width,height];
        markers = new SpriteRenderer[width, height];

 
         SearchTileMap();
    
    }


*//*    private void Update()
    {
        if(!init)InitMarkers();

        ShowTileMap();
    }
    *//*
    
    IEnumerator SearchTileMapDelay()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 searchPosition = BoardToMap(x, y);
                myCursor.transform.position = searchPosition;
                board[x, y] = DetectTile();
                yield return new WaitForFixedUpdate();
            }
        }
    }


    void ShowTileMap() {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (markers[x, y] == null) { Debug.Log(x + "," + y + " = null"); continue; }
               // Debug.Log(x + "," + y + " = " + board[x, y]);
                if (board[x, y] == TileType.ROAD )
                {
                    markers[x, y].color = Color.red;
                }
                else
                {
                    markers[x, y].color = Color.white;
                }
            }
        }

    }


    internal void ResetMap() {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Debug.Log(x + "," + y + " = " + board[x, y]);
                if (board[x, y] == TileType.UNIT)
                {
                    board[x, y] = TileType.ROAD;
                }
            }
        }
    }

 
    TileType DetectTile() {
        // bool touching = myCursor.bounds.Intersects(moveAreaCollider.bounds);
        bool touchingM = myCursor.IsTouchingLayers(LayerMask.GetMask("MoveArea"));
        Debug.Log(myCursor.transform.position + " = " + touchingM);
        return (touchingM)? TileType.ROAD : TileType.INVALID;

    }
    void InitMarkers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 searchPosition = BoardToMap(x, y);
                GameObject obj = Instantiate(marker, searchPosition, Quaternion.identity);
                markers[x, y] = obj.GetComponent<SpriteRenderer>();
            }
        }
        init = true;
    }

    Vector2[] ephemerals = { new Vector2(0f, 0f) , new Vector2(-0.5f, 0f),  new Vector2(0.5f, 0f), new Vector2(0f, 0.5f) };

    void SearchTileMap()
    {
        List<Vector3> cells = GetCellsFromTilemapWorld(tilemap);
        foreach (Vector3 cell in cells)
        {
            foreach (Vector2 ex in ephemerals) {

                int[] pos = MapToBoard(cell.x+ex.x, cell.y+ex.y);
                int x = pos[0];
                int y = pos[1];
                if (x <= 0 || y < 0 || x >= width || y >= height) continue;
                board[x, y] = TileType.ROAD;
            }
        }
    }

  
    List<Vector3Int> GetCellsFromTilemapInt(Tilemap tilemap)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        foreach (var boundInt in tilemap.cellBounds.allPositionsWithin)
        {
            //Get the local position of the cell
            Vector3Int relativePos = new Vector3Int(boundInt.x, boundInt.y, boundInt.z);
            //Add it to the List if the local pos exist in the Tile map
            if (tilemap.HasTile(relativePos))
                cells.Add(relativePos);
        }
        return cells;
    }
    List<Vector3> GetCellsFromTilemapLocal(Tilemap tilemap)
    {
        List<Vector3> worldPosCells = new List<Vector3>();
        foreach (var boundInt in tilemap.cellBounds.allPositionsWithin)
        {
            //Get the local position of the cell
            Vector3Int relativePos = new Vector3Int(boundInt.x, boundInt.y, boundInt.z);
            //Add it to the List if the local pos exist in the Tile map
            if (tilemap.HasTile(relativePos))
            {
                //Convert to world space
                Vector3 localPos = tilemap.CellToLocal(relativePos);
                worldPosCells.Add(localPos);
            }
        }
        return worldPosCells;
    }
    List<Vector3> GetCellsFromTilemapWorld(Tilemap tilemap)
    {
        List<Vector3> worldPosCells = new List<Vector3>();
        foreach (var boundInt in tilemap.cellBounds.allPositionsWithin)
        {
            //Get the local position of the cell
            Vector3Int relativePos = new Vector3Int(boundInt.x, boundInt.y, boundInt.z);
            //Add it to the List if the local pos exist in the Tile map
            if (tilemap.HasTile(relativePos))
            {
                //Convert to world space
                Vector3 worldPos = tilemap.CellToWorld(relativePos);
                worldPosCells.Add(worldPos);
            }
        }
        return worldPosCells;
    }

    internal void Occupy(TileType UnitType, Vector2 target)
    {
        int[] pos = MapToBoard(target.x, target.y);
        if (board[pos[0], pos[1]] != TileType.ROAD) return;
        board[pos[0], pos[1]] = UnitType;
    }
    internal void UnOccupy(Vector3 previous)
    {
        try
        {
            int[] pos = MapToBoard(previous.x, previous.y);
            if (board[pos[0], pos[1]] == TileType.UNIT)
            board[pos[0], pos[1]] = TileType.ROAD;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    Vector2 BoardToMap(float x, float y) {
        return new Vector2((x * stepSizeX ) +startXoffset, (y * stepSizeY ) + startYoffset);
    }

    int[] MapToBoard(float x, float y) {
        int xPos = (int)((x - startXoffset ) / stepSizeX);
        int yPos = (int)((y - startYoffset) / stepSizeY);
        return new int[2] { xPos, yPos };
    }

 
    // Update is called once per frame
    public TileType GetTileAt(int x, int y) {
        return board[x, y];
    }
    public bool IsEmptyAt(float x, float y)
    {
        int[] boardPos = MapToBoard(x, y);
        if (boardPos[0] < 0 || boardPos[0] >= width ||
            boardPos[1] < 0 || boardPos[1] >= height)
            return false;

        return board[boardPos[0], boardPos[1]] ==TileType.ROAD;
    }

    int GetLength(float start, float end ,float stepSize) {
        float w = end - start ;
        int num = (int)(w / stepSize);
        return Mathf.Abs(num);
    }
    
}

public enum TileType
{
    INVALID,ROAD,UNIT,PLACE
}*/