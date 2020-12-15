using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused;
    public GameObject pauseMenuUI;
    public GameObject loseMenuUI;
    public GameObject wonMenuUI;
    public Text movesText ;
    public Text scoreText ;
    public Text finalScoreText;
    public Text targetPieceCountText ;
    public static int targetPieceCount ;
    public static int winCondInt;
    public static int moves;
    public static bool finished;
    public bool goldPLus = false;
    //  public  Match3 game;
 
 


    public static bool winCond;
    public bool lostCond;

    

    void Start()
    {
        
        //game = GetComponent<Match3>();
        finished = false;
        winCond = false;
        lostCond = false;
        gameIsPaused = false;
        resume();
        
    }
   
    // Update is called once per frame
    void Update()
    {
       finalScoreText.text =  scoreText.text;
       checkWinLoose();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
                resume();
            else
                pause(); 
        
        }


    }
   
    void checkWinLoose()
    {
        checkCondtitions();
        if(winCond)youWon();
        if(lostCond)youLose();
    }

    public void checkCondtitions()
    {
        
             if(targetPieceCount  >=  winCondInt)
                winCond = true;
            else
                if(Int32.Parse(movesText.text) <=  0)
                   lostCond = true;   
             

    }

    public void loadNextLevel()
    {
        goldPLus = false;
        if (SaveSystem.serialNumber < 5)
        {
        SaveSystem.serialNumber = SaveSystem.serialNumber + 1;
        SceneManager.LoadScene(1);
        }
        else
            SceneManager.LoadScene(2);
        
    }

    public  void resume()
    {
        
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;


    }

    public  void pause()
    {
        
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void menu()
    {
        goldPLus = false;
        SceneManager.LoadScene(0);
    }

    public void tryAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void youLose()
    {
        finished = true;
        loseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void youWon()
    {
        finished = true;
        wonMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        if (!goldPLus)
        {
            PlayerStats.incrementGold();
            goldPLus = true;
        }
        
    }




}
