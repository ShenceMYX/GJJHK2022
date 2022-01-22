using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    int prevPressedMovementKey = -1;
    ArrayList movementKeys = new ArrayList { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S };
    void Start()
    {

    }

    void Update()
    {
        prevPressedMovementKey = -1;
        // Update Movement Key
        foreach (KeyCode movementKey in movementKeys)
        {
            if (Input.GetKeyDown(movementKey))
            {
                prevPressedMovementKey = (int)movementKey;
            }
        }

        RaycastHit2D ray;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.25f);
        switch (prevPressedMovementKey)
        {
            case (int)KeyCode.A:
                // TODO: test left is available
                ray = Physics2D.Linecast(colliderCenterPos, new Vector2(colliderCenterPos.x-1, colliderCenterPos.y));
                if (!ray.collider)
                {
                    pos.x -= 1;
                    
                }
                break;
            case (int)KeyCode.D:
                // TODO: test right is available
                ray = Physics2D.Linecast(colliderCenterPos, new Vector2(colliderCenterPos.x + 1, colliderCenterPos.y));
                if (!ray.collider)
                {
                    pos.x += 1;
                }
                break;
            case (int)KeyCode.W:
                // TODO: test up is available
                ray = Physics2D.Linecast(colliderCenterPos, new Vector2(colliderCenterPos.x, colliderCenterPos.y+1));
                if (!ray.collider)
                {
                    pos.y += 1;
                }
                break;
            case (int)KeyCode.S:
                // TODO: test down is available
                ray = Physics2D.Linecast(colliderCenterPos, new Vector2(colliderCenterPos.x, colliderCenterPos.y - 1));
                if (!ray.collider){
                    pos.y -= 1;
                }
                break;
            default:
                break;
        }

        transform.position = pos;


    }

    void OnDrawGizmos()
    {
        Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        Gizmos.DrawLine(colliderCenterPos, new Vector2(colliderCenterPos.x-1, colliderCenterPos.y));
    }
}
