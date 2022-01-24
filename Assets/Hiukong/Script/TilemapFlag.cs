using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapFlag : MonoBehaviour
{
    public enum FlagItem
    {
        TwoTile,
        COUNT
    };


    [SerializeField]
    [Tooltip("Is this tilemap containing two tile high objects")]
    public bool IsTwoTileHigh = false;

    public void ClearFlag()
    {
        IsTwoTileHigh = false;
    }

    public void CopyFlag(TilemapFlag other)
    {
        if (other == null) return;
        this.IsTwoTileHigh = other.IsTwoTileHigh;
    }
    
}
