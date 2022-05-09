using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterMove : MonoBehaviour
{    
    public GameObject shooterCamera;
    public Canvas Canvas;
    private RawImage joyStick;
    private RawImage joyStickBG;
    private bool CanMove = true;
    private Vector2 JScenter;
    private Vector2 Touch1startPos;
    private Vector2 Touch2startPos;
    public float shooterMoveSpeed = 1;
    public float shooterRotateSpeed = 1;

    void Start()
    {
        GameObject BackJoyStick = Canvas.GetComponentsInChildren<Transform>()[1].gameObject;
        
        Canvas.GetComponent<CanvasScaler>().scaleFactor = Screen.height / 720f;
        //JoyStick轉換裝置Bug修正(螢幕縮放)

        joyStickBG = BackJoyStick.GetComponentsInChildren<Transform>()[1].GetComponent<RawImage>();
        joyStick = joyStickBG.GetComponentsInChildren<Transform>()[1].GetComponent<RawImage>();
        //設定JoyStick和JoyStickBG

        joyStick.rectTransform.position = joyStickBG.rectTransform.position; //歸正JoyStick
        JScenter = joyStickBG.rectTransform.anchoredPosition *Screen.height/720f; //設定JoyStick中心 +JoyStick轉換裝置Bug修正(中心改變)
        shooterCamera.transform.rotation = this.transform.rotation; //歸正CameraRotation        
    }

    private void FixedUpdate()
    {
        shooterCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
        //cameraTransform

        if (Input.multiTouchEnabled)
        {
            int touchnum = Input.touchCount;
            if (touchnum == 0)
            {
                joyStick.rectTransform.position = joyStickBG.rectTransform.position;
            }
            else
            {
                Vector2 touch1 = Input.touches[0].position;

                if (touch1.x > Screen.width/2)
                {
                    switch (Input.touches[0].phase)
                    {
                        case TouchPhase.Began:
                            Touch1startPos = Input.GetTouch(0).position;
                            break;
                        case TouchPhase.Moved:
                            Vector2 lastPos = Input.GetTouch(0).position;
                            Vector2 handMove = lastPos - Touch1startPos;

                            shooterCamera.transform.Rotate(-handMove.y * shooterRotateSpeed * Time.deltaTime, handMove.x * shooterRotateSpeed * Time.deltaTime, 0f);
                            shooterCamera.transform.eulerAngles = new Vector3(shooterCamera.transform.rotation.eulerAngles.x, shooterCamera.transform.rotation.eulerAngles.y, 0f); //freeze z Rotate 
                            this.transform.eulerAngles = new Vector3(0f, shooterCamera.transform.rotation.eulerAngles.y, 0f);

                            Touch1startPos = Input.GetTouch(0).position;
                            break;
                    }
                }
                else
                {
                    Vector2 joystickMove = touch1 - JScenter;

                    float moveDistance = Vector2.Distance(new Vector2(0f, 0f), joystickMove);

                    if (CanMove)
                    {
                        if (moveDistance <= 40)
                        {
                            this.transform.Translate(joystickMove.x * shooterMoveSpeed * Time.deltaTime, 0f, joystickMove.y * shooterMoveSpeed * Time.deltaTime, Space.Self);
                            joyStick.rectTransform.anchoredPosition = joystickMove;
                        }
                        else
                        {
                            joystickMove.Normalize();

                            this.transform.Translate(joystickMove.x * 40 * shooterMoveSpeed * Time.deltaTime, 0f, joystickMove.y * 40 * shooterMoveSpeed * Time.deltaTime, Space.Self);
                            joyStick.rectTransform.anchoredPosition = joystickMove * 40;
                        }
                    }
                }
                //TouchNum == 1時 : 停止時CameraRotate ; 移動時移動 + joystick

                if (touchnum == 2)
                {
                    switch (Input.touches[1].phase)
                    {
                        case TouchPhase.Began:
                            Touch2startPos = Input.GetTouch(1).position;
                            break;
                        case TouchPhase.Moved:
                            Vector2 lastPos = Input.GetTouch(1).position;
                            Vector2 handMove = lastPos - Touch2startPos;

                            shooterCamera.transform.Rotate(-handMove.y * shooterRotateSpeed*Time.deltaTime, handMove.x * shooterRotateSpeed*Time.deltaTime, 0f);
                            shooterCamera.transform.eulerAngles = new Vector3(shooterCamera.transform.rotation.eulerAngles.x, shooterCamera.transform.rotation.eulerAngles.y, 0f); //freeze z Rotate 
                            this.transform.eulerAngles = new Vector3(0f, shooterCamera.transform.rotation.eulerAngles.y, 0f);

                            Touch2startPos = Input.GetTouch(1).position;
                            break;
                    }
                }
                //shooterRotate + CameraRotate
            }
        }
    }

    public void Roll()
    {
        CanMove = false;
    }
    public void FinishRoll()
    {
        CanMove = true;
    }
}
