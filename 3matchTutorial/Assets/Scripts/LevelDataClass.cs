using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;


[System.Serializable]
public class LevelDataClass 
{
   public int goal ; // 1 - score ; 2 - element
   public int criteria ;// 1 - moves ; 2 - time
   public int serialNumber; // number of level
   public  int spriteValue;// number of sprite is deffined in match3 script
    public int elementCount;
  // public bool unlocked{ get; set; } // 
   public int score;// if level is passed this stores the score
   public  int oneStar ;// if score is above  this value
   public   int twoStar  ;  // if score is above this value
   public  int threeStar ; // if score is above this value

    
    public void Initialize(int goal1, int criteria1, int serNumb, int spriteVal,int elC,  int score1, int star1, int star2, int star3)
    {
        goal = goal1;
        criteria = criteria1;
        serialNumber = serNumb;
        if(goal == 1)
            spriteVal = 0;
        else
            spriteVal = spriteVal;
        //unlocked =  unlocked1;bool unlocked1,
        elementCount = elC;
        score = score1;
        oneStar = star1;
        twoStar = star2;
        threeStar = star3;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.Append("\"goal\":"+goal+",");
        sb.Append("\"criteria\":"+criteria +",");
        sb.Append("\"serialNumber\":"+ serialNumber +",");
        sb.Append("\"spriteValue\":"+ spriteValue +",");
        //sb.Append("\"unlocked\":"+ unlocked +",");
        sb.Append("\"score\":"+ score +",");
        sb.Append("\"oneStar\":"+ oneStar +",");
        sb.Append("\"twoStar\":"+ twoStar +",");
        sb.Append("\"threeStar\":"+ threeStar );
        sb.Append("}");

        return sb.ToString();
    }
   

}
