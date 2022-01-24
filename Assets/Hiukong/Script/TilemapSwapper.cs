using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;


public class TilemapSwapper : MonoSingleton<TilemapSwapper>
{
    #region Type_Decalration
    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
        COUNT,
    };

    public enum Entity
    {
        A,
        B,
        COUNT
    };

    public enum TileType
    {
        PASSABLE,               // Normal passable tile
        WALL,                   // wall
        PORTAL,                 // portal
        ELEVATOR,               // elevator, to next scene/level
        INTERACTABLE,           // interactable, preserved
        COUNT
    };

    private enum TilemapType
    {
        Swapping_A,
        Swapping_B,
        Changing_A,
        Changing_B,
        Changing_A_Shown_In_B,
        Changing_B_Shown_In_A,
        COUNT
    };

    private class Matrix2x2
    {
        private Vector2Int a;
        private Vector2Int b;

        public Matrix2x2()
        {
            this.a = new Vector2Int(1, 0);
            this.b = new Vector2Int(0, 1);
        }
        public Matrix2x2(Vector2Int a, Vector2Int b)
        {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// Returnes result of matrix multiplies this matrix.
        /// </summary>
        public Vector2Int[] LeftMul(Vector2Int[] matrix)
        {
            Vector2Int[] res = new Vector2Int[matrix.Length];

            // Debug.Log(matrix.Length);

            for (int i = 0; i < matrix.Length; i++)
            {
                res[i].x = matrix[i].x * this.a.x + matrix[i].y * this.b.x;
                res[i].y = matrix[i].x * this.a.y + matrix[i].y * this.b.y;
            }

            return res;
        }
    };


    #endregion


    #region Useful_Tools
    static Matrix2x2 shapeOffsetUpToRight = new Matrix2x2(new Vector2Int(0, -1), new Vector2Int(1, 0));
    static Matrix2x2 shapeOffsetUpToDown = new Matrix2x2(new Vector2Int(1, 0), new Vector2Int(0, -1));
    static Matrix2x2 shapeOffsetUpToLeft = new Matrix2x2(new Vector2Int(0, 1), new Vector2Int(-1, 0));
    static Matrix2x2[] shapeOffsetChangeList = { shapeOffsetUpToRight, shapeOffsetUpToDown, shapeOffsetUpToLeft };

    [Header("Gizmos")]
    public bool IsDrawCurrentCellPos = true;
    public bool IsDrawCellCoordinates = true;
    public Vector2Int CellCoordinateRange = new Vector2Int(10, 10);
    #endregion


    #region Varaible_Declaration
    [Header("Tilemaps")]
    [SerializeField]
    [Tooltip("Drop in the grid which those tilemaps are using (their mutual parent node)")]
    private Grid grid;


    [SerializeField]
    [Tooltip("Node containing changing tilemaps for A")]
    private string changingA = "changing A";
    [SerializeField]
    [Tooltip("Node containing changing tilemaps for B")]
    private string changingB = "changing B";
    [SerializeField]
    [Tooltip("Node containing swapping tilemaps for A")]
    private string swappingA = "swapping A";
    [SerializeField]
    [Tooltip("Node containing swapping tilemaps for B")]
    private string swappingB = "swapping B";
    [SerializeField]
    [Tooltip("Node containing changing tilemaps for A, Shown in B's world")]
    private string changingAShownInB = "changing A Shown in B";
    [SerializeField]
    [Tooltip("Node containing changing tilemaps for A, Shown in B's world")]
    private string changingBShownInA = "changing B Shown in A";



    [Header("Entitys")]
    [SerializeField]
    private Transform entityDetectCenterA;
    [SerializeField]
    private Transform entityDetectCenterB;

    [Header("Flashlight Shapes")]
    [SerializeField]
    [Tooltip("Light shape offsets data object.\nUsing MouseRightClick/Create/GGJGameData/LightShape to create one")]
    private LightShape lightShapeA;

    [SerializeField]
    [Tooltip("Light shape offsets data object.\nUsing MouseRightClick/Create/GGJGameData/LightShape to create one")]
    private LightShape lightShapeB;


    // tilemap data
    private TilemapData tilemapData;

    private List<Tilemap> initialTilemaps;
    private List<Tilemap> changingTilemapsA = new List<Tilemap>();
    private List<Tilemap> changingTilemapsB = new List<Tilemap>();
    private List<Tilemap> swappingTilemapsA = new List<Tilemap>();
    private List<Tilemap> swappingTilemapsB = new List<Tilemap>();
    private List<Tilemap> changingTilemapsA_ShownInB = new List<Tilemap>();
    private List<Tilemap> changingTilemapsB_ShownInA = new List<Tilemap>();

    private Dictionary<int, List<Tilemap>> tilemapDict = new Dictionary<int, List<Tilemap>>();
    private Dictionary<int, string> tilemapNameDict = new Dictionary<int, string>();
    private TilemapCanvasPool tilemapCanvasPool = new TilemapCanvasPool();

    // storing flashlight's shape offsets in 4 directions
    private List<Vector2Int[]> shapeAList = new List<Vector2Int[]>();
    private List<Vector2Int[]> shapeBList = new List<Vector2Int[]>();

    // storing tiles changed by A/B
    private HashSet<Vector2Int> tileChangeListA = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> tileChangeListB = new HashSet<Vector2Int>();


    // wall detection
    private Dictionary<int, int> lowerWall = new Dictionary<int, int>();
    private Dictionary<int, int> upperWall = new Dictionary<int, int>();
    private List<Vector2Int> processedOffsets = new List<Vector2Int>();



    #endregion


    #region API

    /// <summary>
    /// Select the grid which contains all the current room's tilemaps
    /// Invoke this after moving to new Grid.
    /// </summary>
    /// <param name="grid">The grid of current room</param>
    /// <param name="initialTilemapEntity">Which world the entity is borne in, default is A's world.</param>
    /// <returnrs>Return old grid</returnrs>
    public Grid SelectTilemaps(Grid grid, Entity initialTilemapEntity = Entity.A)
    {
        Grid old = grid;
        this.grid = grid;
        SelectTilemapData(grid.GetComponent<TilemapData>());
        selectTilemaps(initialTilemapEntity, grid);
        tilemapCanvasPool.SelectGrid(this.grid);
        return old;
    }


    /// <summary>
    /// Select current tilemap's tilemapData
    /// </summary>
    /// <returns>Return used tilemapData</returns>
    public TilemapData SelectTilemapData(TilemapData newTilemapData)
    {
        TilemapData old = this.tilemapData;
        this.tilemapData = newTilemapData;
        return old;
    }

    /// <summary>
    /// *** Important ***
    /// Change Tilemap to its original state.
    /// Invoke this method in level start or level restart.
    /// </summary>
    public void InitializeTilemap()
    {
        compileShapeOffsets();
        clearChangedTile();
        initializeTilemapCanvas();
    }


    /// <summary>
    /// Swap tilemap to specified world's tilemap. 
    /// Invoke this method when using "flashlight".
    /// </summary>
    /// <param name="entity">Specify which entity's world to change</param>
    /// <param name="direction">And in which direction</param>
    /// <param name="isOn">Is the flashlight on?</param>
    public bool ChangeTilemap(Entity entity, Direction direction, bool isOn = true)
    {
        Transform ent;
        List<Tilemap> tilemapChangeTo;
        Vector2Int[] shapeOffsets;
        if (entity == Entity.A)
        {
            ent = entityDetectCenterA;
            tilemapChangeTo = isOn ? changingTilemapsA : swappingTilemapsA;
            shapeOffsets = shapeAList[(int)direction];
        }
        else if (entity == Entity.B)
        {
            ent = entityDetectCenterB;
            tilemapChangeTo = isOn ? changingTilemapsB : swappingTilemapsB;
            shapeOffsets = shapeBList[(int)direction];
        }
        else 
            return false;

        Vector2Int entityCell = (Vector2Int)grid.WorldToCell(ent.transform.position);
        changeTilemap(tilemapChangeTo, entityCell, shapeOffsets, entity, isOn);

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity">Specify which entity's world to change</param>
    /// <param name="offset">The tile's offset from detectCenter cell</param>
    /// <param name="isOn">Is the flashlight on?</param>
    /// <returns></returns>
    public bool ChangeTilemap(Entity entity, Vector2Int offset, bool isOn = true)
    {
        Transform ent;
        List<Tilemap> tilemapChangeTo;
        Vector2Int[] shapeOffsets = { offset };
        if (entity == Entity.A)
        {
            ent = entityDetectCenterA;
            tilemapChangeTo = isOn ? changingTilemapsA : swappingTilemapsA;
        }
        else if (entity == Entity.B)
        {
            ent = entityDetectCenterB;
            tilemapChangeTo = isOn ? changingTilemapsB : swappingTilemapsB;
        }
        else
            return false;

        Vector2Int entityCell = (Vector2Int)grid.WorldToCell(ent.transform.position);
        changeTilemap(tilemapChangeTo, entityCell, shapeOffsets, entity, isOn);

        return true;
    }


    /// <summary>
    /// Swap tilemap to specified world's tilemap. 
    /// Invoke this method when using "flashlight".
    /// </summary>
    /// <param name="entity">The detect center transform of the entity</param>
    /// <param name="direction">And in which direction</param>
    /// <returns></returns>
    public bool ChangeTilemap(Transform detectCenter, Direction direction, bool isOn = true)
    {
        if (detectCenter == entityDetectCenterA)
        {
            return ChangeTilemap(Entity.A, direction, isOn);
        }
        else if (detectCenter == entityDetectCenterB)
        {
            return ChangeTilemap(Entity.B, direction, isOn);
        }
        else
            return false;
    }


    /// <summary>
    /// Change Tilemap centered at centerCell, with direction direction,
    /// in entity's world.
    /// </summary>
    /// <returns>True if successfully changed.</returns>
    public bool ChangeTilemapAt(Entity entity, Vector2Int centerCell, Direction direction, bool isOn = true)
    {
        List<Tilemap> tilemapChangeTo;
        Vector2Int[] shapeOffsets;
        if (entity == Entity.A)
        {
            tilemapChangeTo = isOn ? changingTilemapsA : swappingTilemapsA;
            shapeOffsets = shapeAList[(int)direction];
        }
        else if (entity == Entity.B)
        {
            tilemapChangeTo = isOn ? changingTilemapsB : swappingTilemapsB;
            shapeOffsets = shapeBList[(int)direction];
        }
        else
            return false;

        changeTilemap(tilemapChangeTo, centerCell, shapeOffsets, entity, isOn);

        return true;
    }

    /// <summary>
    /// Change tilemap at cell, in entity's world.
    /// </summary>
    /// <returns>True if succussfully changed.</returns>
    public bool ChangeTilemapAt(Entity entity, Vector2Int cell, bool isOn = true)
    {
        Transform ent;
        List<Tilemap> tilemapChangeTo;
        if (entity == Entity.A)
        {
            ent = entityDetectCenterA;
            tilemapChangeTo = isOn ? changingTilemapsA : swappingTilemapsA;
        }
        else if (entity == Entity.B)
        {
            ent = entityDetectCenterB;
            tilemapChangeTo = isOn ? changingTilemapsB : swappingTilemapsB;
        }
        else
            return false;

        Vector2Int entityCell = (Vector2Int)grid.WorldToCell(ent.transform.position);
        Vector2Int[] shapeOffsets = { cell - (Vector2Int)grid.WorldToCell(ent.position) };
        changeTilemap(tilemapChangeTo, entityCell, shapeOffsets, entity, isOn);
        return false;
    }

    /// <summary>
    /// Restore tile to its previous state.
    /// </summary>
    /// <returns>Return True if surccessfully restored.</returns>
    public bool RestoreTilemap(Entity entity, Direction direction)
    {
        return ChangeTilemap(entity, direction, false);
    }

    /// <summary>
    /// Restore tile to its previous state.
    /// </summary>
    /// <returns>Return True if surccessfully restored.</returns>
    public bool RestoreTilemap(Entity entity, Vector2Int offset)
    {
        return ChangeTilemap(entity, offset, false);
    }


    /// <summary>
    /// Change tilemap to specified entity's own world.
    /// Invoke this method when using "swap"
    /// </summary>
    /// <param name="entity">The entity whose world should this method change to.</param>
    public void SwapTilemap(Entity entity)
    {
        Vector3Int vec3 = new Vector3Int();
        Vector2Int vec2 = new Vector2Int();
        BoundsInt bounds;

        List<Tilemap> swappedTo = entity == Entity.A ? swappingTilemapsA : swappingTilemapsB;
        HashSet<Vector2Int> tilesChangedCurrent = entity == Entity.A ? tileChangeListA : tileChangeListB;
        HashSet<Vector2Int> tilesChangedOther = entity == Entity.A ? tileChangeListB : tileChangeListA;
        List<Tilemap> tilemapChangedCurrent = entity == Entity.A ? changingTilemapsA : changingTilemapsB;
        List<Tilemap> tilemapChangedOther = entity == Entity.A ? changingTilemapsB_ShownInA : changingTilemapsA_ShownInB;

        for(int i = 0; i < swappedTo.Count; i++)
        {
            bounds = swappedTo[i].cellBounds;
            for (int j = bounds.xMin; j < bounds.xMax; j++)
            {
                for (int k = bounds.yMin; k < bounds.yMax; k++)
                {
                    vec3.Set(j, k, 0);
                    vec2.Set(j, k);
                    if (tilesChangedCurrent.Contains(vec2))
                    {
                        tilemapCanvasPool.SetTile(i, vec3, tilemapChangedCurrent[i]);
                    }
                    else if (tilesChangedOther.Contains(vec2))
                    {
                        tilemapCanvasPool.SetTile(i, vec3, tilemapChangedOther[i]);
                    }
                    else
                    {
                        tilemapCanvasPool.SetTile(i, vec3, swappedTo[i]);
                    }
                }
            }
        }


        tilemapCanvasPool.RefreshCollider();
    }


    /// <summary>
    /// Select a new lightShape for entity.
    /// </summary>
    /// <returns>The lightShaped been used</returns>
    public LightShape SelectLightshape(Entity entity, LightShape lightShape)
    {
        LightShape old = entity == Entity.A ? lightShapeA : lightShapeB;
        if(entity == Entity.A)
        {
            lightShapeA = lightShape;
        }
        else
        {
            lightShapeB = lightShape;
        }
        compileShapeOffsets();
        return old;
    }


    /// <summary>
    /// Retrieve current tile's logic type.
    /// </summary>
    /// <returns>Current TileType</returns>
    public TileType GetCurrentTileType(Entity entity)
    {
        return GetOffsetTileType(entity, Vector2Int.zero);
    }


    /// <summary>
    /// Retrive offset tile's logic type
    /// </summary>
    /// <returns>Offset tile's TileType</returns>
    public TileType GetOffsetTileType(Entity entity, Vector2Int offset)
    {
        if (isOffsetTileWall(entity, offset))
        {
            return TileType.WALL;
        }
        else if (isOffsetTileElevator(entity, offset))
        {
            return TileType.ELEVATOR;
        }
        else if (isOffsetTilePortal(entity, offset))
        {
            return TileType.PORTAL;
        }
        else if (isOffsetTileInteractable(entity, offset))
        {
            return TileType.INTERACTABLE;
        }

        return TileType.PASSABLE;
    }

    
    /// <summary>
    /// Retrieve destination scene's name of current elevator.
    /// </summary>
    /// <returns>Destination scene name</returns>
    public string GetElevatorDestinationScene(Entity entity)
    {
        Transform ent = entity == Entity.A ? entityDetectCenterA : entityDetectCenterB;
        return tilemapData.GetElevatorDestinationScene(entity, (Vector2Int)grid.WorldToCell(ent.position));
    }

    /// <summary>
    /// *** IMPORTANT ***
    /// Using GetCurrentTileType to check if current tile is a portal, before invoke this method.
    /// Retrieve the other half portal's location,
    /// of current location's portal.
    /// </summary>
    /// <returns>The other half portal's location</returns>
    public Vector2Int GetOtherPortalLocation(Entity entity)
    {
        Transform ent = entity == Entity.A ? entityDetectCenterA : entityDetectCenterB;
        return tilemapData.GetOtherPortalLocation(entity, (Vector2Int)grid.WorldToCell(ent.position));
    }



    /// <summary>
    /// Retrieve current tile, for furthur processing
    /// </summary>
    /// <returns>Current assigned grid.</returns>
    public Grid GetCurrentGrid()
    {
        return grid;
    }

    /// <summary>
    /// Transforms world position to grid coordinates.
    /// </summary>
    /// <param name="worldPosition">World position</param>
    /// <returns>Cell coordinate in this world position.</returns>
    public Vector2Int GetCell(Vector3 worldPosition)
    {
        return (Vector2Int)grid.WorldToCell(worldPosition);
    }

    #endregion



    #region Private_Method
    private void initializeTilemapCanvas()
    {
        tilemapCanvasPool.ClearTilemap();

        Vector3Int vec = new Vector3Int();
        int index = 0;
        BoundsInt bounds;
        foreach (Tilemap tilemap in initialTilemaps)
        {
            tilemapCanvasPool.CopyTilemapInfo(index, tilemap);
            bounds = tilemap.cellBounds;
            for (int i = bounds.xMin; i < bounds.xMax; i++)
            {
                for (int j = bounds.yMin; j < bounds.yMax; j++)
                {
                    vec.Set(i, j, 0);
                    tilemapCanvasPool.SetTile(index, vec, tilemap);
                }
            }
            index++;
        }

        tilemapCanvasPool.RefreshCollider();
    }


    private void compileShapeOffsets()
    {
        shapeAList.Clear();
        shapeAList.Add(lightShapeA.shapeOffset);
        foreach (Matrix2x2 mat in shapeOffsetChangeList)
        {
            shapeAList.Add(mat.LeftMul(lightShapeA.shapeOffset));
        }
        shapeBList.Clear();
        shapeBList.Add(lightShapeB.shapeOffset);
        foreach (Matrix2x2 mat in shapeOffsetChangeList)
        {
            shapeBList.Add(mat.LeftMul(lightShapeB.shapeOffset));
        }
    }


    private void changeTilemap(List<Tilemap> tilemapChangeTo, Vector2Int entityCell, Vector2Int[] shapeOffsets, Entity entity, bool isChange)
    {
        foreach (Vector2Int offset in offsetWallTest(entity, shapeOffsets))
        {
            Vector2Int cell = new Vector2Int(entityCell.x + offset.x, entityCell.y + offset.y);
            Vector3Int vec = new Vector3Int(cell.x, cell.y, 0);
            tilemapCanvasPool.SetTile(tilemapChangeTo, vec);
            changeOrRestoreTile(entity, cell, isChange);
        }

        tilemapCanvasPool.RefreshCollider();
    }

    private void selectTilemaps(Entity entity, Grid grid)
    {
        updateTilemapNameDict();
        updateTilemapDict();
        List<Tilemap> currentList;
        Transform tilemapRoot;

        for(int i = 0; i < grid.transform.childCount; i++)
        {
            tilemapRoot = grid.transform.GetChild(i);
            foreach(KeyValuePair<int, string> pair in tilemapNameDict)
            {
                if(pair.Value == tilemapRoot.name)
                {
                    currentList = tilemapDict[pair.Key];
                    for (int j = 0; j < tilemapRoot.childCount; j++)
                    {
                        currentList.Add(tilemapRoot.GetChild(j).GetComponent<Tilemap>());
                    }
                }
            }
        }

        initialTilemaps = entity == Entity.A ? swappingTilemapsA : swappingTilemapsB;
    }



    private void clearChangedTile()
    {
        this.tileChangeListA.Clear();
        this.tileChangeListB.Clear();
    }


    private bool changeOrRestoreTile(Entity entity, Vector2Int tile, bool isChange)
    {
        if (isChange)
        {
            return addChangedTile(entity, tile);
        }
        else
        {
            return restoreChangedTile(entity, tile);
        }
    }

    /// <summary>
    /// Add changed tiles to record.
    /// </summary>
    /// <returns>true if the list doestn't contain the tile, false if contains</returns>
    private bool addChangedTile(Entity entity, Vector2Int tile)
    {
        if(entity == Entity.A)
        {
            if (this.tileChangeListA.Contains(tile))
            {
                return false;
            }
            this.tileChangeListA.Add(tile);
            this.tileChangeListB.Remove(tile);

            return true;
        }
        else if(entity == Entity.B)
        {
            if (this.tileChangeListB.Contains(tile))
            {
                return false;
            }
            this.tileChangeListB.Add(tile);
            this.tileChangeListA.Remove(tile);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Delete changed tile from record.
    /// </summary>
    /// <returns>true if the list contains the tile, false if doesn't contains</returns>
    private bool restoreChangedTile(Entity entity, Vector2Int tile)
    {
        if (entity == Entity.A)
        {
            if (this.tileChangeListA.Contains(tile))
            {
                this.tileChangeListA.Remove(tile);
                return true;
            }
            return false;
        }
        else if(entity == Entity.B)
        {
            if (this.tileChangeListB.Contains(tile))
            {
                this.tileChangeListB.Remove(tile);
                return true;
            }
            return false;
        }
        return false;
    }

    private void updateTilemapNameDict()
    {
        tilemapNameDict.Clear();
        tilemapNameDict.Add((int)TilemapType.Changing_A, changingA);
        tilemapNameDict.Add((int)TilemapType.Changing_B, changingB);
        tilemapNameDict.Add((int)TilemapType.Swapping_A, swappingA);
        tilemapNameDict.Add((int)TilemapType.Swapping_B, swappingB);
        tilemapNameDict.Add((int)TilemapType.Changing_A_Shown_In_B, changingAShownInB);
        tilemapNameDict.Add((int)TilemapType.Changing_B_Shown_In_A, changingBShownInA);
    }

    private void updateTilemapDict()
    {
        tilemapDict.Clear();
        tilemapDict.Add((int)TilemapType.Changing_A, changingTilemapsA);
        tilemapDict.Add((int)TilemapType.Changing_B, changingTilemapsB);
        tilemapDict.Add((int)TilemapType.Swapping_A, swappingTilemapsA);
        tilemapDict.Add((int)TilemapType.Swapping_B, swappingTilemapsB);
        tilemapDict.Add((int)TilemapType.Changing_A_Shown_In_B, changingTilemapsA_ShownInB);
        tilemapDict.Add((int)TilemapType.Changing_B_Shown_In_A, changingTilemapsB_ShownInA);
    }


    private bool isOffsetTileWall(Entity entity, Vector2Int offset)
    {
        Transform ent = entity == Entity.A ? entityDetectCenterA : entityDetectCenterB;
        Vector3 pos = new Vector3(ent.position.x + offset.x, ent.position.y + offset.y, ent.position.z);
        bool res = false;
        for(int i = 0;i < initialTilemaps.Count; i++)
        {
            res |= (tilemapCanvasPool.GetColliderType(i, grid.WorldToCell(pos)) != Tile.ColliderType.None);
        }
        return res;
    }

    private bool isOffsetTilePortal(Entity entity, Vector2Int offset)
    {
        Transform ent = entity == Entity.A ? entityDetectCenterA : entityDetectCenterB;
        Vector3 pos = new Vector3(ent.position.x + offset.x, ent.position.y + offset.y, ent.position.z);
        return tilemapData.IsPortalLocation(entity, (Vector2Int)grid.WorldToCell(pos));
    }

    private bool isOffsetTileElevator(Entity entity, Vector2Int offset)
    {
        Transform ent = entity == Entity.A ? entityDetectCenterA : entityDetectCenterB;
        Vector3 pos = new Vector3(ent.position.x + offset.x, ent.position.y + offset.y, ent.position.z);
        return tilemapData.IsElevatorLocation(entity, (Vector2Int)grid.WorldToCell(pos));
    }

    private bool isOffsetTileInteractable(Entity entity, Vector2Int offset)
    {
        Transform ent = entity == Entity.A ? entityDetectCenterA : entityDetectCenterB;
        Vector3 pos = new Vector3(ent.position.x + offset.x, ent.position.y + offset.y, ent.position.z);
        return tilemapData.IsInteractableLocation(entity, (Vector2Int)grid.WorldToCell(pos));
    }

    private List<Vector2Int> offsetWallTest(Entity entity, Vector2Int[] offsets)
    {
        if (offsets.Length == 0) return null;
        Vector3Int center = (Vector3Int)grid.WorldToCell((entity == Entity.A ? entityDetectCenterA : entityDetectCenterB).position);
        List<OffsetDepth> offsetsList = new List<OffsetDepth>();
        foreach(Vector2Int offset in offsets)
        {
            offsetsList.Add(new OffsetDepth(offset));
        }
        offsetsList.Sort();

        processedOffsets.Clear();
        lowerWall.Clear();
        upperWall.Clear();
        int leftWallDepth = int.MinValue;
        int rightWallDepth = int.MaxValue;
        
        foreach(Vector2Int offset in offsets)
        {
            if(offset.y > 0)    // compare upper wall
            {
                if(isOffsetTileWall(entity, offset))
                {
                    if (!upperWall.ContainsKey(offset.x))
                    {
                        upperWall[offset.x] = offset.y;
                        processedOffsets.Add(offset);        // add upper wall
                    }
                }
                else{
                    if(!upperWall.ContainsKey(offset.x) || upperWall[offset.x] > offset.y)
                    {
                        processedOffsets.Add(offset);
                    }
                }
            }
            else if(offset.y < 0)   // compare right wall
            {
                if(isOffsetTileWall(entity, offset))
                {
                    if (!lowerWall.ContainsKey(offset.x))
                    {
                        lowerWall[offset.x] = offset.y;
                        // don't add lower wall
                    }
                }
                else
                {
                    if(!lowerWall.ContainsKey(offset.x) || lowerWall[offset.x] < offset.y)
                    {
                        processedOffsets.Add(offset);
                    }
                }
            }
            else {  // compare left and right wall
                if(offset.x < 0)
                {
                    if(isOffsetTileWall(entity, offset))
                    {
                        leftWallDepth = offset.x;
                        processedOffsets.Add(offset);
                    }
                    else if(offset.x > leftWallDepth)
                    {
                        processedOffsets.Add(offset);
                    }
                }
                else
                {
                    if (isOffsetTileWall(entity, offset))
                    {
                        rightWallDepth = offset.x;
                        processedOffsets.Add(offset);
                    }
                    else if(offset.x < rightWallDepth)
                    {
                        processedOffsets.Add(offset);
                    }
                }
            }
        }

        return processedOffsets;
    }


    #endregion



    #region GIZMOS

    void OnDrawGizmos()
    {
        Tilemap tilemap = grid.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>();

        if (tilemap == null) return;

        if (IsDrawCellCoordinates)
        {
            for (int i = -CellCoordinateRange.x/2; i < CellCoordinateRange.x/2; i++)
            {
                for (int j = -CellCoordinateRange.y/2; j < CellCoordinateRange.y/2; j++)
                {
                    Vector3 pos = grid.CellToWorld(new Vector3Int(i, j, 0));
                    Handles.Label(new Vector3(pos.x + tilemap.cellSize.x/8,
                    pos.y + tilemap.cellSize.y/2, pos.z + tilemap.cellSize.z / 2), "(" + pos.x + "," + pos.y + ")");
                }
            }
        }
    }

    #endregion

}


[CustomEditor(typeof(TilemapSwapper))]
public class TilemapSwapperEditor: Editor
{
    public void OnSceneGUI()
    {
        TilemapSwapper t = target as TilemapSwapper;
        Grid grid = t.GetCurrentGrid();
        Camera camera = SceneView.currentDrawingSceneView.camera;
        Tilemap tilemap = grid.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>();

        if (tilemap == null) return;

        Vector2Int upleft = new Vector2Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMax);

        if (t.IsDrawCurrentCellPos)
        {
            Vector3 mousePosition = new Vector3();
            mousePosition.x = Event.current.mousePosition.x;
            mousePosition.y = camera.pixelHeight - Event.current.mousePosition.y;
            mousePosition = camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, camera.nearClipPlane));
            Vector3Int cell = grid.WorldToCell(mousePosition);
            Vector3 pos = grid.CellToWorld(cell);
            Handles.DrawWireCube(new Vector3(pos.x + tilemap.cellSize.x / 2,
                    pos.y + tilemap.cellSize.y / 2, pos.z + tilemap.cellSize.z / 2), tilemap.cellSize);
            Handles.Label(new Vector3(upleft.x, upleft.y + tilemap.cellSize.y, 0), "Current Size: " + "(" + pos.x + "," + pos.y + ")");
        }
    }
}


public class OffsetDepth : IComparable
{
    public Vector2Int value
    {
        get;
        set;
    }

    public OffsetDepth(Vector2Int value)
    {
        this.value = value;
    }

    public int CompareTo(System.Object obj)
    {
        OffsetDepth other = obj as OffsetDepth;
        if(Math.Abs(this.value.y) == Math.Abs(other.value.y))
        {
            return Math.Abs(this.value.x) - Math.Abs(other.value.x);
        }
        else
        {
            return Math.Abs(this.value.y) - Math.Abs(other.value.y);
        }
    }

    public override string ToString()
    {
        return this.value.ToString();
    }
}


public class TilemapCanvasPool
{
    public Grid grid
    {
        get;
        set;
    }

    private List<Tilemap> tilemapList;

    private Transform canvasRoot;

    public TilemapCanvasPool()
    {
        tilemapList = new List<Tilemap>();
    }

    public TilemapCanvasPool(Transform grid, int initialTilemapCount = 5)
    {
        tilemapList = new List<Tilemap>();
        canvasRoot = new GameObject("tilemapCanvas").transform;
        for(int i = 0; i < initialTilemapCount; i++)
        {
            tilemapList.Add(createTilemapObject(canvasRoot, i));
        }

        this.grid = grid.GetComponent<Grid>();
    }

    ~TilemapCanvasPool()
    {
        foreach(var child in tilemapList)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(canvasRoot.gameObject);
    }


    public Grid SelectGrid(Grid grid)
    {
        if(canvasRoot == null)
        {
            canvasRoot = new GameObject("tilemapCanvas").transform;
        }
        canvasRoot.parent = grid.transform;
        Grid old = this.grid;
        this.grid = grid;

        int n = tilemapList.Count;
        for(int i = 0; i < grid.transform.childCount; i++)
        {
            if(n < grid.transform.GetChild(i).childCount)
            {
                n = grid.transform.GetChild(i).childCount;
            }
        }
        // Add more buffer
        for (int i = tilemapList.Count; i < n; i++)
        {
            tilemapList.Add(createTilemapObject(canvasRoot, i));
        }

        ClearTilemap();
        RefreshCollider();

        return old;
    }

    public void ClearTilemap()
    {
        foreach(var tilemap in tilemapList)
        {
            tilemap.ClearAllTiles();
        }
    }

    public void RefreshCollider(int count = -1)
    {
        if(count == -1)
        {
            foreach (var tilemap in tilemapList)
            {
                tilemap.GetComponent<TilemapCollider2D>().enabled = false;
                tilemap.GetComponent<TilemapCollider2D>().enabled = true;
            }
        }
        else if(count >= 0 && count < tilemapList.Count)
        {
            for(int i = 0; i < count; i++){
                tilemapList[i].GetComponent<TilemapCollider2D>().enabled = false;
                tilemapList[i].GetComponent<TilemapCollider2D>().enabled = true;
            }
        }
    }

    public void CopyTilemapInfo(int index, Tilemap tilemap)
    {
        TilemapRenderer tr = tilemapList[index].GetComponent<TilemapRenderer>();
        TilemapRenderer tr_ = tilemap.GetComponent<TilemapRenderer>();
        //tr.sortOrder = tr_.sortOrder;
        tr.sortingOrder = tr_.sortingOrder;
        tr.sortingLayerName = tr_.sortingLayerName;
        tr.sortingLayerID = tr_.sortingLayerID;
        tr.sharedMaterial = tr_.material;
    }

    public void SetTile(int index, Vector3Int position, Tilemap tilemap)
    {
        TilemapFlag tf = tilemap.GetComponent<TilemapFlag>();
        if(tf != null)
        {
            if (tf.IsTwoTileHigh)
            {
                TileBase a = tilemap.GetTile(position);
                TileBase b = tilemap.GetTile(new Vector3Int(position.x, position.y + 1, position.z));
                TileBase c = tilemapList[index].GetTile(position);
                TileBase d = tilemapList[index].GetTile(new Vector3Int(position.x, position.y + 1, position.z));
                if(a != null && b != null || c != null && d != null)
                {
                    tilemapList[index].SetTile(position, tilemap.GetTile(position));
                    tilemapList[index].SetTile(new Vector3Int(position.x, position.y + 1, position.z),
                        tilemap.GetTile(new Vector3Int(position.x, position.y + 1, position.z)));
                }
            }

        }
        else
        {
            tilemapList[index].SetTile(position, tilemap.GetTile(position));
        }

    }

    public void SetTile(List<Tilemap> tilemaps, Vector3Int position)
    {
        for (int i = 0; i < tilemaps.Count; i++)
        {
            TilemapFlag tf = tilemaps[i].GetComponent<TilemapFlag>();
            if(tf != null)
            {
                if (tf.IsTwoTileHigh)
                {
                    TileBase a = tilemaps[i].GetTile(position);
                    TileBase b = tilemaps[i].GetTile(new Vector3Int(position.x, position.y, position.z));
                    TileBase c = tilemapList[i].GetTile(position);
                    TileBase d = tilemapList[i].GetTile(new Vector3Int(position.x, position.y + 1, position.z));
                    if (a != null && b != null || c != null && d != null)
                    {
                        tilemapList[i].SetTile(position, tilemaps[i].GetTile(position));
                        tilemapList[i].SetTile(new Vector3Int(position.x, position.y + 1, position.z),
                            tilemaps[i].GetTile(new Vector3Int(position.x, position.y + 1, position.z)));
                    }
                }
            }
            else
            {
                tilemapList[i].SetTile(position, tilemaps[i].GetTile(position));
            }
        }
    }

    public Tile.ColliderType GetColliderType(int index, Vector3Int position)
    {
        return tilemapList[index].GetColliderType(position);
    }

    private Tilemap createTilemapObject(Transform parent, int count)
    {
        GameObject go = new GameObject("tilemap (" + count + ")");
        go.AddComponent<Tilemap>();
        go.AddComponent<TilemapRenderer>();
        go.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        go.AddComponent<CompositeCollider2D>();
        go.AddComponent<TilemapCollider2D>().usedByComposite = true;

        go.transform.parent = parent;
        return go.transform.GetComponent<Tilemap>();
    }

}