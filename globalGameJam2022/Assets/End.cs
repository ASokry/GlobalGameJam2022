using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject flashlight;
    private float curTime;
    private float endTime = 5;
    public bool startEnd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (startEnd)
        {
            if (curTime >= endTime)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerMovement.curMovementSpeed = 0;
            flashlight.SetActive(false);
            curTime += 0;
            startEnd = true;
        }
    }
}
