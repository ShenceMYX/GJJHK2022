using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class PortalOperator: MonoSingleton<PortalOperator>
{

    /// <summary>
    /// Teleport entity to destination cell in the same grid.
    /// </summary>
    public void TeleportEntity(Transform entityTrans, Grid grid, Vector2Int destCell)
    {
        entityTrans.position = grid.CellToWorld((Vector3Int)destCell);
    }
}
