using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemAS : MonoBehaviour
{
    [SerializeField] private List<AbstractObjectAS> absObjectList;
    private AbstractObjectAS absObject;

    private PlacedObjectAS placedObjectForDeletion;
    private List<Vector2Int> coordinatesForDeletion;
    private AbstractObjectAS.Dir lastDir = AbstractObjectAS.Dir.Down;
    private Vector3 lastOrigin;

    public List<Vector2Int> nullZones;

    private Transform ghostObject;
    private bool ghostFollow = false;

    private GameGridAS<GridObjectAS> grid;
    private AbstractObjectAS.Dir dir = AbstractObjectAS.Dir.Down;

    public int gridWidth = 60;
    public int gridHeight = 18;
    public float cellSize = 3f;
    public GameObject gridTile;

    public Vector2Int nullOffsetPoint;
    private List<Vector2Int> nullSpotsList = new List<Vector2Int>();
    private List<Vector2Int> lootBoxList = new List<Vector2Int>();

    private void Awake()
    {
        grid = new GameGridAS<GridObjectAS>(gridWidth, gridHeight, cellSize, Vector3.zero, (GameGridAS<GridObjectAS> g, int x, int y) => new GridObjectAS(g, x, y));

        for (int x = 0; x < gridHeight; x++)
        {
            nullSpotsList.Add(nullOffsetPoint + new Vector2Int(0, x));
        }

        int lootBoxWidth = gridWidth - (nullOffsetPoint.x + 1);
        int lootBoxHeight = gridHeight - (nullOffsetPoint.y + 1);
        for (int lx = 0; lx < lootBoxWidth; lx++)
        {
            for (int ly = 0; ly < lootBoxHeight; ly++)
            {
                lootBoxList.Add(nullOffsetPoint + new Vector2Int(lx + 1, ly + 1));
            }
        }
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

    private void Start()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject tiles = Instantiate(gridTile, grid.GetWorldPosition(x, y), Quaternion.identity);
            }
        }

        Vector2Int rotationOffset = new Vector2Int(0, 0);
        foreach (Vector2Int coordinate in nullZones)
        {
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(coordinate.x, coordinate.y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();

            PlacedObjectAS placedObject = PlacedObjectAS.Create(placedObjectWorldPosition, new Vector2Int(coordinate.x, coordinate.y), dir, absObjectList[0]);

            grid.GetGridObject(coordinate.x, coordinate.y).SetPlacedObject(placedObject);
        }

        ////
        Vector3 firstItemWorldPosition = grid.GetWorldPosition(18, 3) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();

        PlacedObjectAS firstItem = PlacedObjectAS.Create(firstItemWorldPosition, new Vector2Int(18, 3), dir, absObjectList[1]);

        grid.GetGridObject(18, 3).SetPlacedObject(firstItem);

        List<Vector2Int> firstItemPositionList = absObjectList[1].GetGridPositionList(new Vector2Int(18, 3), dir);
        foreach (Vector2Int gridPosition in firstItemPositionList)
        {
            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(firstItem);
        }
        /////
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

    private void Update()
    {
        if (Input.GetMouseButton(0) && (Mouse3DAS.GetMouseWorldPosition().x > 0 && Mouse3DAS.GetMouseWorldPosition().y > 0))
        {
            GridObjectAS gridObject = grid.GetGridObject(Mouse3DAS.GetMouseWorldPosition());
            PlacedObjectAS placedObject = gridObject.GetPlacedObject();

            //Replace with right click
            if (Input.GetMouseButtonDown(1))
            {
                dir = AbstractObjectAS.GetNextDir(dir);
                print(dir);
            }

            if (placedObject != null && placedObject.GetObjectName() != "Null Object")
            {
                ghostFollow = true;

                //Debug.Log(placedObject.GetObjectName());
                absObject = placedObject.GetComponent<AbstractObjectAS>();

                placedObjectForDeletion = placedObject;
                coordinatesForDeletion = placedObjectForDeletion.GetGridPositionList();
            }

            GhostFollow(absObject);
        }
        else if (Input.GetMouseButton(0) && absObject != null)
        {
            print("Out Bound!");
            absObject = null;
            Destroy(ghostObject.gameObject);
            ghostObject = null;
            ghostFollow = false;
        }

        if (Input.GetMouseButtonUp(0) && (Mouse3DAS.GetMouseWorldPosition().x > 0 && Mouse3DAS.GetMouseWorldPosition().y > 0) && absObject != null)
        {
            grid.GetXY(Mouse3DAS.GetMouseWorldPosition(), out int x, out int y);

            List<Vector2Int> gridPositionList = absObject.GetGridPositionList(new Vector2Int(x, y), dir);

            bool canBuild = true;
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
                absObject = null;
                Destroy(ghostObject.gameObject);
                ghostObject = null;
                ghostFollow = false;
                
                placedObjectForDeletion.DestroySelf();
                foreach (Vector2Int deleteCoordinates in coordinatesForDeletion)
                {
                    grid.GetGridObject(deleteCoordinates.x, deleteCoordinates.y).ClearPlacedObject();
                }
            }
            else
            {
                print("Cant build here!");
                absObject = null;
                Destroy(ghostObject.gameObject);
                ghostObject = null;
                ghostFollow = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Vector2Int coordinate in lootBoxList)
            {
                GridObjectAS gridObject = grid.GetGridObject(coordinate.x, coordinate.y);
                PlacedObjectAS placedObject = gridObject.GetPlacedObject();
                if (placedObject != null && placedObject.GetObjectName() != "Null Object") { placedObject.DestroySelf(); }

                grid.GetGridObject(coordinate.x, coordinate.y).ClearPlacedObject();
            }
        }
    }
}
