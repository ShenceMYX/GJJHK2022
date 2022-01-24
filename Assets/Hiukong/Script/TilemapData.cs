using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newTilemapData", menuName="GGJGameData/TilemapData")]
public class TilemapData : ScriptableObject
{
    public List<Vector2Int> portalListA_X;
    public List<Vector2Int> portalListA_Y;
    public List<Vector2Int> portalListB_X;
    public List<Vector2Int> portalListB_Y;
    public List<Vector2Int> elevatorListA;
    public List<string> elevatorDestinationListA;
    public List<Vector2Int> elevatorListB;
    public List<string> elevatorDestinationListB;
}
