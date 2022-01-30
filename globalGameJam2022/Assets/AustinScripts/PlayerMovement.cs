using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float slowMovementSpeed;
    public float fastMovementSpeed;
    public float curMovementSpeed;
    public bool stopped;
    // Start is called before the first frame update
    void Start()
    {
        curMovementSpeed = fastMovementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            transform.Translate(Vector3.right * curMovementSpeed * Time.deltaTime);
        }

    }
}
