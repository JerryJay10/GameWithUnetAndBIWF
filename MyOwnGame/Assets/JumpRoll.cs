using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpRoll : MonoBehaviour
{
    public Button JumpRollButton;
    public float JumpForce = 1f;
    public float rotateSpeed = 30f;
    private float startTime;
    private bool isGround;
    private bool isRotate = false;
    private bool checkjump = false;
    private bool checkcheck = false;

    void Start()
    {
        
    }

    public void OnButtonTick()
    {
        if (isGround)
        {
            isRotate = true;
            this.GetComponent<Rigidbody>().AddForce(0f, JumpForce,0f,ForceMode.Impulse);
            startTime = Time.time;
            checkjump = true;
        }
        
    }
    
    void FixedUpdate()
    {
        if (this.transform.position.y < 0.38f)
        {
            isGround = true;
            JumpRollButton.interactable = true;
        }
        else
        {
            isGround = false;
            JumpRollButton.interactable = false;
        }

        if (isRotate)
        {
            this.transform.Rotate(rotateSpeed*Time.deltaTime, 0f, 0f,Space.Self);
            this.transform.localScale = new Vector3(2.6f, 0.75f, 3.5f);
            float nowtime = Time.time;

            if(nowtime - startTime >= 1)
            {
                isRotate = false;
                this.transform.localScale = new Vector3(2.6f, 0.75f, 1.5f);
            }
        }
        //jumproll

        if (checkjump)
            if (!isGround)
                checkcheck = true;
        if(isGround)
            if (checkcheck)
            {
                this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y - 180, 0f);
                checkcheck = false;
                checkjump = false;
            }               
        //roll fix
    }
}
