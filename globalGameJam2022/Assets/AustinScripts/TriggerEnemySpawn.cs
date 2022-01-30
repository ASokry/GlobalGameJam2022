using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemySpawn : MonoBehaviour
{
    public GameObject enemy;
    public int maxSpawnAmount;
    public int minSpawnAmount;
    private int chosenSpawnAmount;
    public Transform[] spawnLocations;
    private int chosenSpawnLocation;
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
        if (other.tag == "Player")
        {
            if(maxSpawnAmount != minSpawnAmount)
            {
                chosenSpawnAmount = Random.Range(minSpawnAmount, maxSpawnAmount+1);
            }
            for (int i = 0; i < chosenSpawnAmount; i++)
            {
                chosenSpawnLocation = Random.Range(0, spawnLocations.Length);

                Instantiate(enemy, spawnLocations[chosenSpawnLocation].position, spawnLocations[chosenSpawnLocation].rotation);
            }

        }
    }
}
