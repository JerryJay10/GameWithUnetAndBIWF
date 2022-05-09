using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Test : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKey(KeyCode.A))
            CmdTest();
        if (Input.GetKey(KeyCode.W))
            RpcTest();
    }

    [Command]
    void CmdTest()
    {
        this.transform.Translate(3f, 0f, 0f);
        print("test");
    }

    [ClientRpc]
    void RpcTest()
    {
        this.transform.Translate(0f, 3f, 0f);
    }
}
