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

    private Vector2Int[] shapeA =
    {
        new Vector2Int(-1,1),
        new Vector2Int(-1,2),
        new Vector2Int(0,1),
        new Vector2Int(0,2),
        new Vector2Int(1,1),
        new Vector2Int(1,2),
    };

    [SerializeField]
    [Tooltip("Light shape offsets data object.\nUsing MouseRightClick/Create/GGJGameData/LightShape to create one")]
    private LightShape lightShapeB;

    public Vector2Int[] shapeB =
    {
        new Vector2Int(-1,1),
        new Vector2Int(-1,2),
        new Vector2Int(0,1),
        new Vector2Int(0,2),
        new Vector2Int(1,1),
        new Vector2Int(1,2),
    };

    // storing flashlight's shape offsets in 4 directions
    private List<Vector2Int[]> shapeAList = new List<Vector2Int[]>();
    private List<Vector2Int[]> shapeBList = new List<Vector2Int[]>();
    private HashSet<Vector2Int> permanentCellList = new HashSet<Vector2Int>();

    #endregion


    #region API

    /// <summary>
    /// Change Tilemap to its original state.
    /// Invoke this method in level start or level restart.
    /// </summary>
    public void InitializeTilemap()
    {
        compileShapeOffsets();
        permanentCellList.Clear();
        initializeTilemapCanvas();
    }


    /// <summary>
    /// Swap tilemap to specified world's tilemap. 
    /// Invoke this method when using "flashlight".
    /// </summary>
    /// <param name="entity">Specify which entity's world to swap</param>
    /// <param name="direction">And in which direction</param>
    public bool ChangeTilemap(Entity entity, Direction direction)
    {
        Transform ent;
        Tilemap tilemapChangeTo;
        Vector2Int[] shapeOffsets;
        if (entity == Entity.A)
        {
            ent = entityDetectCenterA;
            tilemapChangeTo = changingTilemapA;
            shapeOffsets = shapeAList[(int)direction];
        }
        else if (entity == Entity.B)
        {
            ent = entityDetectCenterB;
            tilemapChangeTo = changingTilemapB;
            shapeOffsets = shapeBList[(int)direction];
        }
        else 
            return false;

        Vector2Int entityCell = (Vector2Int)grid.WorldToCell(ent.transform.position);
        //Debug.Log("Current Cell: " + entityCell.ToString());
        //Debug.Log("Current position: " + ent.transform.position);
        changeTilemap(tilemapChangeTo, entityCell, shapeOffsets);

        return true;   
    }


    /// <summary>
    /// Swap tilemap to specified world's tilemap. 
    /// Invoke this method when using "flashlight".
    /// </summary>
    /// <param name="entity">The detect center transform of the entity</param>
    /// <param name="direction">And in which direction</param>
    /// <returns></returns>
    public bool ChangeTilemap(Transform detectCenter, Direction direction)
    {
        if (detectCenter == entityDetectCenterA)
        {
            return ChangeTilemap(Entity.A, direction);
        }
        else if (detectCenter == entityDetectCenterB)
        {
            return ChangeTilemap(Entity.B, direction);
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
        Tilemap tilemapSwapTo;
        if(entity == Entity.A)
        {
            tilemapSwapTo = swappingTilemapA;
        }
        else if(entity == Entity.B)
        {
            tilemapSwapTo = swappingTilemapB;
        }
        else
        {
            return;
        }

        BoundsInt bounds = initialTilemap.cellBounds;
        Vector3Int vec = new Vector3Int();
        for (int i = bounds.xMin; i < bounds.xMax; i++)
        {
            for (int j = bounds.yMin; j < bounds.yMax; j++)
            {
                vec.Set(i, j, 0);
                if (!permanentCellList.Contains(new Vector2Int(i, j)))
                {
                    tilemapCanvas.SetTile(vec, tilemapSwapTo.GetTile(vec));
                }
                
            }
        }
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
    }


    private void compileShapeOffsets()
    {
        shapeAList.Clear();
        shapeAList.Add(shapeA);
        foreach (Matrix2x2 mat in shapeOffsetChangeList)
        {
            shapeAList.Add(mat.LeftMul(shapeA));
        }
        shapeBList.Clear();
        shapeBList.Add(shapeB);
        foreach (Matrix2x2 mat in shapeOffsetChangeList)
        {
            shapeBList.Add(mat.LeftMul(shapeB));
        }

        /*
        Debug.Log("Shape A Offset");
        int i = 0;
        foreach (Vector2Int[] offset in shapeAList)
        {
            Debug.Log("Offset for " + (Direction)(i++));
            foreach(Vector2Int vec in offset)
            {
                Debug.Log(vec);
            }
        }
        */
    }
    private void changeTilemap(Tilemap tilemapChangeTo, Vector2Int entityCell, Vector2Int[] shapeOffsets)
    {
        foreach (Vector2Int offset in shapeOffsets)
        {
            Vector2Int cell = new Vector2Int(entityCell.x + offset.x, entityCell.y + offset.y);
            if (permanentCellList.Contains(cell))
            {
                continue;
            }
            else
            {
                permanentCellList.Add(cell);
                tilemapCanvas.SetTile(new Vector3Int(cell.x, cell.y, 0),
                        tilemapChangeTo.GetTile(new Vector3Int(cell.x, cell.y, 0)));
            }
        }
    }
    #endregion

}
