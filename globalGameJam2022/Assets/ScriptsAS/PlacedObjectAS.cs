using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectAS : MonoBehaviour
{
    public static PlacedObjectAS Create(Vector3 worldPosition, Vector2Int origin, AbstractObjectAS.Dir dir, AbstractObjectAS absObject)
    {
        Transform placedObjectTransform = Instantiate(absObject.GetPrefab(), worldPosition, Quaternion.Euler(0, 0, absObject.GetRotationAngle(dir)));

        PlacedObjectAS placedObject = placedObjectTransform.GetComponent<PlacedObjectAS>();

        placedObject.absObject = absObject;
        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.objectName = absObject.nameString;

        return placedObject;
    }

    private AbstractObjectAS absObject;
    private Vector2Int origin;
    private AbstractObjectAS.Dir dir;
    private string objectName;

    public List<Vector2Int> GetGridPositionList()
    {
        return absObject.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public string GetObjectName()
    {
        return objectName;
    }
}
