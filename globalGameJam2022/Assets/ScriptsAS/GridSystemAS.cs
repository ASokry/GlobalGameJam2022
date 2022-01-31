using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemAS : MonoBehaviour
{
    [SerializeField] private List<AbstractObjectAS> absObjectList;
    private AbstractObjectAS absObject;

    [SerializeField] private List<Vector2Int> pendantLoc;

    private PlacedObjectAS placedObjectForDeletion;
    private List<Vector2Int> coordinatesForDeletion;

    public List<Vector2Int> nullZones;
    public List<Vector2Int> inventorySlots;
    public List<Vector2Int> backpackSlots;
    public List<Vector2Int> handSlots;
    public List<Vector2Int> trashSlot;

    private Transform ghostObject;
    private bool ghostFollow = false;

    private GameGridAS<GridObjectAS> grid;
    private AbstractObjectAS.Dir dir = AbstractObjectAS.Dir.Down;

    private bool canBuild = true;
    private bool inventoryClear = false;

    public int gridWidth = 60;
    public int gridHeight = 18;
    public float cellSize = 3f;
    public GameObject gridTile;

    private GridObjectAS gridObjectForMouseDown;
    private PlacedObjectAS placedObjectForMouseDown;

    private bool haveFlashlight = false;
    private bool encounterEnemy = false;

    private int numOfBatterySquares = 0;
    private int numOfBatteries = 0;
    private BatteryObjAS battery = null;
    private List<Vector2Int> batteryLoc = null;

    private void Awake()
    {
        grid = new GameGridAS<GridObjectAS>(gridWidth, gridHeight, cellSize, Vector3.zero, (GameGridAS<GridObjectAS> g, int x, int y) => new GridObjectAS(g, x, y));
    }

    public class GridObjectAS
    {
        private GameGridAS<GridObjectAS> grid;
        private int x;
        private int y;
        private PlacedObjectAS placedObject;

        public GridObjectAS(GameGridAS<GridObjectAS> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetPlacedObject(PlacedObjectAS placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObjectAS GetPlacedObject()
        {
            return placedObject;
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject;
        }
    }

    private void GenerateTiles()
    {
        List<Vector2Int> entireGrid = new List<Vector2Int>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                entireGrid.Add(new Vector2Int(x,y));
            }
        }

        foreach (Vector2Int coordinates in entireGrid)
        {
            if (!nullZones.Contains(coordinates))
            {
                GameObject tiles = Instantiate(gridTile, grid.GetWorldPosition(coordinates.x, coordinates.y), Quaternion.identity);
            }
        }
    }

    private void SpawnItem(AbstractObjectAS absObj, int objPosX, int objPosY, int rotationOffsetX, int rotationOffsetY)
    {
        Vector3 pendantPosition = grid.GetWorldPosition(objPosX, objPosY) + new Vector3(rotationOffsetX, rotationOffsetY, 0) * grid.GetCellSize();

        PlacedObjectAS pendantItem = PlacedObjectAS.Create(pendantPosition, new Vector2Int(objPosX, objPosY), dir, absObj);

        grid.GetGridObject(objPosX, objPosY).SetPlacedObject(pendantItem);

        List<Vector2Int> firstItemPositionList = absObj.GetGridPositionList(new Vector2Int(objPosX, objPosY), dir);
        foreach (Vector2Int gridPosition in firstItemPositionList)
        {
            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(pendantItem);
        }
    }

    private void Start()
    {
        GenerateTiles();

        Vector2Int rotationOffset = new Vector2Int(0, 0);
        foreach (Vector2Int coordinate in nullZones)
        {
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(coordinate.x, coordinate.y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();

            PlacedObjectAS placedObject = PlacedObjectAS.Create(placedObjectWorldPosition, new Vector2Int(coordinate.x, coordinate.y), dir, absObjectList[0]);

            grid.GetGridObject(coordinate.x, coordinate.y).SetPlacedObject(placedObject);
        }

        SpawnItem(absObjectList[1], pendantLoc[0].x, pendantLoc[0].y, rotationOffset.x, rotationOffset.y);
        SpawnItem(absObjectList[2], 4, 6, rotationOffset.x, rotationOffset.y);
        SpawnItem(absObjectList[3], 8, 3, rotationOffset.x, rotationOffset.y);
        SpawnItem(absObjectList[3], 8, 5, rotationOffset.x, rotationOffset.y);
    }

    private void GhostFollow(AbstractObjectAS abs)
    {
        if (ghostFollow && ghostObject == null)
        {
            //print(null);
            ghostObject = Instantiate(abs.GetPrefab(), Mouse3DAS.GetMouseWorldPosition(), Quaternion.Euler(0, 0, abs.GetRotationAngle(dir)));
        }

        if (ghostObject != null)
        {
            //print("following");
            ghostObject.rotation = Quaternion.Euler(0, 0, abs.GetRotationAngle(dir));
            Vector2Int rotationOffset = absObject.GetRotationOffset(dir);
            //print(rotationOffset);
            //print(-cellSize / 2);
            ghostObject.position = Mouse3DAS.GetMouseWorldPosition() + absObject.GetPositionOffset(dir, cellSize);
        }
    }

    private bool MouseIsInGrid()
    {
        Vector3 mousePos = Mouse3DAS.GetMouseWorldPosition();
        int w = (int)cellSize * gridWidth;
        int h = (int)cellSize * gridHeight;
        if ((mousePos.x >= 0 && mousePos.y >= 0) && (mousePos.x < w && mousePos.y < h))
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        //print(Mouse3DAS.GetMouseWorldPosition().x + ", " + Mouse3DAS.GetMouseWorldPosition().y);
        if (Input.GetMouseButton(0) && MouseIsInGrid())
        {
            if (absObject == null)
            {
                gridObjectForMouseDown = grid.GetGridObject(Mouse3DAS.GetMouseWorldPosition());
                placedObjectForMouseDown = gridObjectForMouseDown.GetPlacedObject();
            }

            //print(placedObjectForMouseDown.GetObjectName());

            if (Input.GetMouseButtonDown(1))
            {
                dir = AbstractObjectAS.GetNextDir(dir);
                print(dir);
            }

            if (placedObjectForMouseDown != null && placedObjectForMouseDown.GetObjectName() != "Null Object" && placedObjectForMouseDown.GetObjectName() != "Flashlight")
            {
                ghostFollow = true;

                //Debug.Log(placedObject.GetObjectName());
                absObject = placedObjectForMouseDown.GetComponent<AbstractObjectAS>();

                placedObjectForDeletion = placedObjectForMouseDown;
                coordinatesForDeletion = placedObjectForDeletion.GetGridPositionList();
            }

            GhostFollow(absObject);
            
        }
        else if (Input.GetMouseButton(0) && absObject != null)
        {
            print("Out Bound!");
            ClearAbsAndGhost();
        }

        if (Input.GetMouseButtonUp(0) && MouseIsInGrid() && absObject != null)
        {
            TrashSlot();

            grid.GetXY(Mouse3DAS.GetMouseWorldPosition(), out int x, out int y);

            List<Vector2Int> gridPositionList = absObject.GetGridPositionList(new Vector2Int(x, y), dir);

            canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                //print(gridPosition.ToString());
                //print(grid.GetGridObject(gridPosition.x, gridPosition.y) == null);
                if (grid.GetGridObject(gridPosition.x, gridPosition.y) == null)
                {
                    canBuild = false;
                    break;
                }

                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    //Can build here
                    canBuild = false;
                    break;
                }

                //print(canBuild);
            }

            HandSlot();

            if (canBuild)
            {
                Vector2Int rotationOffset = absObject.GetRotationOffset(dir);
                //print(rotationOffset);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();

                PlacedObjectAS placedObjectCopy = PlacedObjectAS.Create(placedObjectWorldPosition, new Vector2Int(x, y), dir, absObject);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObjectCopy);
                }

                ClearAbsAndGhost();
                DeleteOldObject();
            }
            else
            {
                print("Cant build here!");
                ClearAbsAndGhost();
            }
        }
        BatteryLogic();
        Inventory();
    }

    public void SetEncounterEnemy(bool b)
    {
        encounterEnemy = b;
    }

    public int GetNumOfBatteries()
    {
        return numOfBatteries;
    }

    public void BatteryLogic()
    {
        /*foreach (Vector2Int hand in handSlots)
        {
            GridObjectAS objectInHandSlot = grid.GetGridObject(hand.x, hand.y);
            PlacedObjectAS placedObjectFromHand = objectInHandSlot.GetPlacedObject();
            if (placedObjectFromHand != null && placedObjectFromHand.GetObjectName() == "Flashlight")
            {
                haveFlashlight = true;
                break;
            }
            haveFlashlight = false;
        }*/

        GridObjectAS objectInBackpack;
        PlacedObjectAS placedObjectInBackpack;
        
        foreach (Vector2Int slot in backpackSlots)
        {
            objectInBackpack = grid.GetGridObject(slot.x, slot.y);
            placedObjectInBackpack = objectInBackpack.GetPlacedObject();

            if (placedObjectInBackpack != null && placedObjectInBackpack.GetObjectName() == "Battery")
            {
                numOfBatterySquares++;
            }
        }
        numOfBatteries = numOfBatterySquares/2;
        numOfBatterySquares = 0;
        //print(numOfBatteries);

        foreach (Vector2Int slot in backpackSlots)
        {
            objectInBackpack = grid.GetGridObject(slot.x, slot.y);
            placedObjectInBackpack = objectInBackpack.GetPlacedObject();

            if (placedObjectInBackpack != null && placedObjectInBackpack.GetObjectName() == "Battery")
            {
                battery = placedObjectInBackpack.GetComponent<BatteryObjAS>();
                batteryLoc = placedObjectInBackpack.GetComponent<AbstractObjectAS>().GetGridPositionList(new Vector2Int(slot.x, slot.y), placedObjectInBackpack.GetDir());
                break;
            }
        }
    }

    public void DeleteBattery()
    {
        if (battery != null && batteryLoc != null)
        {
            if (absObject != null && absObject.GetComponent<PlacedObjectAS>() == battery)
            {
                ClearAbsAndGhost();
                DeleteOldObject();
            }

            battery.GetComponent<PlacedObjectAS>().DestroySelf();
            foreach (Vector2Int batterySpot in batteryLoc)
            {
                grid.GetGridObject(batterySpot.x, batterySpot.y).ClearPlacedObject();
            }
        }
    }

    public void ClearAbsAndGhost()
    {
        absObject = null;
        Destroy(ghostObject.gameObject);
        ghostObject = null;
        ghostFollow = false;
    }

    public void DeleteOldObject()
    {
        placedObjectForDeletion.DestroySelf();
        foreach (Vector2Int deleteCoordinates in coordinatesForDeletion)
        {
            grid.GetGridObject(deleteCoordinates.x, deleteCoordinates.y).ClearPlacedObject();
        }
        coordinatesForDeletion = null;
        placedObjectForDeletion = null;
    }

    public void Inventory()
    {
        inventoryClear = true;
        if (inventoryClear == false)
        {
            //Spawn Set items

        }

        if (Input.GetKeyDown(KeyCode.Space) && inventoryClear == true)
        {
            foreach (Vector2Int coordinate in inventorySlots)
            {
                GridObjectAS gridObject = grid.GetGridObject(coordinate.x, coordinate.y);
                PlacedObjectAS placedObject = gridObject.GetPlacedObject();
                //print(gridObject);
                if (placedObject != null && placedObject.GetObjectName() != "Null Object")
                {
                    if (absObject != null && absObject.GetComponent<PlacedObjectAS>() == placedObject)
                    {
                        ClearAbsAndGhost();
                        DeleteOldObject();
                    }
                    placedObject.DestroySelf();

                }

                grid.GetGridObject(coordinate.x, coordinate.y).ClearPlacedObject();
            }
            
        }
    }

    public void HandSlot()
    {
        foreach (Vector2Int hand in handSlots)
        {
            grid.GetXY(Mouse3DAS.GetMouseWorldPosition(), out int x, out int y);
            Vector2Int mousePosOnGrid = new Vector2Int(x, y);
            if (absObject != null && hand == mousePosOnGrid && absObject.GetObjType() != AbstractObjectAS.Typ.Action)
            {
                canBuild = false;
                break;
            }
        }
    }

    public void ConsumeHand()
    {
        //consume item
        foreach (Vector2Int coordinate in handSlots)
        {
            GridObjectAS gridObject = grid.GetGridObject(coordinate.x, coordinate.y);
            PlacedObjectAS placedObject = gridObject.GetPlacedObject();
            if (placedObject != null && placedObject.GetObjectName() != "Null Object") { placedObject.DestroySelf(); }

            grid.GetGridObject(coordinate.x, coordinate.y).ClearPlacedObject();
        }
    }

    public void TrashSlot()
    {
        grid.GetXY(Mouse3DAS.GetMouseWorldPosition(), out int x, out int y);
        Vector2Int mousePosOnGrid = new Vector2Int(x,y);
        if (absObject != null && trashSlot[0] == mousePosOnGrid)
        {
            print("trash");
            //ClearAbsAndGhost();
            DeleteOldObject();
        }
    }
}
