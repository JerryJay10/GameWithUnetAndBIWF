using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollButton : MonoBehaviour
{
    public Button RollLeftButton;
    public Button RollRightButton;
    public float RollSpeed = 0.04f;
    private bool lockRotate = true;

    public void OnRollRightButton()
    {
        StartCoroutine(RollRight());
    }
    IEnumerator RollRight()
    {
        lockRotate = false;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<ShooterMove>().Roll();
        RollRightButton.interactable = false;

        this.GetComponent<Rigidbody>().AddForce(this.transform.right*RollSpeed, ForceMode.Impulse);      
        for (float i = 1; i<= 20; i++)
        {            
            this.transform.Rotate(0f, 0f, 9f);
            yield return new WaitForFixedUpdate();
        }

        lockRotate = true;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<ShooterMove>().FinishRoll();
        RollRightButton.interactable = true;
    }
    //RollRightButton

    public void OnRollLeftButton()
    {
       StartCoroutine(RollLeft());
    }
    IEnumerator RollLeft()
    {
        lockRotate = false;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<ShooterMove>().Roll();
        RollLeftButton.interactable = false;

        this.GetComponent<Rigidbody>().AddForce(-this.transform.right * RollSpeed, ForceMode.Impulse);
        for (int i = 1; i <= 20; i++)
        {
            this.transform.Rotate(0f, 0f, -9f);
            yield return new WaitForFixedUpdate();
        }

        lockRotate = true;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<ShooterMove>().FinishRoll();
        RollLeftButton.interactable = true;
    }
    //RollLeftButton

    void Update()
    {
        if (lockRotate)
            this.transform.eulerAngles = new Vector3(0f, this.transform.rotation.eulerAngles.y, 0f);
        //freeze Rotate
    }
}
