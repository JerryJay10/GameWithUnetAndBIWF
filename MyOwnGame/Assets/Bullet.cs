using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Bullet : NetworkBehaviour
{
    public GameObject LittleCrack;
    public GameObject Explosion;
    public GameObject AttackHotExplose;

    private float StartTime;
    [SyncVar]
    public bool HasStop = false;
    private bool Buncing = false;

    private GameObject HitWall;
    private Vector3 LastForce;

    void OnTriggerEnter(Collider other)
    {
        this.GetComponentsInChildren<Transform>()[1].GetComponent<ParticleSystem>().Stop();
        this.GetComponentsInChildren<Transform>()[1].GetComponent<ParticleSystem>().Clear();
        //取消拖曳線特效
        //註:我有直接Destory他過，但產生了完全不相干的bug，所以如此

        switch (other.tag)
        {
            case "BattleField":
                GameObject NewLittleCrack = Instantiate(LittleCrack, this.transform.position, other.transform.rotation);
                NewLittleCrack.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
                Destroy(NewLittleCrack, 0.8f);
                break;
            case "Player":
                GameObject NewExplosion = Instantiate(Explosion, transform.position, Quaternion.identity);
                NewExplosion.GetComponentsInChildren<Transform>()[1].GetComponent<Renderer>().material.color = other.GetComponent<Renderer>().material.color;
                Destroy(NewExplosion, 0.6f);
                break;
            case "AttackStuff":
                if (!HasStop)
                {
                    GameObject NewAttackHitExplose = Instantiate(AttackHotExplose, (this.transform.position + other.transform.position) / 2, Quaternion.identity);
                    Destroy(NewAttackHitExplose, 0.8f);
                }
                break;
        }
        //粒子特效


        if (isServer)
        {
            switch (other.tag)
            {
                case "BattleField":
                    if (!HasStop && !Buncing)
                    {
                        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * -2000);//因為Bullet沒受任何力引響，所以此讓Bullet停止

                        HasStop = true;//修正撞牆反彈Bug
                    }
                    //設定HasStop

                    HitWall = other.gameObject;
                    //設定HitWall

                    if (Buncing)
                    {
                        this.GetComponent<Rigidbody>().AddForce(-LastForce);//先停止在施力，比較符合彈射的感覺

                        if (HitWall.name == "Floor")
                            LastForce = Vector3.Reflect(LastForce, HitWall.transform.up);
                        //子彈被打到從地板反彈模擬  
                        else
                            LastForce = Vector3.Reflect(LastForce, HitWall.transform.right);
                        //子彈被打到從牆壁反彈模擬
                        //註:還沒教向量只好用別人的演算法(其實是想不到)，向量一定要好好學才行(我看不懂官方算法)

                        this.GetComponent<Rigidbody>().AddForce(LastForce);
                    }
                    break;
                case "Player":
                    other.GetComponent<BeHit>().HasBeHit();
                    Destroy(this.gameObject);
                    break;
                case "AttackStuff":
                    if (HasStop)
                    {
                        if (HitWall.name == "Floor")
                            LastForce = Vector3.Reflect(other.transform.forward, HitWall.transform.up) * 1500;
                        //子彈被打到從地板反彈模擬  
                        else
                            LastForce = Vector3.Reflect(other.transform.forward, HitWall.transform.right) * 1500;
                        //子彈被打到從牆壁反彈模擬
                        //註:還沒教向量只好用別人的演算法(其實是想不到)，向量一定要好好學才行(我看不懂官方算法)
                        this.GetComponent<Rigidbody>().AddForce(LastForce);

                        HasStop = false;
                        Buncing = true;//開始反彈

                        other.GetComponent<Bullet>().StopBuncing();
                    }
                    else
                    {
                        if (other.GetComponent<Bullet>().HasStop == false)
                            this.GetComponent<Rigidbody>().AddForce(other.transform.forward * 3000);//子彈被空中反彈模擬
                        else
                        {
                            this.GetComponent<Rigidbody>().AddForce(this.transform.forward * -1500);//因為Bullet沒受任何力引響，所以此讓Bullet停止

                            HasStop = true;//修正撞牆反彈Bug
                        }
                    }                    
                    break;
            }
        }
    }

    private void Start()
    {
        StartTime = Time.time;
    }
    private void Update()
    {
        if (Time.time - StartTime > 5f)
        {
            if (HasStop)
                Destroy(this.gameObject);
        }
        if (Time.time - StartTime > 20f)
            Destroy(this.gameObject);
    }
    //預防射向天空，要一直計算子彈，不會被消滅

    public void StopBuncing()
    {
        StartCoroutine(LittleStopTriggering());
    }
    IEnumerator LittleStopTriggering()
    {
        this.GetComponent<Rigidbody>().Sleep();
        yield return new WaitForSeconds(0.01f);
        this.GetComponent<Rigidbody>().WakeUp();
    }
    //在反彈時暫時停止偵測，才不會一起跳(互相偵測到)
}
