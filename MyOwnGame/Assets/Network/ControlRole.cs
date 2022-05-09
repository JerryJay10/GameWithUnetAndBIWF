using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ControlRole : NetworkBehaviour
{
    public GameObject MyRole;
    public GameObject Bullet;
    public GameObject DeathExpose;

    public float CameraRotateSpeed;
    public float HorseMoveSpeed;
    public float ShooterMoveSpeed;

    private bool CanStart = false;
    [SyncVar]
    private bool HasStart = false;
    private bool isHorse = false;
    private bool isShooter = false;
    private bool isUseShoot = false;
    private bool HasSetJoyStick = false;

    private int TouchNum = 0 ;
    private int RightTouchNum = 0;
    private int LeftTouchNum = 0;
    private Vector2[] TouchPos = new Vector2[10];
    private Vector2[] TouchStartPos = new Vector2[10];
    private int TurningTouch = -1;
    private int MovingTouch = -1; //-1代表沒有此Touch

    private float StartTimer;
    private float RoleMoveSpeed;

    private ChooseRole ChooseRole;
   
    private GameObject MainCamera;
    private GameObject Canvas;
    private GameObject CountDownStuff;
    private RawImage Joystick;
    private RawImage JoystickBG;
    private Vector2 JScenter;

    private GameObject WeaponStuff;
    private RawImage Aim;
    private RawImage HasChooseWeaponRawImage;
    private Toggle ShootToggle;
    private Color MyColor;

    [SyncVar]
    public int HealthPoint = 5;
    private GameObject HealthBar;
    private GameObject LoseStuff;

    public void SpawnRole(GameObject Role)
    {
        MyRole = Role;

        CanStart = true;

       if(isServer)
            StartTimer = Time.time;
    }
    //在SpawnRole時註冊MyRole + 代替OnStart發動 + 紀錄開始時間

    public void GameStart()
    {
        if (isServer)
        {
            float Timer = Time.time - StartTimer;

            RpcCountDown(Timer);

            if (Timer >= 5.5f)
                HasStart = true;
        }
        //開始的倒數計時器

        ChooseRole = this.GetComponent<ChooseRole>();
        switch (ChooseRole.MyRole / 10)
        {
            case 1:
                isHorse = true;
                MyColor = ChooseRole.Colors[ChooseRole.MyRole - 10];
                RoleMoveSpeed = HorseMoveSpeed;
                break;
            case 2:
                isShooter = true;
                MyColor = ChooseRole.Colors[ChooseRole.MyRole - 20];
                RoleMoveSpeed = ShooterMoveSpeed;
                break;
        }
        //先設好isHorse和isShooter和其Speed值，省流量

        if (!isLocalPlayer)
            return;
        //不讓玩家程式互相干擾
        //註 : 用此Server端也不會執行了，拜託別忘，已經被整兩次了!!

        if (isClient)
        {
            MainCamera = GameObject.Find("MainCamera");
            Canvas = GameObject.Find("Canvas");
            Canvas.GetComponent<CanvasScaler>().scaleFactor = Screen.height / 720f;//Joystick轉換裝置Bug修正(自訂螢幕縮放)
            CountDownStuff = GameObject.Find("CountDown");
            JoystickBG = GameObject.Find("JoystickBG").GetComponent<RawImage>();
            Joystick = JoystickBG.GetComponentsInChildren<Transform>()[1].GetComponent<RawImage>();
            JScenter = JoystickBG.rectTransform.anchoredPosition * (Screen.height / 720f); //設定JoyStick中心 +JoyStick轉換裝置Bug修正(中心隨螢幕中心改變)

            WeaponStuff = GameObject.Find("WeaponCircle");
            Aim = GameObject.Find("Aim").GetComponent<RawImage>();
            Aim.rectTransform.anchoredPosition = new Vector2(1000f, 208f);
            HasChooseWeaponRawImage = WeaponStuff.GetComponentsInChildren<Transform>()[1].GetComponent<RawImage>();
            HasChooseWeaponRawImage.rectTransform.anchoredPosition = new Vector2(1000f, 0f);
            ShootToggle = WeaponStuff.GetComponentsInChildren<Transform>()[2].GetComponent<Toggle>();            
            ShootToggle.onValueChanged.RemoveAllListeners();
            ShootToggle.onValueChanged.AddListener(OnClickWeapon);

            HealthBar = GameObject.Find("Health");
            LoseStuff = GameObject.Find("LoseStuff");
            //各種設定

            Joystick.rectTransform.position = JoystickBG.rectTransform.position;
            //歸正JoyStick

            if (isHorse)
            {
                WeaponStuff.transform.Translate(1000f, 0f, 0f);//不需要WeaponStuff，踢走
            }
            if (isShooter)
            {
                MainCamera.transform.position = new Vector3(MyRole.transform.position.x, MyRole.transform.position.y + 0.5f, MyRole.transform.position.z);
                MainCamera.transform.eulerAngles = new Vector3(0f, MyRole.transform.eulerAngles.y, 0f);
            }             
            //歸正CameraRotation (借用FixedUpdate保險MyRole旋轉時歸正 ， 因為有時候此歸正會比MyRole旋轉先呼叫導致bug)
        }      
    }
    [ClientRpc]
    public void RpcCountDown(float Timer)
    {
        if (Timer >= 2f && Timer < 2.5f)
        {
            RawImage Three = CountDownStuff.GetComponentsInChildren<Transform>()[2].GetComponent<RawImage>();

            Three.rectTransform.anchoredPosition = new Vector2(Three.rectTransform.anchoredPosition.x + (-2000 * Time.fixedDeltaTime), 0f);
        }
        if (Timer >= 3f && Timer < 3.5f)
        {
            RawImage Three = CountDownStuff.GetComponentsInChildren<Transform>()[2].GetComponent<RawImage>();
            RawImage Two = CountDownStuff.GetComponentsInChildren<Transform>()[3].GetComponent<RawImage>();

            Three.rectTransform.anchoredPosition = new Vector2(Three.rectTransform.anchoredPosition.x + (-2000 * Time.fixedDeltaTime), 0f);
            Two.rectTransform.anchoredPosition = new Vector2(Two.rectTransform.anchoredPosition.x + (-2000 * Time.fixedDeltaTime), 0f);
        }
        if (Timer >= 4f && Timer < 4.5f)
        {
            RawImage Two = CountDownStuff.GetComponentsInChildren<Transform>()[3].GetComponent<RawImage>();
            RawImage One = CountDownStuff.GetComponentsInChildren<Transform>()[4].GetComponent<RawImage>();

            Two.rectTransform.anchoredPosition = new Vector2(Two.rectTransform.anchoredPosition.x + (-2000 * Time.fixedDeltaTime), 0f);
            One.rectTransform.anchoredPosition = new Vector2(One.rectTransform.anchoredPosition.x + (-2000 * Time.fixedDeltaTime), 0f);
        }
        if (Timer >= 5f)
        {
            RawImage One = CountDownStuff.GetComponentsInChildren<Transform>()[4].GetComponent<RawImage>();
            RawImage Go = CountDownStuff.GetComponentsInChildren<Transform>()[5].GetComponent<RawImage>();

            One.rectTransform.anchoredPosition = new Vector2(-2000f, 0f);
            Go.rectTransform.anchoredPosition = new Vector2(-1000f, 0f);
            StartCoroutine(ShowStart());
        }
    }
    IEnumerator ShowStart()
    {
        RawImage Go = CountDownStuff.GetComponentsInChildren<Transform>()[5].GetComponent<RawImage>();

        for (float i = 0; i <= 5; i++)
        {
            if (Go.color == Color.black)
                Go.color = Color.white;
            else
                Go.color = Color.black;
            yield return new WaitForSeconds(0.1f);
        }
        CountDownStuff.gameObject.SetActive(false);
    }
    //倒數計時

   
    /////////////////////////開始前作業//////////////////////////////
 

    private void FixedUpdate()
    {
        if (CanStart)
        {
            if(!HasStart)
                GameStart();
        }
        //代替OnStart

        if (!HasStart)     
            return;
        //未開始時不呼叫

        if (!isLocalPlayer)
            return;
        //不讓玩家程式互相干擾

     
        if (TurningTouch != -1)
        {
            HeadTurning();
            TouchStartPos[TurningTouch] = TouchPos[TurningTouch];//有時候TouchPhase.Start會沒有呼叫到，所以用此取代(TurningTouch的)
        }
        //呼叫感測觸控點函式
        if (MovingTouch != -1)
        {
            if (!HasSetJoyStick)
            {
                JoystickBG.rectTransform.anchoredPosition = Input.touches[MovingTouch].position / Screen.height * 720f;//設定JoyjStick位置(在螢幕上(0-720)的哪裡)
                JScenter = JoystickBG.rectTransform.anchoredPosition * (Screen.height / 720f); //設定JoyStick中心(用成目前螢幕(ex:1080)的觸控點，能維持Joystick移動90)
                HasSetJoyStick = true;
            }
            //有時候TouchPhase.Start會沒有呼叫到，所以用此取代(MovingTouch的)

            Moving();
        }
        else
            Joystick.rectTransform.position = JoystickBG.rectTransform.position;
        //呼叫感測觸控點函式
    }

    private void Update()
    {
        if (!HasStart)
            return;
        //未開始時不呼叫

        if (MyRole.GetComponent<BeHit>().Hit == true)
        {
            DecreaseHealth();
            MyRole.GetComponent<BeHit>().Hit = false;
        }
        //Server端的MyRole.BeHit().Hit改變時 (只有Server端會改變)，在Server端呼叫DecreaseHealth

        if (HealthPoint == 0)
        {
            GameObject newDeathExpose = Instantiate(DeathExpose);
            newDeathExpose.GetComponentsInChildren<Transform>()[1].GetComponent<Renderer>().material.color = MyColor;
            newDeathExpose.transform.position = MyRole.transform.position;

            if (isServer)
                NetworkServer.Destroy(MyRole);
        }
        //生命歸零時特效 + 摧毀MyRole

        if (!isLocalPlayer)
            return;
        //不讓玩家程式互相干擾

        RightTouchNum = 0;
        LeftTouchNum = 0;
        //每貞重新計算觸控點數

        int LastTouchNum = TouchNum;

        TouchNum = Input.touchCount;

        if (TouchNum != 0)
        {
            for (int i = 0; i <= (TouchNum - 1); i++)
            {
                TouchPos[i] = Input.touches[i].position;

                if (TouchPos[i].x > Screen.width / 2)
                {
                    if (TurningTouch == -1 && i != MovingTouch)
                        TurningTouch = i;
                    RightTouchNum++;
                }
                if (TouchPos[i].x <= Screen.width / 2)
                {
                    if (MovingTouch == -1 && i != TurningTouch)
                        MovingTouch = i;
                    LeftTouchNum++;
                }
            }
        }
            //設定Touch點位置 + 設定MovingTouch與TurningTouch + 計算RightTouchNum與LeftTouchNum


        if (LastTouchNum - TouchNum >0)
        {
            if (TouchNum == 0)
            {
                Joystick.rectTransform.position = JoystickBG.rectTransform.position;

                if (TurningTouch != -1)
                {
                    if (isUseShoot)
                        ShootStart();

                    TurningTouch = -1;
                    //有用右手(要用ShootStart())
                }

                MovingTouch = -1;
                HasSetJoyStick = false;
            }
            else if (TouchNum == 1)
            {
                if (Input.touches[0].position.x <= Screen.width / 2)
                {
                    if (isUseShoot)
                        ShootStart();

                    TurningTouch = -1;//TruningTouch的
                }
                else
                {
                    Joystick.rectTransform.position = JoystickBG.rectTransform.position;
                    HasSetJoyStick = false;
                    MovingTouch = -1;//MovingTouch的
                }
            }
        }
        //有時候TouchPhase.Ended會沒有呼叫到，所以用此取代(當Touch數變小時呼叫)

      
        CmdHeadTurning(MainCamera.transform.eulerAngles.y, MyRole);
        MyRole.transform.eulerAngles = new Vector3(0f, MainCamera.transform.eulerAngles.y, 0f);
        //以Camera旋轉為主，再同步到MyRole(每貞同步，也就是MyRole.transform.eulerAngles.y = MainCamera.transform.eulerAngles.y)，以避免以MyRole為主時，會受物理影響而抖動，也修正MyRole撞牆會自己旋轉的問題

        if (isHorse)
        {

        }
        if (isShooter)
        {
            MainCamera.transform.position = new Vector3(MyRole.transform.position.x, MyRole.transform.position.y + 0.5f, MyRole.transform.position.z);
            MainCamera.transform.eulerAngles = new Vector3(MainCamera.transform.eulerAngles.x, MainCamera.transform.eulerAngles.y, 0f);//鎖定z座標旋轉
        }
        //每偵同步cameraTransform.position
    }
    //矯正等要常執行者用Update



    /////////////////////每貞呼叫/////////////////////////


    private void HeadTurning()
    {
        if (Input.touches[TurningTouch].phase == TouchPhase.Moved)
        {
            Vector2 lastPos = TouchPos[TurningTouch];
            Vector2 handMove = lastPos - TouchStartPos[TurningTouch];

            if (Vector2.Distance(new Vector2(0f, 0f), handMove) < 0.01f)
                return;
            //防銀幕振動

            MainCamera.transform.Rotate(-handMove.y * CameraRotateSpeed * Time.fixedDeltaTime, handMove.x * CameraRotateSpeed * Time.fixedDeltaTime, 0f);

            if (isHorse)
            {

            }

            TouchStartPos[TurningTouch] = Input.GetTouch(TurningTouch).position;
        }
        //Camera旋轉主程式
    }
    [Command]
    public void CmdHeadTurning(float CameraRotation_Y,GameObject MyRole)
    {
        MyRole.transform.eulerAngles = new Vector3(0f, CameraRotation_Y, 0f);
    }
    //在Server端旋轉，Network Transform會同步至Client端（也會回傳給原本Client端，確認同步)

    private void Moving()
    {
        Vector2 JoystickMove = TouchPos[MovingTouch] - JScenter;
        float moveDistance = Vector2.Distance(new Vector2(0f, 0f), JoystickMove);

        CmdMoving(JoystickMove, MyRole);

        if (moveDistance <= 90f)
        {
            MyRole.transform.Translate(JoystickMove.x * RoleMoveSpeed * Time.fixedDeltaTime, 0f, JoystickMove.y * RoleMoveSpeed * Time.fixedDeltaTime, Space.Self);
            Joystick.rectTransform.anchoredPosition = JoystickMove;
        }          
        else
        {
            JoystickMove.Normalize();
            MyRole.transform.Translate(JoystickMove.x * 90 * RoleMoveSpeed * Time.fixedDeltaTime, 0f, JoystickMove.y * 90 * RoleMoveSpeed * Time.fixedDeltaTime, Space.Self);
            Joystick.rectTransform.anchoredPosition = JoystickMove * 90;
        }
        //移動主程式
    }
    [Command]
    public void CmdMoving(Vector2 JoystickMove,GameObject MyRole)
    {
        float moveDistance = Vector2.Distance(new Vector2(0f, 0f), JoystickMove);

        if(moveDistance <= 40f)
            MyRole.transform.Translate(JoystickMove.x * RoleMoveSpeed * Time.fixedDeltaTime, 0f, JoystickMove.y * RoleMoveSpeed * Time.fixedDeltaTime, Space.Self);
        else
        {
            JoystickMove.Normalize();
            MyRole.transform.Translate(JoystickMove.x * 40 * RoleMoveSpeed * Time.fixedDeltaTime, 0f, JoystickMove.y * 40 * RoleMoveSpeed * Time.fixedDeltaTime, Space.Self);
        }
    }
    //在Server端移動，Network Transform會同步至Client端（也會回傳給原本Client端，確認同步)


    private void OnClickWeapon(bool Falcon__Punchhh)
    {
        if (!isUseShoot)
        {
            Aim.rectTransform.anchoredPosition = new Vector2(-322f, 208f);
            HasChooseWeaponRawImage.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            //UI就位

            isUseShoot = true;
        }
        else
        {
            Aim.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            HasChooseWeaponRawImage.rectTransform.anchoredPosition = new Vector2(1000f, 0f);
            //UI移走

            isUseShoot = false;
        }
    }
    //開關Shoot

    private void ShootStart()
    {
        Vector3 BulletPosition = MainCamera.transform.position + MainCamera.transform.forward * 1.5f;
        Vector3 BulletRotation = MainCamera.transform.eulerAngles;

        CmdShootStart(BulletPosition, BulletRotation);
    }
    [Command]
    private void CmdShootStart(Vector3 BulletPosition, Vector3 BulletRotation)
    {
        GameObject newBullet = Instantiate(Bullet);
        NetworkServer.Spawn(newBullet);

        newBullet.transform.position = BulletPosition;
        newBullet.transform.eulerAngles = BulletRotation;

        RpcShootStart(newBullet, BulletRotation, MyColor);

        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * 2000);
    }
    [ClientRpc]
    private void RpcShootStart(GameObject Bullet,Vector3 BulletRotation,Color BulletColor)
    {
        Bullet.GetComponent<Renderer>().material.color = BulletColor;//material不會同步，需要自己用
        Bullet.transform.eulerAngles = BulletRotation;//同步Rotation要時間，所以幫忙一下
        Bullet.GetComponent<Rigidbody>().AddForce(Bullet.transform.forward * 50f, ForceMode.Impulse);//註 : NetworkTransform有時會不同時，所以Client端也要計算，再用NetworkTrasfom修正
       // Bullet.GetComponent<OfflineBullet>().BulletSpeed = 50;
    }
    //發射子彈


    private void DecreaseHealth()
    {
        if (!isServer)
            return;
        //雖然只有Server端會執行到此函式，仍然保險一下，不然感覺很容易被駭 (雖然我不知報怎麼駭)

        HealthPoint = HealthPoint - 1;
        RpcHealthDecrease();
    }
    [ClientRpc]
    private void RpcHealthDecrease()
    {
        StartCoroutine(HealthDecrease());
    }
    IEnumerator HealthDecrease()
    {
        int NowClaCulHealthPoint = HealthPoint + 1;//過了0.1秒HealthPoint可能會不一樣，要先記錄

        HealthBar.GetComponentsInChildren<Transform>()[NowClaCulHealthPoint].GetComponent<Image>().color = Color.gray;
        yield return new WaitForSeconds(0.1f);
        HealthBar.GetComponentsInChildren<Transform>()[NowClaCulHealthPoint].gameObject.SetActive(false);

        if (HealthPoint == 0)
        {
            LoseStuff.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
        }
    }
    //血條函式 + 呼叫死亡函式
    //註 : 不知道原因，千萬不要把Coroutine用在網路上，會讓整個Unet癱瘓 (遇到bug後修正的結論，非上網查)
}