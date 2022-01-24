[Script/TilemapSwapper]
Tilemap visual manipulator

Variable:
bool isDrawCurrentCellPos: showing mouse pointed cell's coordinate
bool isDrawCellCoordinates: showing Grid's coordinates

string changingTilemapANodeName: changed tilemap, for entity A
string changingTilemapBNodeName: changed tilemap, for entity B
string swappingTilemapANodeName: used for world swapping, for entity A
string swappingTilemapBNodeName: used for world swapping, for entity B
string changingTilemapAShownInBNodeName: tiles changed by A, shown in B's world
string changingTilemapBShownInANodeName: tiles changed by B, shown in A's world
string tilemapCanvasNodeName, used for drawing current map, just a data holder

Transform entityDetectCenterA: collider detection origin center, for A
Transform entityDetectCenterB: collider detection origin center, for B

LightShape lightShapeA: light shape offset for Entity A
LightShape lightShapeB: light shape offset for Entity B


API:

SelectTilemap(Grid grid, Entity initialTilemapEntity)
Select a room using it's grid gameObject, which contains all the 6
required tilemaps (initial, canvas, swappingA, swappingB, changingA,
changingB).
The initialMap deponds on the initial Entity who enters the world



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




SelectLightShape(Entity entity, LightShape lightShape)
Select a new lightShape for entity, returns the old one.




TileType GetCurrentTileType(Entity entity)
Retrieves current entity location's tile type
Enum TileType{PASSABLE, WALL, PORTAL, ELEVATOR, INTERACTABLE,}


TileType GetOffsetTileType(Entity entity, Vector2Int offset)
Retrieves offset entity location's tile type



string GetElevatorDestinationScene(Entity entity)
Retrives elevator destination's scene name.
Must invoke after checking current cell is an Elevator cell.




Vector2Int GetOtherPortalLocation(Entity entity)
Retrives current portal's destination cell(currently two way portal.)
Must invoke after checking current cell is a portal cell.



[Scirpt/TilemapData]
Level Data.
Attach this component to corresponding grid object.

Variables:
portalListA_X, portalListA_Y:
Must place peer portals in the same index location. Portals for entity A.

portalListB_X, portalListB_Y:
Same as above, portals for entity B.

elevatorListA, elevatorListB:
Elevator cell location for entity A/B.

elevatorDestinationSceneListA, elevatorDestinationSceneListB:
Destination scene for A/B's elevators, must place in the same index location
with elevatorListA or elevatorListB.




[Script/PortalOperator]
Portal operation methods.
API:
TeleportEntity(Transform entityTrans, Grid grid, Vector2Int destCell
Transfer entityTrans to location indicated by destCell in the grid.
destCell is grid coordinate of the portal destination position.





[Script/LightShape]
A scriptable object, used for creating customed light shape.
Offset (0, 0) is the tile where entityDetectCenterA(B) stays.




[Remarks]
entityDetectCenterA(B) probably needs to be attached to a gameObject which
contains entity's spriteRenderer, and being a child gameObject.

SelectTilemap should be used before entering a room, such as in game starts,
followed by an InitializeTilemap call if first time entering this room.

No longer need an initialTilemap placeHolder, instead, SelectTilemap must decides
which entity's world is intial world.
