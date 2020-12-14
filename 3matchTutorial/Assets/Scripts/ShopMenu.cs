using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopMenu: MonoBehaviour
{
    public static int priceHammer = 100;
    public static int priceRowCross = 300;
    public static int priceMagic = 500;
    public static int priceLives = 250;
    public static int priceGold = 100;


    public void buyHammer()
    {
        if (isBuyPossible(priceHammer))
        {
            PlayerStats.icncrementPUHammer();
            PlayerStats.decrementGold(priceHammer);
        }
    }
    public void buyRoWCross()
    {
        if (isBuyPossible(priceRowCross))
        {
            PlayerStats.icncrementPURowCross();
            PlayerStats.decrementGold(priceRowCross);
        }
    }
    public void buyMagic()
    {
        if (isBuyPossible(priceMagic))
        {
            PlayerStats.icncrementPUMagic();
            PlayerStats.decrementGold(priceMagic);
        }

    }
    public void buyLives()
    {
        if (isBuyPossible(priceLives))
        {
            PlayerStats.icncrementLives();
            PlayerStats.decrementGold(priceLives);
        }
    }


    public bool isBuyPossible(int gold)
    {
        if (PlayerStats.gold >= gold)
            return true;
        return false;
    }
    public void menu()
    {
        SceneManager.LoadScene(0);
    }
}
