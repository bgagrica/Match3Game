using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Start()
    {
        SaveSystem.runIt();
     // Debug.Log(Application.dataPath);
    // Debug.Log(Application.persistentDataPath);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void loadLevel(int val)
    {
        SaveSystem.serialNumber  = val;
         SceneManager.LoadScene(1);
        
    }

    public void loadMenu()
    {
        
         SceneManager.LoadScene(0);
        
    }
    public void loadShopScene()
    {
        SceneManager.LoadScene(3);
    }

     public void exitGame()
     {
        Debug.Log("Quit");
       Application.Quit();
     }
     


}
