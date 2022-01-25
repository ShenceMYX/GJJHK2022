using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapFlag : MonoBehaviour
{
    public enum FlagItem
    {
        TwoTile,
        Door,
        COUNT
    };


    [SerializeField]
    [Tooltip("Is this tilemap containing two tile high objects")]
    public bool IsTwoTileHigh = false;

    [SerializeField]
    [Tooltip("Is this tilemap containing doors?")]
    public bool IsDoors = false;

    public void ClearFlag()
    {
        IsTwoTileHigh = false;
        IsDoors = false;
    }

    public void CopyFlag(TilemapFlag other)
    {
        if (other == null) return;
        this.IsTwoTileHigh = other.IsTwoTileHigh;
        this.IsDoors = other.IsDoors;
    }
    
}
