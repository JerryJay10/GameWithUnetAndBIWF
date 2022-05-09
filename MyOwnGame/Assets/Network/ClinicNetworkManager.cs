using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ClinicNetworkManager : NetworkManager
{
    public InputField InputFieldConnectPort;
    public InputField InputFieldConnectIP;
    public Button RoomButton;
    public Button BackButton;
    public Button DisConnectButton;  
    public Text ConnectingText;
    public Text ConnectFailText;
    private int ConnectPort;
    private string ConnectIP;
    private bool HasSetInRoomUI = false;
    private bool HasSetPlaySceneUI = false;

    public void InputingPort(string mdamdamda)
    {    
        ConnectPort = int.Parse(InputFieldConnectPort.text);
    }
    //輸入Port
    public void InputingIP(string olaolaola)
    {      
        ConnectIP = InputFieldConnectIP.text.ToString();
    }
    //輸入IP

    public void RoomButtonClick()
    {
        networkPort = ConnectPort;
        networkAddress = ConnectIP;
        singleton.StartClient();
    }
    public override void OnClientConnect(NetworkConnection conn)
    {      
        ClientScene.Ready(conn);
        ClientScene.AddPlayer(0);
        SceneManager.LoadSceneAsync(2);
    }
    //加入伺服器 + 到房間內 

   public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopCoroutine(ConnectFail());
        SceneManager.LoadSceneAsync(1);
    }
    IEnumerator ConnectFail()
    {
        ConnectFailText.color = Color.red;
        yield return new WaitForSeconds(1.5f);
        ConnectFailText.color = Color.gray;
    }
    //連線失敗回傳

    public void OnBackButtonClick()
    {
        SceneManager.LoadSceneAsync(0);
    }
    //返回標題紐

   public void OnDisConnectButtonClick()
    {
        singleton.StopClient();
    }
    //斷線紐

    ////////////////RoomUI程式/////////////////

    private void ExitRoom()
    {
        singleton.StopClient();
        SceneManager.LoadSceneAsync(1);
    }
    //設定InRoom返回紐(結束客戶端)

    ////////////////InRoomUI程式///////////////
    
    private void FinishGame()
    {
        singleton.StopClient();
        Destroy(this.gameObject);
        SceneManager.LoadScene(0);
    }


    ///////////////////////////////雜事////////////////////////////////////



    private void Update()
    {
        Scene NowScene = SceneManager.GetActiveScene();

        if (NowScene.name == "RoomUI")
        {
            if (ConnectingText == null)
                SetUI();
            //設定Room按鈕(OnlevelWasLoad有問題，此為替代方案)

            if (client != null)
            {
                ConnectingText.gameObject.transform.localPosition = new Vector2(225f, 100f);
                DisConnectButton.transform.localPosition = new Vector2(160f, -20f);

                RoomButton.interactable = false;
                BackButton.interactable = false;
                InputFieldConnectIP.interactable = false;
                InputFieldConnectPort.interactable = false;
            }
            else
            {
                ConnectingText.gameObject.transform.localPosition = new Vector2(2250f, 100f);
                DisConnectButton.transform.localPosition = new Vector2(1600f, -20f);

                RoomButton.interactable = true;
                BackButton.interactable = true;
                InputFieldConnectIP.interactable = true;
                InputFieldConnectPort.interactable = true;
            }
        }
        //偵測有無在連線中 + 跟改按鈕

        if (IsClientConnected() && !ClientScene.ready)
        {
            ClientScene.Ready(client.connection);

            if (ClientScene.localPlayers.Count == 0)
                ClientScene.AddPlayer(0);
        }
        //確保客戶端能確實接收訊息(Ready)

        ////////////////RoomUI程式/////////////////
        
        if(NowScene.name == "INRoomUI")
        {
            if (!HasSetInRoomUI)
            {
                SetUI();
                HasSetInRoomUI = true;
            }
        }

        ////////////////InRoomUI程式///////////////

        if (NowScene.name == "Play Scene")
        {
            if (!HasSetPlaySceneUI)
            {
                SetUI();
                HasSetPlaySceneUI = true;
            }
        }
    }


    /////////////////////////////////每偵進行/////////////////////////////////



    private void SetUI()
    {
        Scene NowScene = SceneManager.GetActiveScene();

        if (NowScene.name == "RoomUI")
        {
            GameObject.Find("RoomButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("RoomButton").GetComponent<Button>().onClick.AddListener(RoomButtonClick);

            GameObject.Find("BackButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(OnBackButtonClick);

            GameObject.Find("DisConnectButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("DisConnectButton").GetComponent<Button>().onClick.AddListener(OnDisConnectButtonClick);

            GameObject.Find("InputField_IP").GetComponent<InputField>().onEndEdit.RemoveAllListeners();
            GameObject.Find("InputField_IP").GetComponent<InputField>().onEndEdit.AddListener(InputingIP);

            GameObject.Find("InputField_Port").GetComponent<InputField>().onEndEdit.RemoveAllListeners();
            GameObject.Find("InputField_Port").GetComponent<InputField>().onEndEdit.AddListener(InputingPort);

            ConnectingText = GameObject.Find("ConnectingText").GetComponent<Text>();
            ConnectFailText = GameObject.Find("ConnectFailText").GetComponent<Text>();
            
            RoomButton = GameObject.Find("RoomButton").GetComponent<Button>();
            DisConnectButton = GameObject.Find("DisConnectButton").GetComponent<Button>();
            BackButton = GameObject.Find("BackButton").GetComponent<Button>();

            InputFieldConnectPort = GameObject.Find("InputField_Port").GetComponent<InputField>();
            InputFieldConnectIP = GameObject.Find("InputField_IP").GetComponent<InputField>();
        }
        ////////////////RoomUI程式/////////////////

        if (NowScene.name == "INRoomUI")
        {
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener(ExitRoom);  
        }
        ////////////////InRoomUI程式///////////////
        
        if(NowScene.name == "Play Scene")
        {
            GameObject.Find("BackButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(FinishGame);
        }
        ////////////////PlayScene程式/////////////////
    }


    /////////////////////////////////UI設定/////////////////////////////////
}
