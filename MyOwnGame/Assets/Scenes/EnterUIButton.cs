using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterUIButton : MonoBehaviour
{
    public GameObject OfflineControlRoleStuff;

    public void OnJoinButtonClick()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void OnAlongButtonClick()
    {
        SceneManager.LoadSceneAsync(2);
        GameObject OfflineControlRoleStuff_Clone = Instantiate(OfflineControlRoleStuff);
        DontDestroyOnLoad(OfflineControlRoleStuff_Clone);//可能在LoadScene前就Spawn了，所以要DontDestoryOnLoad (之後用Destory毀他)
    }
}
