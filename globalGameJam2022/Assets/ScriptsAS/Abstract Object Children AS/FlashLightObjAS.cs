using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightObjAS : AbstractObjectAS
{
    public override Vector2Int GetRotationOffset(Dir dir)
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

    public override Vector3 GetPositionOffset(Dir dir, float cellSize)
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

    public override List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
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
}
