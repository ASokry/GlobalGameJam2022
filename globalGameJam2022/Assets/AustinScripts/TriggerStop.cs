using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStop : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement playerMovement;
    public LevelManager levelManager;

    public bool isTimedStop;
    public bool isCombatEncounter;
    //private int initialEnemyAmount;

    public float stopTime;
    private float curTime;
    private int timeReset;
    //private int combatEncounterReset;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimedStop && isCombatEncounter && playerMovement.stopped)
        {
            if(levelManager.enemies.Count <= 0)
            {
                playerMovement.stopped = false;
            }
        }

        if (isTimedStop)
        {
            curTime += Time.deltaTime;

            if (playerMovement.stopped && curTime >= stopTime)
            {
                playerMovement.stopped = false;
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!isTimedStop && isCombatEncounter)
            {
                //initialEnemyAmount = levelManager.enemies.Count;
                playerMovement.stopped = true;
            }

            if (isTimedStop)
            {
                if (timeReset == 0)
                {
                    curTime = 0;
                    timeReset++;
                }
                playerMovement.stopped = true;
            }
        }
    }
}
