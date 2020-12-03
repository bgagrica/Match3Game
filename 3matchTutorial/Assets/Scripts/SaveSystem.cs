using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public static class SaveSystem 
{
    public static int serialNumber;
    public static readonly string saveFolder = Application.persistentDataPath+"/saves";
     
   
    


    public static void runIt()
    {
        Debug.Log(Application.persistentDataPath);
         
        // Test if Save Folder exists
        if(!Directory.Exists(saveFolder))
        {
            // Create Save Folder
            Directory.CreateDirectory(saveFolder);
            saveLevels();
            
        }
            
       
        
    }
    
    
    public static void Save(LevelDataClass level)
    {
        
        string levelName = "/Level" + level.serialNumber+".txt";
        string jsonStr = JsonUtility.ToJson(level);
        //Resources.UnloadAsset(jsonStr);
        // Debug.Log(jsonStr);
        File.WriteAllText(saveFolder + levelName, jsonStr);
        
    }

 
    
    public static LevelDataClass Load()
    {   
      
        string saveString;
        string levelName = "/Level" + serialNumber.ToString()+".txt";
       
        saveString = File.ReadAllText(saveFolder + levelName); 
        LevelDataClass level = new LevelDataClass();
        level = JsonUtility.FromJson<LevelDataClass>(saveString);
       
        
        return level;

    
    }
    public static LevelDataClass LoadLevelByNumber(int numb)
    {   
        string saveString;
        
        string levelName = "/Level" + numb.ToString()+".txt";
        
       saveString = File.ReadAllText(saveFolder + levelName); 
        LevelDataClass level = new LevelDataClass();
        level = JsonUtility.FromJson<LevelDataClass>(saveString);
       
        
        return level;

    
    }

    public static void saveLevels()
    {
        for(int i = 1 ; i <= 5 ; i++)
        {
            string levelName = "Level"+i.ToString();
            string saveString;
            TextAsset ta = Resources.Load<TextAsset>(levelName);
            saveString = ta.ToString();
            Debug.Log(saveString);
            if(!Directory.Exists(saveFolder +"/"+ levelName +".txt"))
            File.WriteAllText(saveFolder +"/"+ levelName +".txt", saveString);
            
        }
        
    
    }


}
