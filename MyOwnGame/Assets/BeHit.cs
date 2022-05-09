using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BeHit : MonoBehaviour
{
    public bool Hit = false;

    public void HasBeHit()
    {
        Hit = true;//只有Server端會呼叫此程式，改變Hit給Server端的ControlRole針測用
    }
}
