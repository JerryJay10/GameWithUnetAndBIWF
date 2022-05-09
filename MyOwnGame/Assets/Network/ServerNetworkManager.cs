using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;

public class ServerNetworkManager : NetworkManager
{
    public InputField InputFieldserverPort;
    public Text ServerIPText;
    public Text ServerPeopleText;
    public Text PortErrorText;
    public Text ServerOnText;
    public Text ServerOffText;
    public Text PlaySituation_NotStartText;
    public Text PlaySituation_InRoomText;
    public Text PlaySituation_GamingText;
    public Text PlaySituation_EndText;
    public Button GoServerButton;
    public Button StopServerButton;
    public Button BackButton;
    public GameObject ConnectTogetherStuff;
    private int serverPort;
    private int serverPeopleNumber;

    void Start()
    {
    
    }
   
    public void InputingPort()
    {
        serverPort = int.Parse(InputFieldserverPort.text);
    }
    //自訂Port

    public void StartServerButton()
    {
        if(serverPort < 1025 || serverPort > 65535)
        {
           StartCoroutine(PortError());
           return;
        }
        networkPort = serverPort;
        singleton.StartServer();
    }
    //開始伺服器

    IEnumerator PortError()
    {
        PortErrorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PortErrorText.gameObject.SetActive(false);
    }
    //顯示Port輸入錯誤Text

    public override void OnStartServer()
    {
        IPHostEntry host;   //提供網路位置資訊的class(需DNS給資料)
        host = Dns.GetHostEntry(Dns.GetHostName());   //DNS將改變後的IP(ex:oooo.com.tw)改回原IP

        foreach(IPAddress ip in host.AddressList)   //在原IP(陣列)中每項呼叫
        {
            networkAddress = ip.ToString();
        }
        //顯示伺服器私有IP

        PlaySituation_NotStartText.gameObject.SetActive(false);
        PlaySituation_InRoomText.gameObject.SetActive(true);
        //遊戲狀況回報
    }

    public void EndServerButton()
    {
        singleton.StopServer();
        serverPeopleNumber = 0;
    }
    //結束伺服器

    public override void OnStopServer()
    {
        PlaySituation_EndText.gameObject.SetActive(false);
        PlaySituation_GamingText.gameObject.SetActive(false);
        PlaySituation_InRoomText.gameObject.SetActive(false);
        PlaySituation_NotStartText.gameObject.SetActive(true);
    }
    //遊戲狀況回報

    public override void OnServerConnect(NetworkConnection conn)
    {
        serverPeopleNumber++;
        ServerPeopleText.text = serverPeopleNumber.ToString();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        serverPeopleNumber--;
        ServerPeopleText.text = serverPeopleNumber.ToString();

        NetworkServer.DestroyPlayersForConnection(conn);
    }
    //偵測伺服器人數 + 刪除客戶端連線東西

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (GameObject.FindGameObjectWithTag("ConnectTogetherStuff") == null)
        {
            GameObject ConnectTogetherStuff_Clone = Instantiate(ConnectTogetherStuff);
            NetworkServer.Spawn(ConnectTogetherStuff_Clone);
        }
        else
        {
            TogetherChooseRole TogetherChooseRole = GameObject.FindGameObjectWithTag("ConnectTogetherStuff").GetComponent<TogetherChooseRole>();
            for (int i = 0; i <= 3; i++)
            {
                if (TogetherChooseRole.HasChooseHorse[i])
                    TogetherChooseRole.RpcOnHorseChange(i);
                if (TogetherChooseRole.HasChooseShooter[i])
                    TogetherChooseRole.RpcOnShooterChange(i);
            }
        }

        GameObject ClientConnectStuff = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, ClientConnectStuff, playerControllerId);
    }
    //(召喚TogetherChooseRole || 在開始時同步其他人的HasChoose) + 客戶端連線東西

    public void OnGameStart()
    {
        PlaySituation_InRoomText.gameObject.SetActive(false);
        PlaySituation_GamingText.gameObject.SetActive(true);
    }
    //在Game開始時接收Client端狀況

    private void Update()
    {
        if (NetworkServer.active)
        {
            ServerOnText.gameObject.SetActive(true);
            ServerOffText.gameObject.SetActive(false);

            GoServerButton.interactable = false;
            BackButton.interactable = false;
            StopServerButton.interactable = true;

            InputFieldserverPort.interactable = false;
        }
        else
        {
            ServerOnText.gameObject.SetActive(false);
            ServerOffText.gameObject.SetActive(true);

            GoServerButton.interactable = true;
            BackButton.interactable = true;
            StopServerButton.interactable = false;

            InputFieldserverPort.interactable = true;
        }
        //偵測伺服器開關

        if (NetworkServer.active)
            ServerIPText.text = networkAddress;
        else
            ServerIPText.text = "未啟動";
        //顯示伺服器IP
    }
}
