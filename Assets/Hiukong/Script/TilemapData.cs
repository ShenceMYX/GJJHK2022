using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapData : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Portal locations, for entity A")]
    private List<Vector2Int> portalListA_X;

    [SerializeField]
    [Tooltip("The other half of portal locations, for entity A")]
    private List<Vector2Int> portalListA_Y;

    [SerializeField]
    [Tooltip("Portal locations, for entity B")]
    private List<Vector2Int> portalListB_X;

    [SerializeField]
    [Tooltip("The other half of portal locations, for entity B")]
    private List<Vector2Int> portalListB_Y;

    [SerializeField]
    [Tooltip("Elevator locations for entity A")]
    private List<Vector2Int> elevatorListA;

    [SerializeField]
    [Tooltip("Next level's scene name, for corresponding elevator, for A (temporary gameplay)")]
    private List<string> elevatorDestinationSceneListA;

    [SerializeField]
    [Tooltip("Elevator locations for entity B")]
    private List<Vector2Int> elevatorListB;

    [SerializeField]
    [Tooltip("Next level's scene name, for corresponding elevator, for B (temporary gameplay)")]
    private List<string> elevatorDestinationSceneListB;

    [SerializeField]
    [Tooltip("Interactable Tile, for entity A")]
    private List<Vector2Int> interactableListA;

    [SerializeField]
    [Tooltip("Interactable Tile, for entity B")]
    private List<Vector2Int> interactableListB;


    /// <summary>
    /// Determine if current position contains a portal.
    /// </summary>
    /// <returns>True if there is a portal</returns>
    public bool IsPortalLocation(TilemapSwapper.Entity entity, Vector2Int pos)
    {
        if (entity == TilemapSwapper.Entity.A)
        {
            return portalListA_X.Contains(pos) || portalListA_Y.Contains(pos);
        }
        else if(entity == TilemapSwapper.Entity.B)
        {
            return portalListB_X.Contains(pos) || portalListB_Y.Contains(pos);
        }
        return false;
    }



    /// <summary>
    /// Invoke IsPortalLocation before invoking this method.
    /// </summary>
    /// <returns>The other portal's location</returns>
    public Vector2Int GetOtherPortalLocation(TilemapSwapper.Entity entity, Vector2Int pos)
    {
        if(entity == TilemapSwapper.Entity.A)
        {
            if (portalListA_X.Contains(pos))
            {
                return portalListA_Y[portalListA_X.IndexOf(pos)];
            }
            else
            {
                return portalListA_X[portalListA_Y.IndexOf(pos)];
            }
        }
        else
        {
            if (portalListB_X.Contains(pos))
            {
                return portalListB_Y[portalListB_X.IndexOf(pos)];
            }
            else
            {
                return portalListB_X[portalListB_Y.IndexOf(pos)];
            }
        }
    }


    /// <summary>
    /// Determine if current position is a elevator location
    /// </summary>
    /// <returns>True if elevator location</returns>
    public bool IsElevatorLocation(TilemapSwapper.Entity entity, Vector2Int pos)
    {
        if(entity == TilemapSwapper.Entity.A)
        {
            return elevatorListA.Contains(pos);
        }
        else if(entity == TilemapSwapper.Entity.B)
        {
            return elevatorListB.Contains(pos);
        }
        return false;
    }


    /// <summary>
    /// Invoke IsElevatorLocation before invoking this method.
    /// </summary>
    /// <returns>Return next scene's name this elevetor location leads to.</returns>
    public string GetElevatorDestinationScene(TilemapSwapper.Entity entity, Vector2Int pos)
    {
        if (entity == TilemapSwapper.Entity.A)
        {
            return elevatorDestinationSceneListA[elevatorListA.IndexOf(pos)];
        }
        else
        {
            return elevatorDestinationSceneListB[elevatorListB.IndexOf(pos)];
        }
    }


    /// <summary>
    /// Determine if current tile is an interactable location.
    /// </summary>
    /// <returns>True if is interactable location</returns>
    public bool IsInteractableLocation(TilemapSwapper.Entity entity, Vector2Int pos)
    {
        if(entity == TilemapSwapper.Entity.A)
        {
            return interactableListA.Contains(pos);
        }
        else if(entity == TilemapSwapper.Entity.B)
        {
            return interactableListB.Contains(pos);
        }
        return false;
    }

}
