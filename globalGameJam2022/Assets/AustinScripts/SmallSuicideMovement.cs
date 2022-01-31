using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallSuicideMovement : MonoBehaviour
{
    public bool isJumping;
    public float fallingSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isJumping)
        {
            transform.Translate(Vector3.down * fallingSpeed * Time.deltaTime);
        }
    }
    public void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
        }
 
    }
}
