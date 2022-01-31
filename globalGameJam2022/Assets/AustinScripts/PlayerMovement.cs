using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float slowMovementSpeed;
    public float fastMovementSpeed;
    public float curMovementSpeed;
    public bool stopped;
    [Header("InventoryItems")]
    public int bullets;

    [Header("Animation")]
    public Animator animator;

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
            animator.SetBool("isStopped", false);
        }
        else
        {
            animator.SetBool("isStopped", true);
        }

    }
    public void Attacked(GameObject enemy)
    {
        print("Attacked");
        if(bullets >= 0)
        {
            enemy.SendMessage("Die");
            bullets--;
        }
        if(bullets <= 0)
        {
            Dead();
        }
    }
    void Dead()
    {
        SceneManager.LoadScene(2);
    }
    public void End()
    {
        curMovementSpeed = 0;
    }
}
