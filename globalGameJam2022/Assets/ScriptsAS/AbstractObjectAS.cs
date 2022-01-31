using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractObjectAS : MonoBehaviour
{
    public enum Dir { Down, Left, Up, Right };
    public enum Typ { Action, Passive, None};

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public Typ objType;

    public List<Vector2Int> downCoordinatesList;
    public List<Vector2Int> leftCoordinatesList;
    public List<Vector2Int> upCoordinatesList;
    public List<Vector2Int> rightCoordinatesList;

    public int width;
    public int height;

    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return -90;
            case Dir.Up: return 180;
            case Dir.Right: return -270;
        }
    }

    public virtual Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, 1);
            case Dir.Up: return new Vector2Int(1, 1);
            case Dir.Right: return new Vector2Int(1, 0);
        }
    }

    public virtual Vector3 GetPositionOffset(Dir dir, float cellSize)
    {
        // This is for subtracting the ghost position with the offset
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector3(-cellSize / 2, -cellSize / 2);
            case Dir.Left: return new Vector3(-cellSize / 2, cellSize / 2);
            case Dir.Up: return new Vector3(cellSize / 2, cellSize / 2);
            case Dir.Right: return new Vector3(cellSize / 2, -cellSize / 2);
        }
    }

    public virtual List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
                foreach (Vector2Int coordinate in downCoordinatesList)
                {
                    gridPositionList.Add(offset + coordinate);
                }
                break;
            case Dir.Up:
                foreach (Vector2Int coordinate in upCoordinatesList)
                {
                    gridPositionList.Add(offset + coordinate);
                }
                break;
            case Dir.Left:
                foreach (Vector2Int coordinate in leftCoordinatesList)
                {
                    gridPositionList.Add(offset + coordinate);
                }
                break;
            case Dir.Right:
                foreach (Vector2Int coordinate in rightCoordinatesList)
                {
                    gridPositionList.Add(offset + coordinate);
                }
                break;
        }

        return gridPositionList;
    }

    public Transform GetPrefab()
    {
        return prefab;
    }

    public Typ GetObjType()
    {
        return objType;
    }
}
