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

    public GridSystemAS gridSys;
    private float curTime2;
    public float batteryUseRate = 20;
    public float batteryUseTime = .05f;
    public float maxBatteryUse;
    public float currentBatteryUse;
    public int batteryAmount = 2;
    // Start is called before the first frame update
    void Start()
    {
        spotLight = gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(batteryAmount > 0)
        {

            Ray ray = new Ray(transform.position, Vector3.right);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                print(hit);
                if (hit.collider.tag == "Enemy")
                {
                    directionToEnemy = hit.transform.position - transform.position;

                    if (Vector3.Angle(directionToEnemy, transform.right) >= spotLight.spotAngle && Vector3.Distance(hit.transform.position, transform.position) <= spotLight.range)
                    {
                        curTime += Time.deltaTime;
                        curTime2 += Time.deltaTime;
                        hit.transform.gameObject.SendMessage("FlashlightSlow", stunRate);
                        if (curTime2 >= batteryUseTime)
                        {
                            currentBatteryUse -= batteryUseRate;
                            if (currentBatteryUse <= 0)
                            {
                                batteryAmount--;
                                currentBatteryUse = maxBatteryUse;
                            }
                            curTime2 = 0;
                        }
                        if (curTime >= damageRate)
                        {
                            hit.transform.gameObject.SendMessage("Damage", damage);
                            curTime = 0;
                        }
                    }
                }
                else
                {
                    gridSys.SetEncounterEnemy(false);
                }
            }
        }
        else
        {
            spotLight.enabled = false;
        }
    
    }

}
