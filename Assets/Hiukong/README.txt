[Script/TilemapSwapper]
Variable:
Tilemap initialTilemap: initial map set up
Tilemap changingTilemapA: permanently changed tilemap, for entity A
Tilemap changingTilemapB: permanently changed tilemap, for entity B
Tilemap swappingTilemapA: used for world swapping, for entity A
Tilemap swappingTilemapB: used for world swapping, for entity B
Tilemap tilemapCanvas, used for drawing current map, just a data holder

Transform entityDetectCenterA: collider detection origin center, for A
Transform entityDetectCenterB: collider detection origin center, for B

LightShape lightShapeA: light shape offset for Entity A
LightShape lightShapeB: light shape offset for Entity B


API:
InitializeTilemap()

Initialize tilemap, using initialTilemap to fill tilemapCanvas,
probably should invoke inside OnEnable() or other restart occasions.



ChangeTilemap(Enum Entity, Enum Direction)

Enum Entity is declared inside class TilemapSwapper,
use like TilemapSwapper.Entity.A, same for Enum Direction
Change tile permanently to one world type, with offset (0,0) in
the tile which entityDetectCenterA(B) stays.



ChangeTilemap(Transform, Direction)

Same as above, however, can use original entityDetectCenterA(B) as one of 
the two arguments.



SwapTilemap(Enum Entity)

Swap world to which belong to the argument Enum Entity type equals to.




[Script/LightShape]
A scriptable object, used for creating customed light shape.
Offset (0, 0) is the tile where entityDetectCenterA(B) stays.





[Remarks]
entityDetectCenterA(B) probably needs to be attached to a gameObject which
contains entity's spriteRenderer, and being a child gameObject.
