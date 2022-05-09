using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TogetherChooseRole : NetworkBehaviour
{
    private Text[] HorseText = new Text[4];
    private Text[] ShooterText = new Text[4];

    ///////////伺服器///////////

    public bool[] HasChooseHorse = new bool[4];
    public bool[] HasChooseShooter = new bool[4];

    private Color[] Colors = new Color[4];

    private void Start()
    {
        if (isServer)
        {
            HorseText[0] = GameObject.Find("RedHorse").GetComponent<Text>();
            HorseText[1] = GameObject.Find("BlueHorse").GetComponent<Text>();
            HorseText[2] = GameObject.Find("GreenHorse").GetComponent<Text>();
            HorseText[3] = GameObject.Find("PurpleHorse").GetComponent<Text>();
            ShooterText[0] = GameObject.Find("RedShooter").GetComponent<Text>();
            ShooterText[1] = GameObject.Find("BlueShooter").GetComponent<Text>();
            ShooterText[2] = GameObject.Find("GreenShooter").GetComponent<Text>();
            ShooterText[3] = GameObject.Find("PurpleShooter").GetComponent<Text>();

            Colors[0] = Color.red;
            Colors[1] = Color.blue;
            Colors[2] = Color.green;
            Colors[3] = Color.magenta;
        }
        //伺服器設定好Text的UI + 設定好Colors    
    }

    private void Update()
    {
        if (isServer)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (HasChooseHorse[i])
                    HorseText[i].color = Colors[i];
                else
                    HorseText[i].color = Color.black;
            }
            for (int i = 0; i <= 3; i++)
            {
                if (HasChooseShooter[i])
                    ShooterText[i].color = Colors[i];
                else
                    ShooterText[i].color = Color.black;
            }
        }        
    }
    //設定伺服器選項顏色(偵測用)

    [ClientRpc]
    public void RpcOnHorseChange(int i)
    {
        HasChooseHorse[i] = true;
    }
    [ClientRpc]
    public void RpcOnShooterChange(int i)
    {
        HasChooseShooter[i] = true;
    }
    [ClientRpc]
    public void RpcClearChooseRole(int MyRoleNum)
    {
        switch (MyRoleNum / 10)
        {
            case 1:
                HasChooseHorse[MyRoleNum - 10] = false;
                break;
            case 2:
                HasChooseShooter[MyRoleNum - 20] = false;
                break;
        }
    }
    //讓Client端的HasChooseRole同步
    
    [ClientRpc]
    public void RpcOnPlayButtonClick()
    {
        SceneManager.LoadScene(3);
    }
    //有人按開始時，一起到下個場景
}
