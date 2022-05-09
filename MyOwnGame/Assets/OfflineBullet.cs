using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OfflineBullet : MonoBehaviour
{
    public GameObject LittleCrack;
    public GameObject Explosion;
    public GameObject AttackHotExplose;
    public GameObject BulletDestory;

    public float BulletSpeed;
    private float StartTime;
    private float FlyTime;

    public bool Buncing = false;//子彈撞牆過，反彈中
    public bool isOnCollider = false;
    public bool HasStop = false;//有無停
    private bool HasFixed = false;//如果第一次碰撞就是碰邊邊，就不用再進行碰邊邊修正
    private bool HasFirstEnterCollider = false;

    private Vector3 LastForce;
    private Vector3 ReflectVector;
    private Vector3 SimlarContactPoint;
    private Vector3 StartPos;

    private void OnCollisionEnter(Collision collision)
    {
        if (!HasFirstEnterCollider)
        {
            HasFirstEnterCollider = true;
            //只執行一次ColliderEnetr(可能一次碰到多個Collider)

            switch (collision.gameObject.tag)
            {
                case "BattleField":
                    if (!Buncing)
                    {
                        Vector3 thisForward = this.transform.forward;//導好thisForward才不用一直寫

                        SimlarContactPoint = collision.GetContact(0).point;//get第一接觸點(注意!不知為何，第一接觸點不會是預期的!第一!接觸點，但仍在第一接觸面)
                        Vector3 StartToContact = SimlarContactPoint - StartPos;
                        float StartToContactDistance = StartToContact.magnitude;
                        float StartAngle = Vector3.Angle(thisForward, StartToContact);
                        float StartFixDistance = StartToContactDistance * Mathf.Cos(StartAngle * Mathf.Deg2Rad);
                        this.transform.position = StartPos + thisForward * StartFixDistance;
                        HasFixed = true;//此時就完成碰邊邊修正了，順便紀錄為第一次修正
                        //OnColliderEnter時已經處理碰撞，不會是離碰撞點最近點，故做此修正(前兩幀: 未碰撞->前一幀:處理碰撞(Trigger沒有) + 確認Continuse(當兩幀到一幀間有東西，觸發碰撞)->此幀:OnTrigger/ColliderEnter)，此為實驗結果，無查資料(Continous原理有查到))
                        //由於第一接觸點問題，此位置可能在牆內，需要吃子彈修正(因為吃子彈修正時子彈要在thForward上，此不可省)

                        this.GetComponent<Collider>().isTrigger = true;//才不會被擠出去 + 不被Raycast打到(記得把queries hit triggers關掉，不然raycast會打到子彈)

                        Vector3 BeforePosition = this.transform.position - thisForward * BulletSpeed * Time.deltaTime;//因第一接觸點問題，只好找一幀前位置(不確定下陷多少，只好找此，最近確定無牆內位置)
                        Vector3 BulletToContact = SimlarContactPoint - BeforePosition;//get Reflector用
                        RaycastHit hit;
                        Physics.Raycast(BeforePosition, BulletToContact, out hit);
                        ReflectVector = hit.normal;//不用collision.GetContact(0).normal是因為這是get"把子彈推出去的方向"，和get表面的normal不一樣
                                                   //get ReflectVector

                        BulletInWallFixed(collision.collider);//在此時修正Raycast才打不到子彈
                    }
                    break;
            }
        }
    }


    private void OnTriggerEnter(Collider other)//不用isKinematic + OnColliderEnter來做無碰撞偵測原因 : 1.不用collision.GetContact(0).normal了 2.continuous speculative仍有可能tunneling(雖然旋轉時不會，但快速移動會，不符合目的)，isKinematic只能用speculative
    {
        if (!HasFirstEnterCollider)
        {
            HasFirstEnterCollider = true;
            //只執行一次ColliderEnetr(可能一次碰到多個Collider)


            switch (other.tag)
            {
                case "BattleField":
                    if (!HasStop)
                        BulletInWallFixed(other);

                    break;
                case "AttackStuff":
                    if (!Buncing)
                        Destroy(this.gameObject);
                    else
                    {
                        //LastForce = Vector3.Reflect(other.transform.forward, ReflectVector) * 1000f;
                        //this.GetComponent<Rigidbody>().AddForce(LastForce);
                        this.GetComponent<Collider>().isTrigger = false;
                        //子彈被打到從牆壁反彈模擬 
                    }

                    break;

            }
        }
        //主運算程式

        if (other.tag != "BattleField")//吃子彈特效會播，避免
            UseParticle(other);//粒子特效
    }


    private void OnTriggerStay(Collider other)
    {
        if (!HasStop)
            BulletInWallFixed(other);
    }


    private void OnCollisionExit(Collision collision)
    {
        HasFirstEnterCollider = false;
    }
    //離開Collision回復HasFirstEnetrCollider



    private void Start()
    {
        StartTime = Time.time;
        StartPos = this.transform.position;
    }
    private void Update()
    {
        FlyTime = Time.time - StartTime;
        if (FlyTime > 5f)
        {
            if (HasStop && !Buncing)
                Destroy(this.gameObject);
        }
        if (Time.time - StartTime > 20f)
            Destroy(this.gameObject);
        //計算FlyigTime，預防射向天空，要一直計算子彈，不會被消滅
    }
    //Continuse Dynmic有問題在每秒處理(Fixed Timestep)物理50次時有問題(碰邊邊有時不會Collision)，調成100次就沒有了(網路偏方)，但是每秒10次時也不會有問題(超卡就是了)，不知道為什麼(查沒有)，原理和執行本來就多bug，應該是原代碼有錯吧



    private void UseParticle(Collider other)
    {
        switch (other.tag)
        {
            case "BattleField":
                if (HasStop)
                {
                    Destroy(this.GetComponentsInChildren<Transform>()[1].gameObject);//取消拖曳線特效
                    GameObject NewLittleCrack = Instantiate(LittleCrack, this.transform.position, other.transform.rotation);
                    NewLittleCrack.GetComponent<Renderer>().material.color = other.GetComponent<Renderer>().material.color;
                    Destroy(NewLittleCrack, 0.8f);
                }
                break;
            case "Player":
                GameObject NewExplosion = Instantiate(Explosion, this.transform.position, Quaternion.identity);
                NewExplosion.GetComponentsInChildren<Transform>()[1].GetComponent<Renderer>().material.color = other.GetComponent<Renderer>().material.color;
                Destroy(NewExplosion, 0.8f);
                break;
            case "AttackStuff":
                GameObject NewAttackHitExplose = Instantiate(AttackHotExplose, (this.transform.position + other.transform.position) / 2, Quaternion.identity);
                Destroy(NewAttackHitExplose, 0.8f);
                break;
        }
    }
    //粒子特效

    private void BulletInWallFixed(Collider other)
    {
        Vector3 thisForward = this.transform.forward;//導好thisForward才不用一直寫
        Vector3 thisPosition = this.transform.position;//導好thisPosition才不用一直寫
        float FrameDistance = BulletSpeed * Time.deltaTime;//子彈一幀前進距離

        Vector3 BeforeBulletPosition = thisPosition - thisForward * FrameDistance;//找一幀前的位置，雖然(前兩幀: 未碰撞->前一幀:處理碰撞(Trigger沒有) + 確認Continuse(當兩幀到一幀間有東西，觸發碰撞)->此幀:OnTriggerEnter)，但第一次修正應不會陷入太多(SimluarContactPoint不會偏離太多)，且指後是每幀偵測，所以可以用一幀前距沒問題

        bool CanStop = true;
        RaycastHit hit;
        if (Physics.Raycast(BeforeBulletPosition, thisForward, out hit, FrameDistance + 0.25f))//一幀前位置往前(1個FrameDistance)+子彈半徑有東西
            this.transform.position = hit.point;//這就是吃子彈修正(前一幀射出的Raycast打到的點)
        else//沒有陷進去
        {
            if (Physics.Raycast(thisPosition, -ReflectVector, out hit) && (Mathf.Round(Vector3.Angle(ReflectVector, hit.point - SimlarContactPoint) * 10) / 10 == 90f))//以此時位置為始向下找地板，地板與other是同一平面，就不會是碰邊邊，會是沒陷太深，再執行一次(||後要四捨五入不然會有微小偏差)
            {
                HasFirstEnterCollider = false;//讓程式可以再跑一次
                CanStop = false;
            }
            else//碰邊邊修正
            {
                if (!HasFixed)
                    BulletHitEdgeFixed(other);
            }
        }


        if (CanStop)//可能沒陷進去不執行
        {
            if (HasFixed)
                HasFixed = false;

            HasStop = true;

            this.GetComponent<Rigidbody>().Sleep();//實驗得知，當這一幀受到力會在下一幀一起執行，所以會有同時打中兩物，一個CanStop一個沒有，結果沒停下，所以要Sleep()讓此幀不受力
            this.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);//確認修正完成後才停止子彈    
            UseParticle(other);//不確定何時做完修正，所以做完就播
        }
        else
        {
            if (HasFixed)//第一次不修正，會反彈(會先計算碰撞才OnColliderEnter)，所以要改
            {
                this.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                this.GetComponent<Rigidbody>().AddForce(thisForward * BulletSpeed, ForceMode.Impulse);
                HasFixed = false;
            }
        }
    }
    //吃子彈修正

    private void BulletHitEdgeFixed(Collider other)//無法得知碰撞點的碰邊邊修正(SimlarContactPoint只是知道在碰撞面)，目前想不到通用算法，僅在Box和SphereCollider上準確，其他物體目前用BoxCollider的算法，希望不會差太多
    {
        Vector3 thisForward = this.transform.forward;//導好thisForward才不用一直寫
        Vector3 thisPosition = this.transform.position;//導好thisPosition才不用一直寫

        float FixedForwardDistance;
        if (other.TryGetComponent(out SphereCollider sphere))
        {
            Vector3 CenterToBullet = thisPosition - other.transform.position;
            float CenterDistance = CenterToBullet.magnitude;
            float CenterAngle = Vector3.Angle(thisForward, CenterToBullet);
            FixedForwardDistance = CenterDistance * Mathf.Cos(Mathf.Deg2Rad * CenterAngle);
        }
        else
        {
            Vector3 BulletToSimlarPoint = SimlarContactPoint - thisPosition;//主要計算用
            float SimlarAngle = Vector3.Angle(BulletToSimlarPoint, ReflectVector);
            float SimlarDistance = BulletToSimlarPoint.magnitude;
            float VertialDistance = SimlarDistance * Mathf.Cos(Mathf.Deg2Rad * SimlarAngle);//下陷垂直距離(Deg2Rad是/180 * pi的結果)
            float RealAngle = Vector3.Angle(ReflectVector, -thisForward);//髮線與子彈forward的夾角
            float RealForwardDistance = VertialDistance / Mathf.Cos(Mathf.Deg2Rad * RealAngle);//真正下陷距離

            Vector3 SimlarBullet = thisPosition - thisForward * RealForwardDistance;//找到平行尖端的點(與SimlarContactPoint同一面上)，確認等一下ClosestPoint必找到尖端

            SimlarContactPoint = other.ClosestPoint(SimlarBullet);//以SimlarBullet找最近點(原本為前兩幀)，確認找到尖端
            Vector3 CloestPointToBullet = thisPosition - SimlarContactPoint;//最近點到子彈向量              
            float CloestPointDistance = CloestPointToBullet.magnitude;
            float FixingAndle = Vector3.Angle(CloestPointToBullet, thisForward);//最近點到子彈向量與Forward的夾角
            FixedForwardDistance = CloestPointDistance * Mathf.Cos(Mathf.Deg2Rad * FixingAndle);//返回距離
        }


        this.transform.position = thisPosition - thisForward * FixedForwardDistance;//返回到thisForward離最近點最近的位置，返回完成
    }

    private void OnDestroy()
    {
        if (HasStop || !HasFirstEnterCollider)//在第一次碰撞時不撥放
        {
            GameObject NewBulletDestory = Instantiate(BulletDestory, this.transform.position, this.transform.rotation);
            NewBulletDestory.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
            Destroy(NewBulletDestory, 0.8f);
        }
    }
}