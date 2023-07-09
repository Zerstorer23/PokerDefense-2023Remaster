using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInitialiser : MonoBehaviour
{
    internal TowerSpawner towerSpawner;
    public Transform mapHomeTransform;
    public Tilemap NamcoTiles;
    public Tilemap KuroiTiles;
    public int tileXcount = 12;
    public int tileYcount = 8;




    public Tower[,] towerOccupiedMap;
    public Owner[,] towerOwnerMap;
    public int MaxNamcoTower = 0;
    public int MaxKuroiTower = 0;

    internal Vector2 map_stepX = new Vector3(1, 0.5f);
    internal Vector2 map_stepY = new Vector3(-1, 0.5f);
    internal Vector2 map_home;

   [SerializeField] ConstructionArea[] constructionBlockers;

    private void Awake()
    {
      //  Debug.Log("map init start");
        towerSpawner = GetComponent<TowerSpawner>();

        BuildTowerMap();
      //  Debug.Log("map init end");
    }
    public void ResetMap() {

        for (int x = 0; x < towerOccupiedMap.GetLength(0); x++)
        {
            for (int y = 0; y < towerOccupiedMap.GetLength(1); y++)
            {
                towerOccupiedMap[x, y] = null;
            }
        }
    }
    public bool HasEmptySpace()
    {
        int namcoCount = 0;
        foreach (Tower t in towerSpawner.GetMyTowers().Values)
        {
            if (t.owner == Owner.NAMCO)
            {
                namcoCount++;
            }
        }
        int finalMax = MaxNamcoTower;
        foreach (ConstructionArea c in constructionBlockers) {
            if (c != null) {
                finalMax -= 6;
            }
        }

        return (namcoCount < finalMax);
    }

    internal void AddTowerOnMap(Tower t)
    {
        int[] boardPos = ConvertMapToBoard(t.gameObject.transform.localPosition);
        towerOccupiedMap[boardPos[0], boardPos[1]] = t;
        t.mapPosition = new Vector3(boardPos[0], boardPos[1]);
    }
    internal int[] ConvertMapToBoard(Vector2 mapPos)
    {
       // Debug.Log("Received pos " + mapPos);
        for (int x = 0; x < towerOccupiedMap.GetLength(0); x++)
        {
            for (int y = 0; y < towerOccupiedMap.GetLength(1); y++)
            {
                Vector2 compPos = (map_stepX * x + map_stepY * y) + map_home;
             //   Debug.Log(mapPos + " vs " + compPos);
                if (mapPos == compPos)
                {
                    return new int[] { x, y };
                }
            }
        }
     //   Debug.LogWarning("Critical position Error at"+ mapPos);
        return null;
    }
    private void BuildTowerMap()
    {
        map_home = mapHomeTransform.localPosition;
        towerOccupiedMap = new Tower[tileXcount, tileYcount];
        towerOwnerMap = new Owner[tileXcount, tileYcount];
        List<Vector3> kuroiTiles = GetCellsFromTilemapWorld(KuroiTiles);
        foreach (Vector3 v in kuroiTiles)
        {
            int[] boardPos = ConvertMapToBoard(v);
            towerOwnerMap[boardPos[0], boardPos[1]] = Owner.KUROI;
        }
        List<Vector3> namcoTiles = GetCellsFromTilemapWorld(NamcoTiles);
        foreach (Vector3 v in namcoTiles)
        {
            int[] boardPos = ConvertMapToBoard(v);
            towerOwnerMap[boardPos[0], boardPos[1]] = Owner.NAMCO;
        }

        MaxNamcoTower = namcoTiles.Count;
        MaxKuroiTower = kuroiTiles.Count;

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
                //   Debug.Log("Found " + worldPos);
            }
        }
        return worldPosCells;
    }

    public Vector3 GetValidPosition(Owner requestedOwner)
    {
        for (int x = towerOccupiedMap.GetLength(0) - 1; x >= 0; x--)
        {
            for (int y = 0; y < towerOccupiedMap.GetLength(1); y++)
            {
                if (towerOccupiedMap[x, y] == null
                    && towerOwnerMap[x, y] == requestedOwner)
                {
                    return new Vector3(x, y);
                }
            }
        }
        return Vector3.back;
    }

   public bool CheckPositionIsValid(Vector3 mapPosition, Owner owner) {
        int[] boardPos = ConvertMapToBoard(mapPosition);
        if (boardPos == null) return false;

        if (towerOccupiedMap[boardPos[0], boardPos[1]] != null) return false;
        if (towerOwnerMap[boardPos[0], boardPos[1]] != owner) return false;
        return true;
    
    }
}
