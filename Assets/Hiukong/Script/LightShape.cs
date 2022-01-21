using UnityEngine;

[CreateAssetMenu(fileName ="newLightShape", menuName ="GGJGameData/LightShape")]
public class LightShape : ScriptableObject
{
    [Tooltip("Offsets of the shape of the flashlight, for entity facing upward.\nDefault is a 3x2 rectangle.")]
    public Vector2Int[] shapeOffset =
    {
        new Vector2Int(-1,1),
        new Vector2Int(-1,2),
        new Vector2Int(0,1),
        new Vector2Int(0,2),
        new Vector2Int(1,1),
        new Vector2Int(1,2),
    };
}
