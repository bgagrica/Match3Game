using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMarkerPiece : MonoBehaviour
{
    public Point index;
    public  int value;
    Image img;
     [HideInInspector]
    public RectTransform rect;
    public SelectMarkerPiece(Point p, int val)
    {
        index = p;
        value = val;
    }
    
   void UpdateName()
    {
        transform.name = "SelectionMarkerPiece[" + index.x + "," + index.y + "]";
    }
    
    public void ResePosition()
    {
            
    }

    public void Initialize(int v, Point p, Sprite piece)
    {
        
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        value = v;
        index = p;
        img.sprite = piece;
         UpdateName();
    }
   

    

   
}
