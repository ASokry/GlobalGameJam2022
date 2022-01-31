using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3DAS : MonoBehaviour
{
    public static Mouse3DAS Instance { get; private set; }

    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();
    private Vector2 lastPos;

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Physics.Raycast vs Physic2D.Raycast
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            transform.position = raycastHit.point;
        }
    }

    public static Vector3 GetMouseWorldPosition() => Instance.GetMouseWorldPosition_Instance();

    private Vector3 GetMouseWorldPosition_Instance()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            lastPos = raycastHit.point;
            return raycastHit.point;
        }
        else
        {
            return lastPos;
            //return new Vector3(0, 0);
            //return new Vector3(-10,-10);
        }
    }
}
