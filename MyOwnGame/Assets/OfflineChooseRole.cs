using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class OfflineChooseRole : MonoBehaviour
{
    private bool HasSetUI = false;

    private Toggle[] Horse = new Toggle[4];
    private Toggle[] Shooter = new Toggle[4];

    public Color[] Colors = new Color[4];

    public Vector3[] SpawnPoint = new Vector3[4];
    public Vector3[] SpawnRotate = new Vector3[4];

    private OfflineControlRole OfflineControlRole;

    public int MyRole = 0;   //十位 : 1 -> 馬 ; 2 -> 射手  //個位 : 顏色數字 

    public bool HasSpawnRole = false;

    public GameObject HorseRole;
    public GameObject ShooterRole;

    ///////////客戶端///////////

    public bool[] HasChooseHorse = new bool[4];
    public bool[] HasChooseShooter = new bool[4]; 


    private void SetStartUI()
    {
        OfflineControlRole = this.GetComponent<OfflineControlRole>();
        //設定好OfflineControlRole，省流量

        Colors[0] = Color.red;
        Colors[1] = Color.blue;
        Colors[2] = Color.green;
        Colors[3] = Color.magenta;
        //設定好Colors


         Horse[0] = GameObject.Find("RedHorse").GetComponent<Toggle>();
         Horse[1] = GameObject.Find("BlueHorse").GetComponent<Toggle>();
         Horse[2] = GameObject.Find("GreenHorse").GetComponent<Toggle>();
         Horse[3] = GameObject.Find("PurpleHorse").GetComponent<Toggle>();
         Shooter[0] = GameObject.Find("RedShooter").GetComponent<Toggle>();
         Shooter[1] = GameObject.Find("BlueShooter").GetComponent<Toggle>();
         Shooter[2] = GameObject.Find("GreenShooter").GetComponent<Toggle>();
         Shooter[3] = GameObject.Find("PurpleShooter").GetComponent<Toggle>();

         for (int i = 0; i <= 3; i++)
         {
            Horse[i].onValueChanged.RemoveAllListeners();
            Horse[i].onValueChanged.AddListener(OnHorseChange);

            Shooter[i].onValueChanged.RemoveAllListeners();
            Shooter[i].onValueChanged.AddListener(OnShooterChange);
         }

        GameObject.Find("PlayButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(OnPlayButtonClick);

        DontDestroyOnLoad(this.gameObject);
        //客戶端套好Toggle + 設好Toggle的UI + 設好開始鈕的UI + 別破壞此物        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "INRoomUI")
        {
            if(!HasSetUI)
            {
                SetStartUI();
                HasSetUI = true;
            }

            for (int i = 0; i <= 3; i++)
            {
                if (MyRole == 10 + i)
                {
                    Horse[i].interactable = true;
                    Horse[i].GetComponentsInChildren<Transform>()[1].GetComponent<Text>().color = Colors[i];
                }
                else
                {
                    if (HasChooseHorse[i])
                    {
                        Horse[i].interactable = false;
                        Horse[i].GetComponentsInChildren<Transform>()[1].GetComponent<Text>().color = Color.gray;
                    }
                    else
                    {
                        Horse[i].interactable = true;
                        Horse[i].GetComponentsInChildren<Transform>()[1].GetComponent<Text>().color = Color.white;
                    }
                }
            

                if (MyRole == 20 + i)
                {
                     Shooter[i].interactable = true;
                     Shooter[i].GetComponentsInChildren<Transform>()[1].GetComponent<Text>().color = Colors[i];
                }
                else
                {
                    if (HasChooseShooter[i])
                    {
                        Shooter[i].interactable = false;
                        Shooter[i].GetComponentsInChildren<Transform>()[1].GetComponent<Text>().color = Color.gray;
                    }
                    else
                    {
                        Shooter[i].interactable = true;
                        Shooter[i].GetComponentsInChildren<Transform>()[1].GetComponent<Text>().color = Color.white;
                    }
                }

            }
        }
        else
        {
            if (!HasSpawnRole)
            {
                SpawnRole();
                HasSpawnRole = true;
            }
        }
        //設定客戶端選項顏色(與可互動性有關) + 轉換場景時召喚MyRole     
    }

    public void OnHorseChange(bool toooya)
    {
        if (MyRole != 0)
            ClearChooseRole();
        for (int i = 0; i <= 3; i++)
        {
            if (Horse[i].isOn)
            {
                HasChooseHorse[i] = true;
                MyRole = 10 + i;
            }
        }
    }
   
    public void OnShooterChange(bool sanyaaa)
    {
        if (MyRole != 0)
            ClearChooseRole();
        for (int i = 0; i <= 3; i++)
        {
            if (Shooter[i].isOn)
            {
                HasChooseShooter[i] = true;
                MyRole = 20 + i;
            }
        }
    }
    
    private void ClearChooseRole()
    {
        int MyRoleNum = MyRole;
        MyRole = 0;
        switch (MyRoleNum / 10)
        {
            case 1:
                Horse[MyRoleNum - 10].isOn = false;
                HasChooseHorse[MyRoleNum - 10] = false;
                break;
            case 2:
                Shooter[MyRoleNum - 20].isOn = false;
                HasChooseShooter[MyRoleNum - 20] = false;
                break;
        }
    }  
    //此可改時必為 白轉顏 或 顏轉白 ， 設定MyRole與HasChoose ， 變顏色給Update處理  +  選前清除選過的角色

    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene(3);
        SpawnRole();
    }
    //有人按開始時，到下個場景

    public void SpawnRole()
    {           
        switch (MyRole / 10)
        {
            case 1:
                GameObject HorseRole_Clone = Instantiate(HorseRole);
                DontDestroyOnLoad(HorseRole_Clone);//可能在LoadScene前就Spawn了，所以要DontDestoryOnLoad (之後用Destory毀他)
                HorseRole_Clone.transform.position = SpawnPoint[MyRole - 10];
                HorseRole_Clone.transform.eulerAngles = SpawnRotate[MyRole - 10];
                OfflineControlRole.SpawnRole(HorseRole_Clone);
                HorseRole_Clone.GetComponent<Renderer>().material.color = Colors[MyRole - 10];
                break;
            case 2:
                GameObject ShooterRole_Clone = Instantiate(ShooterRole);
                DontDestroyOnLoad(ShooterRole_Clone);//可能在LoadScene前就Spawn了，所以要DontDestoryOnLoad (之後用Destory毀他)
                ShooterRole_Clone.transform.position = SpawnPoint[MyRole - 20];
                ShooterRole_Clone.transform.eulerAngles = SpawnRotate[MyRole - 20];
                OfflineControlRole.SpawnRole(ShooterRole_Clone);
                ShooterRole_Clone.GetComponent<Renderer>().material.color = Colors[MyRole - 20];
                break;
        }
    }
    //召喚MyRole 
}