using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OfflineUISetting : MonoBehaviour
{
    void Update()
    {
        Scene NowScene = SceneManager.GetActiveScene();

        if (NowScene.name == "INRoomUI")
        {
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener(ExitRoom);
        }    
        ////////////////InRoomUI程式///////////////

        if (NowScene.name == "Play Scene")
        {
            GameObject.Find("BackButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(FinishGame);
        }
        ////////////////PlayScene程式/////////////////
    }

    private void ExitRoom()
    {
        Destroy(this.gameObject);
        SceneManager.LoadSceneAsync(0);
    }

    private void FinishGame()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene(0);
    }
}
