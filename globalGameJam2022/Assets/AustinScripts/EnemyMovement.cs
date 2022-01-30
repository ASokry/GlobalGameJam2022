using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float fastMovementSpeed;
    public float slowMovementSpeed;
    public float curMovementSpeed;
    public float health;
    public float curHealth;
    public GameObject player;
    //private PlayerMovement playerMovement;
    private float curTime;
    public GameObject levelManager;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //playerMovement = player.GetComponent<PlayerMovement>();
        curMovementSpeed = fastMovementSpeed;
        curHealth = health;
        levelManager = GameObject.FindGameObjectWithTag("LevelManager");
        levelManager.GetComponent<LevelManager>().enemies.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit leftHit;
        RaycastHit rightHit;

        Ray leftRay = new Ray(transform.position, Vector3.left);
        Ray rightRay = new Ray(transform.position, Vector3.right);
        Debug.DrawRay(transform.position, Vector3.left, Color.red);
        Debug.DrawRay(transform.position, Vector3.right, Color.green);

        if (Physics.Raycast(leftRay, out leftHit))
        {
            if(leftHit.collider.tag == "Player")
            {
                transform.Translate(Vector3.left * curMovementSpeed * Time.deltaTime);
            }
        }
        if(Physics.Raycast(rightRay, out rightHit))
        {
            if(rightHit.collider.tag == "Player")
            {
                print("works");
                transform.Translate(Vector3.right * curMovementSpeed * Time.deltaTime);
            }
        }

        if(curHealth <= 0)
        {
            levelManager.GetComponent<LevelManager>().enemies.Remove(gameObject);
            Destroy(gameObject);
        }

    }
    void FlashlightSlow(float stunRate)
    {
        curTime += Time.deltaTime;
        print("Enemy Slowed");
        if(curTime <= stunRate)
        {
            curMovementSpeed = Mathf.Lerp(fastMovementSpeed, slowMovementSpeed, curTime/stunRate);
        }
        else
        {
            curMovementSpeed = slowMovementSpeed;
        }

    }

    void Damage(float damage)
    {
        curHealth -= damage;
    }
}
