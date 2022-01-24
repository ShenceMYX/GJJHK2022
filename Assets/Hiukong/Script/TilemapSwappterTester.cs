using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TilemapSwappterTester : MonoBehaviour
{

    public PlayerController player;
    public Grid grid;

    Dictionary<KeyCode, TilemapSwapper.Direction> map;

    bool isFlashlightOn = false;

    void OnEnable()
    {
        map = new Dictionary<KeyCode, TilemapSwapper.Direction>();
        map[KeyCode.UpArrow] = TilemapSwapper.Direction.UP;
        map[KeyCode.RightArrow] = TilemapSwapper.Direction.RIGHT;
        map[KeyCode.DownArrow] = TilemapSwapper.Direction.DOWN;
        map[KeyCode.LeftArrow] = TilemapSwapper.Direction.LEFT;

        GetComponent<TilemapSwapper>().SelectTilemaps(grid);
        GetComponent<TilemapSwapper>().InitializeTilemap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFlashlightOn = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isFlashlightOn = false;
        }


        foreach (KeyValuePair<KeyCode, TilemapSwapper.Direction> pair in map)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                GetComponent<TilemapSwapper>().ChangeTilemap(TilemapSwapper.Entity.A, pair.Value, isFlashlightOn);
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            GetComponent<TilemapSwapper>().SwapTilemap(TilemapSwapper.Entity.A);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<TilemapSwapper>().SwapTilemap(TilemapSwapper.Entity.B);
        }
    }
}
