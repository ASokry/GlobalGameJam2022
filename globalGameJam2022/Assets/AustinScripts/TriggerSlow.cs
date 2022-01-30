using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSlow : MonoBehaviour
{
    public PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerMovement.curMovementSpeed = playerMovement.slowMovementSpeed;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerMovement.curMovementSpeed = playerMovement.fastMovementSpeed;
    }
}
