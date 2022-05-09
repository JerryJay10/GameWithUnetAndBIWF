using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ChooseRole : NetworkBehaviour
{
    private Toggle[] Horse = new Toggle[4];
    private Toggle[] Shooter = new Toggle[4];

    public Color[] Colors = new Color[4];

    public Vector3[] SpawnPoint = new Vector3[4];
    public Vector3[] SpawnRotate = new Vector3[4];

    private TogetherChooseRole TogetherChooseRole;
    private ServerNetworkManager ServerNetworkManager;
    private ControlRole ControlRole;

    [SyncVar]
    public int MyRole = 0;   //十位 : 1 -> 馬 ; 2 -> 射手  //個位 : 顏色數字 

    [SyncVar]
    public bool HasSpawnRole = false;

    public GameObject HorseRole;
    public GameObject ShooterRole;

    ///////////客戶端///////////
 
    public bool[] HasChooseHorse = new bool[4];      
    public bool[] HasChooseShooter = new bool[4]; //因為SyncVar不能用在陣列，只能用ClientRpc和Command同步他們


    private void Start()
    {
        TogetherChooseRole = GameObject.FindGameObjectWithTag("ConnectTogetherStuff").GetComponent<TogetherChooseRole>();
        ControlRole = this.GetComponent<ControlRole>();
        //設定好TogetherChooseRole和ControlRole，省流量

        Colors[0] = Color.red;
        Colors[1] = Color.blue;
        Colors[2] = Color.green;
        Colors[3] = Color.magenta;
        //設定好Colors

        if (isServer)
        {
            HasChooseHorse = TogetherChooseRole.HasChooseHorse;
            HasChooseShooter = TogetherChooseRole.HasChooseShooter;

            ServerNetworkManager = GameObject.Find("NetworkManager").GetComponent<ServerNetworkManager>();
        }
        //Server統一HasChoose系列 + 設定好ServerNetworkManager

        if (!isLocalPlayer)
            return;
        //不讓玩家程式互相干擾

        HasChooseHorse = TogetherChooseRole.HasChooseHorse;
        HasChooseShooter = TogetherChooseRole.HasChooseShooter;
        //Client統一HasChoose系列，此時TogetherChooseRole的一改，ChooseRole的會更著改 ，反之亦如是

        if (isClient)
        {
            Horse[0] = GameObject.Find("RedHorse").GetComponent<Toggle>();
            Horse[1] = GameObject.Find("BlueHorse").GetComponent<Toggle>();
            Horse[2] = GameObject.Find("GreenHorse").GetComponent<Toggle>();
            Horse[3] = GameObject.Find("PurpleHorse").GetComponent<Toggle>();
            Shooter[0] = GameObject.Find("RedShooter").GetComponent<Toggle>();
            Shooter[1] = GameObject.Find("BlueShooter").GetComponent<Toggle>();
            Shooter[2] = GameObject.Find("GreenShooter").GetComponent<Toggle>();
            Shooter[3] = GameObject.Find("PurpleShooter").GetComponent<Toggle>();

            for(int i = 0; i <= 3; i++)
            {
                Horse[i].onValueChanged.RemoveAllListeners();
                Horse[i].onValueChanged.AddListener(OnHorseChange);

                Shooter[i].onValueChanged.RemoveAllListeners();
                Shooter[i].onValueChanged.AddListener(OnShooterChange);
            }
    
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(OnPlayButtonClick);

            DontDestroyOnLoad(this.gameObject);         
        }
        //客戶端套好Toggle + 設好Toggle的UI + 設好開始鈕的UI + 別破壞此物        
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        //不讓玩家程式互相干擾

        if (isClient)
        {
            if(SceneManager.GetActiveScene().name == "INRoomUI")
            {
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
                    CmdSpawnRole();
                    HasSpawnRole = true;
                }
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
                CmdOnHorseChange(i);
        }
    }
    [Command]
    public void CmdOnHorseChange(int i)
    {
        HasChooseHorse[i] = true;              //在Server端改ChooseRole的，Server端的TogetherChooseRole的也會改，達到Server端同步
        TogetherChooseRole.RpcOnHorseChange(i);//在Client端改TogetherChooseRole的，Client端的ChooseRole的也會改，達到Client端同步
        MyRole = 10 + i;
    }
    public void OnShooterChange(bool sanyaaa)
    {
        if (MyRole != 0)
            ClearChooseRole();
        for (int i = 0; i <= 3; i++)
        {
            if (Shooter[i].isOn)
                CmdOnShooterChange(i);
        }      
    }
    [Command]
    public void CmdOnShooterChange(int i)
    {
        HasChooseShooter[i] = true;               //在Server端改ChooseRole的，Server端的TogetherChooseRole的也會改，達到Server端同步
        TogetherChooseRole.RpcOnShooterChange(i);//在Client端改TogetherChooseRole的，Client端的ChooseRole的也會改，達到Client端同步
        MyRole = 20 + i;
    }
    private void ClearChooseRole()
    {
        int MyRoleNum = MyRole;
        MyRole = 0;
        switch (MyRoleNum / 10)
        {
            case 1:               
                Horse[MyRoleNum - 10].isOn = false;
                CmdClearChooseRole(MyRoleNum);
                break;
            case 2:               
                Shooter[MyRoleNum - 20].isOn = false;
                CmdClearChooseRole(MyRoleNum);
                break;
        }
    }
    [Command]
    public void CmdClearChooseRole(int MyRoleNum)
    {
        MyRole = 0;//Server端的MyRole沒改到
        switch (MyRoleNum / 10)
        {
            case 1:
                HasChooseHorse[MyRoleNum - 10] = false;          //在Server端改ChooseRole的，Server端的TogetherChooseRole的也會改，達到Server端同步
                TogetherChooseRole.RpcClearChooseRole(MyRoleNum);//在Client端改TogetherChooseRole的，Client端的ChooseRole的也會改，達到Client端同步
                break;
            case 2:
                HasChooseShooter[MyRoleNum - 20] = false;        //在Server端改ChooseRole的，Server端的TogetherChooseRole的也會改，達到Server端同步
                TogetherChooseRole.RpcClearChooseRole(MyRoleNum);//在Client端改TogetherChooseRole的，Client端的ChooseRole的也會改，達到Client端同步
                break;
        }
    }
    //此可改時必為 白轉顏 或 顏轉白 ， 設定MyRole與HasChoose ， 變顏色給Update處理  +  選前清除選過的角色
    //Cmd註記 : Client呼叫Server中自己的Cmd函式，所以Cmd函式終止可放Server端有的東西，不可放Horse[i]

    public void OnPlayButtonClick()
    {
        CmdOnPlayButtonClick();
    }
    [Command]
    public void CmdOnPlayButtonClick()
    {
        TogetherChooseRole.RpcOnPlayButtonClick();
    }
    //有人按開始時，一起到下個場景
    //註記 : 似乎只有玩家物件可以呼叫Cmd與Rpc涵式 ， 但可在任何網路物件上執行 (測試後結論，非查資料)

    [Command]
    public void CmdSpawnRole()
    {
        GameObject HorseRole_Clone = Instantiate(HorseRole);
        GameObject ShooterRole_Clone = Instantiate(ShooterRole);
        
        
        switch (MyRole/10)
        {
            case 1:
                Destroy(ShooterRole_Clone);                                      //不知道原因，但此時用Spawn會產生ShooterRole_Clone在Server端，所以要刪除
                NetworkServer.Spawn(HorseRole_Clone);
                HorseRole_Clone.transform.position = SpawnPoint[MyRole - 10];
                HorseRole_Clone.transform.eulerAngles = SpawnRotate[MyRole - 10];
                ControlRole.SpawnRole(HorseRole_Clone);
                RpcChangeRoleColor(HorseRole_Clone);               
                break;
            case 2:
                Destroy(HorseRole_Clone);                                         //不知道原因，但此時用Spawn會產生HorseRole_Clone在Server端，所以要刪除
                NetworkServer.Spawn(ShooterRole_Clone);
                ShooterRole_Clone.transform.position = SpawnPoint[MyRole - 20];
                ShooterRole_Clone.transform.eulerAngles = SpawnRotate[MyRole - 20];
                ControlRole.SpawnRole(ShooterRole_Clone);
                RpcChangeRoleColor(ShooterRole_Clone);
                break;
        }

        ServerNetworkManager.OnGameStart();       
    }
    [ClientRpc]
    public void RpcChangeRoleColor(GameObject Role_Clone)
    {      
        switch (MyRole/ 10)
        {
            case 1:
                Role_Clone.GetComponent<Renderer>().material.color = Colors[MyRole - 10];                
               break;
            case 2:
                Role_Clone.GetComponent<Renderer>().material.color = Colors[MyRole- 20];
                break;
        }
        ControlRole.SpawnRole(Role_Clone);
    }
    //召喚MyRole (Transform會有NetworkTransform同步，所以Transform只要且必要在Server上運算 ; Color只要在Client顯示，所以只用ClientRpc就好) + 回傳給Server狀態
    //註記 : NetworkServer.Spawn()不可以用於Prefer物件 ， 所以才需要Instantiate

    public void OnDestroy()
    {
        if (isServer)
        {
            switch (MyRole / 10)
            {
                case 1:
                    HasChooseHorse[MyRole - 10] = false;
                    TogetherChooseRole.RpcClearChooseRole(MyRole);
                    break;
                case 2:
                    HasChooseShooter[MyRole - 20] = false;
                    TogetherChooseRole.RpcClearChooseRole(MyRole);
                    break;
            }
        }
    }
    //中途登出時清除HasChoose
    //註記 : Server端好像不能呼叫Cmd函式 (測試後結論，非查資料) ， 所以只好複製貼上CmdClearChooseRole
}
