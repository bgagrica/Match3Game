using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool unlocked;
    public int number;
    public Image unlockImage;
    public GameObject[] emptyStars;
    public GameObject[] fullStars;
    private LevelDataClass levelData;
    private LevelDataClass lastLevelData;

    void Awake()
    {
     loadLevel();
    }

    void Update()
    {
      
        UpdateLevelImage();
        
    }

    private void UpdateLevelImage()
    {
        if(!unlocked)// if it isnt unlocked
        {
            unlockImage.gameObject.SetActive(true);
            for(int i = 0; i<emptyStars.Length;i++)
                emptyStars[i].gameObject.SetActive(false);
            for(int i = 0; i<fullStars.Length;i++)
                fullStars[i].gameObject.SetActive(false);
        }
        else // if it is unlocked
        {
            unlockImage.gameObject.SetActive(false);
            for(int i = 0; i<emptyStars.Length;i++)
                emptyStars[i].gameObject.SetActive(true);
             for(int i = 0; i<fullStars.Length;i++)
                fullStars[i].gameObject.SetActive(false);
             UpdateFullStars();
        
        }
    }

    private void loadLevel()
    {
       if(number > 1)
       lastLevelData = SaveSystem.LoadLevelByNumber(number - 1);
       levelData = SaveSystem.LoadLevelByNumber(number);

        if(number == 1 || lastLevelData.score >= lastLevelData.oneStar)
        {
            unlocked = true;
        }




    }

    private void UpdateFullStars()
    {
        int value = levelData.score;
        int firstStar = levelData.oneStar;
        int secondStar = levelData.twoStar;
        int thirdStar = levelData.threeStar;

        if(!unlocked)return;
        if(value >= thirdStar)
            for(int i = 0; i<fullStars.Length;i++)
            {
                fullStars[i].gameObject.SetActive(true);
                
            }
                
        if(value >= secondStar)
        {
            for(int i = 0; i<fullStars.Length - 1 ;i++)
                fullStars[i].gameObject.SetActive(true);
           
        }
        if(value >= firstStar)
                fullStars[0].gameObject.SetActive(true);
       
       
    
    }

   public void clickOnButtonLoad()
    {
        if(unlocked)
        {
        SaveSystem.serialNumber  = number;
        SceneManager.LoadScene(1);
        }
    }

}
