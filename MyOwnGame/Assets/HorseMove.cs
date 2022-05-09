using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HorseMove : MonoBehaviour
{
    public Canvas Canvas;   
    public GameObject horseCamera;
    private RawImage joyStick;
    private RawImage joyStickBG;
    private Vector2 Touch2startPos;
    private Vector2 JScenter;
    private Vector3 startRotate;
    public float horseMoveSpeed = 1;
    public float horseRotateSpeed = 1;
    private bool checkHeight = false;

    void Start()
    {
        GameObject BackJoyStick = Canvas.GetComponentsInChildren<Transform>()[1].gameObject;

        Canvas.GetComponent<CanvasScaler>().scaleFactor = Screen.height / 720f;
        //JoyStick轉換裝置Bug修正(螢幕縮放)

        joyStickBG = BackJoyStick.GetComponentsInChildren<Transform>()[1].GetComponent<RawImage>();
        joyStick = joyStickBG.GetComponentsInChildren<Transform>()[1].GetComponent<RawImage>();
        //設定JoyStick和JoyStickBG

        joyStick.rectTransform.position = joyStickBG.rectTransform.position; //歸正JoyStick
        JScenter = joyStickBG.rectTransform.anchoredPosition * Screen.height / 720f; //設定JoyStick中心 +JoyStick轉換裝置Bug修正(中心改變)
    }

    void FixedUpdate()
    {
        if (this.transform.position.y > 0.38f)
            if (!checkHeight)
            {
                checkHeight = true;
                startRotate = this.transform.rotation.eulerAngles;
            }
               
        if (this.transform.position.y < 0.38f)
        {
            if (checkHeight)
            {
                if (this.transform.rotation.eulerAngles.x - startRotate.x > 90 | this.transform.rotation.eulerAngles.x - startRotate.x < 359)
                    this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y + 180, 0f);
                if (this.transform.rotation.eulerAngles.z - startRotate.z > 90| this.transform.rotation.eulerAngles.z - startRotate.z < 359)
                    this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y - 180, 0f);
                if (this.transform.rotation.eulerAngles.x - startRotate.x <= -90 | this.transform.rotation.eulerAngles.x - startRotate.x > -359)
                    this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y - 180, 0f);
                if (this.transform.rotation.eulerAngles.z - startRotate.z <= -90 | this.transform.rotation.eulerAngles.z - startRotate.z > -359)
                    this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y + 180, 0f);           
            }
            checkHeight = false;
        }
        //control fix

        horseCamera.transform.position = new Vector3(this.transform.position.x, 50f, this.transform.position.z);
        if (this.transform.position.y < 0.37501f)
            horseCamera.transform.eulerAngles = new Vector3(90f, this.transform.eulerAngles.y + 90f, 0f);
        //cameraTransform + Rotate

        if (Input.multiTouchEnabled)
        {
            int touchnum = Input.touchCount; 
            if (touchnum == 0)
            {
                joyStick.rectTransform.position = joyStickBG.rectTransform.position;
            }
            else
            {
                Vector2 touch1 = Input.GetTouch(0).position;

                Vector2 joystickMove = touch1 - JScenter;

                float moveDistance = Vector2.Distance(new Vector2(0f, 0f), joystickMove);
                if (moveDistance <= 40)
                {
                    this.transform.Translate(joystickMove.y*horseMoveSpeed*Time.deltaTime, 0f, -joystickMove.x*horseMoveSpeed*Time.deltaTime,Space.Self);
                    joyStick.rectTransform.anchoredPosition = joystickMove;
                }
                else
                {
                    joystickMove.Normalize();

                    this.transform.Translate(joystickMove.y *40* horseMoveSpeed*Time.deltaTime, 0f, -joystickMove.x *35* horseMoveSpeed*Time.deltaTime,Space.Self);
                    joyStick.rectTransform.anchoredPosition = joystickMove * 40;
                }
                //joystick + horseMove 

                if(touchnum == 2)
                {
                    switch (Input.touches[1].phase)
                {
                    case TouchPhase.Began:
                        Touch2startPos = Input.GetTouch(1).position;
                        break;
                    case TouchPhase.Moved:
                        Vector2 lastPos = Input.GetTouch(1).position;
                        Vector2 handMove = lastPos - Touch2startPos;

                        this.transform.Rotate(0f, handMove.x*horseRotateSpeed*Time.deltaTime, 0f);

                        Touch2startPos = Input.GetTouch(1).position;
                        break;
                }
                }
                //horseRotate
            }
        }
    }
}
