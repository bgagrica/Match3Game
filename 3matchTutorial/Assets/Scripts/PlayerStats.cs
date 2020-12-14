using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats 
{
    public static int PUHammer = 4;
    public static int PURowCross =5;
    public static int PUMagic =6;
    public static int lives;
    public static int gold = 10000,silver;



    public static void decrementPUHammer()
    {
        --PUHammer;
        Debug.Log(PUHammer);
    }
    public static void decrementPUMagic()
    {
        --PUMagic;
    }
    public static void decrementPURowCross()
    {
        --PURowCross;
    }
    public static void decrementLives()
    {
        --lives;
    }
    public static void decrementGold(int goldMinus)
    {
        if(gold >= goldMinus)
        gold -= goldMinus;
    }

    public static void icncrementPUHammer()
    {
        ++PUHammer;
    }
    public static void icncrementPUMagic()
    {
        ++PUMagic;
    }
    public static void icncrementPURowCross()
    {
        ++PURowCross;
    }
    public static void icncrementLives()
    {
        ++lives;
    }


}
