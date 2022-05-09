using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMancine : MonoBehaviour
{
    public GameObject Bullet;
    private float endTime;
    private bool CanShoot = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        if (CanShoot)
        {
            GameObject newBullet = Instantiate(Bullet);

            newBullet.transform.position = this.transform.position + this.transform.forward * 1.5f;
            newBullet.transform.rotation = this.transform.rotation;

            newBullet.GetComponent<Rigidbody>().AddForce(this.transform.forward * 1500);

            endTime = Time.time;
            CanShoot = false;
        }

        if (!CanShoot)
        {
            float nowTime = Time.time;

            if (nowTime - endTime > 1)
                CanShoot = true;
        }
    }
}
