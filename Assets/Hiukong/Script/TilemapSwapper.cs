using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Tilemaps;


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
    static Matrix2x2 shapeOffsetUpToRight = new Matrix2x2(new Vector2Int(0,-1), new Vector2Int(1,0));
    static Matrix2x2 shapeOffsetUpToDown = new Matrix2x2(new Vector2Int(1, 0), new Vector2Int(0, -1));
    static Matrix2x2 shapeOffsetUpToLeft = new Matrix2x2(new Vector2Int(0, 1), new Vector2Int(-1, 0));
    static Matrix2x2[] shapeOffsetChangeList = { shapeOffsetUpToRight, shapeOffsetUpToDown, shapeOffsetUpToLeft };
    #endregion


    #region Varaible_Declaration
    [Header("Tilemaps")]
    [SerializeField]
    [Tooltip("Drop in the grid which those tilemaps are using (their mutual parent node)")]
    private Grid grid;

    [SerializeField]
    [Tooltip("Drop in the initial tilemap")]
    private Tilemap initialTilemap;

    [SerializeField]
    [Tooltip("Drop in the tilemap to change permanently, for this entity")]
    private Tilemap changingTilemapA;

    [SerializeField]
    [Tooltip("Drop in the tilemap to change permanently, for this entity")]
    private Tilemap changingTilemapB;

    [SerializeField]
    [Tooltip("Drop in the tilemap to swap, for this entity")]
    private Tilemap swappingTilemapA;

    [SerializeField]
    [Tooltip("Drop in the tilemap to swap, for this entity")]
    private Tilemap swappingTilemapB;

    [Tooltip("The tilemap to draw all the tiles (a canvas tilemap)")]
    public Tilemap tilemapCanvas;

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


    // storing flashlight's shape offsets in 4 directions
    private List<Vector2Int[]> shapeAList = new List<Vector2Int[]>();
    private List<Vector2Int[]> shapeBList = new List<Vector2Int[]>();

    // storing tiles changed by A/B
    private HashSet<Vector2Int> tileChangeListA = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> tileChangeListB = new HashSet<Vector2Int>();

    #endregion


    #region API

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
        Tilemap tilemapChangeTo;
        Vector2Int[] shapeOffsets;
        if (entity == Entity.A)
        {
            ent = entityDetectCenterA;
            tilemapChangeTo = isOn ? changingTilemapA : swappingTilemapA;
            shapeOffsets = shapeAList[(int)direction];
        }
        else if (entity == Entity.B)
        {
            ent = entityDetectCenterB;
            tilemapChangeTo = isOn ? changingTilemapB : swappingTilemapB;
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
        Tilemap tilemapChangeTo;
        Vector2Int[] shapeOffsets = { offset };
        if (entity == Entity.A)
        {
            ent = entityDetectCenterA;
            tilemapChangeTo = isOn ? changingTilemapA : swappingTilemapA;
        }
        else if (entity == Entity.B)
        {
            ent = entityDetectCenterB;
            tilemapChangeTo = isOn ? changingTilemapB : swappingTilemapB;
        }
        else
            return false;

        Vector2Int entityCell = (Vector2Int)grid.WorldToCell(ent.transform.position);
        changeTilemap(tilemapChangeTo, entityCell, shapeOffsets, entity, isOn);

        return true;
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
    /// Change tilemap to specified entity's own world.
    /// Invoke this method when using "swap"
    /// </summary>
    /// <param name="entity">The entity whose world should this method change to.</param>
    public void SwapTilemap(Entity entity)
    {
        BoundsInt bounds = initialTilemap.cellBounds;
        Vector3Int vec3 = new Vector3Int();
        Vector2Int vec2 = new Vector2Int();

        Tilemap swappedTo = entity == Entity.A ? swappingTilemapA : swappingTilemapB;
        HashSet<Vector2Int> tilesChangedCurrent = entity == Entity.A ? tileChangeListA : tileChangeListB;
        HashSet<Vector2Int> tilesChangedOther = entity == Entity.A ? tileChangeListB : tileChangeListA;
        Tilemap tilemapChangedCurrent = entity == Entity.A ? changingTilemapA : changingTilemapB;
        Tilemap tilemapChangedOther = entity == Entity.A ? swappingTilemapB : swappingTilemapA;

        for (int i = bounds.xMin; i < bounds.xMax; i++)
        {
            for (int j = bounds.yMin; j < bounds.yMax; j++)
            {
                vec3.Set(i, j, 0);
                vec2.Set(i, j);
                if (tilesChangedCurrent.Contains(vec2))
                {
                    tilemapCanvas.SetTile(vec3, tilemapChangedCurrent.GetTile(vec3));
                }
                else if (tilesChangedOther.Contains(vec2))
                {
                    tilemapCanvas.SetTile(vec3, tilemapChangedOther.GetTile(vec3));
                }
                else
                {
                    tilemapCanvas.SetTile(vec3, swappedTo.GetTile(vec3));
                }
            }
        }

        refreshCollider();
    }

    #endregion



    #region Private_Method


    private void initializeTilemapCanvas()
    {
        BoundsInt bounds = initialTilemap.cellBounds;
        Vector3Int vec = new Vector3Int();
        for (int i = bounds.xMin; i < bounds.xMax; i++)
        {
            for (int j = bounds.yMin; j < bounds.yMax; j++)
            {
                vec.Set(i, j, 0);
                tilemapCanvas.SetTile(vec, initialTilemap.GetTile(vec));
            }
        }
        refreshCollider();
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


    private void changeTilemap(Tilemap tilemapChangeTo, Vector2Int entityCell, Vector2Int[] shapeOffsets, Entity entity, bool isChange)
    {
        foreach (Vector2Int offset in shapeOffsets)
        {
            Vector2Int cell = new Vector2Int(entityCell.x + offset.x, entityCell.y + offset.y);
            Vector3Int vec = new Vector3Int(cell.x, cell.y, 0);
            tilemapCanvas.SetTile(vec, tilemapChangeTo.GetTile(vec));
            changeOrRestoreTile(entity, cell, isChange);
        }
        refreshCollider();
    }


    private void refreshCollider()
    {
        tilemapCanvas.GetComponent<TilemapCollider2D>().enabled = false;
        tilemapCanvas.GetComponent<TilemapCollider2D>().enabled = true;
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
            return true;
        }
        else if(entity == Entity.B)
        {
            if (this.tileChangeListB.Contains(tile))
            {
                return false;
            }
            this.tileChangeListB.Add(tile);
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


    #endregion

}
