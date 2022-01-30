using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    //public List <EnemyMovement> enemyMovements;
    public float damage;
    public float damageRate;
    public float stunRate;
    private Light spotLight;
    private Vector3 directionToEnemy;
    public LevelManager levelManager;
    private float curTime;
    // Start is called before the first frame update
    void Start()
    {
        spotLight = gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        

        Ray ray = new Ray(transform.position, Vector3.right);
        RaycastHit hit;
      

        if(Physics.Raycast(ray, out hit))
        {
            print(hit);
            if (hit.collider.tag == "Enemy")
            {
                directionToEnemy = hit.transform.position - transform.position;

                if (Vector3.Angle(directionToEnemy, transform.right) >= spotLight.spotAngle && Vector3.Distance(hit.transform.position, transform.position) <= spotLight.range)
                {
                    curTime += Time.deltaTime;
                    hit.transform.gameObject.SendMessage("FlashlightSlow", stunRate);
                    if(curTime >= damageRate)
                    {
                        hit.transform.gameObject.SendMessage("Damage", damage);
                        curTime = 0;
                    }
                }
            }
        }
    }

}
