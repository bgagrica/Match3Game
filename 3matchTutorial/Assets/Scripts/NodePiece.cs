using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int value;
    public Point index;
    public int superPiecePower;
    bool updating; 

    [HideInInspector]
    public Vector2 pos;

    [HideInInspector]
    public RectTransform rect;

    Image img;
    public static Point ptn;

    public void Initialize(int v, Point p, Sprite piece, int super)
    {
        ptn = new Point(0,0);
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        value = v;
        SetIndex(p);
        img.sprite = piece;
        superPiecePower = super != 0 ? super : 0 ;
    }
   
    public void SetIndex(Point p)
    {
        index = p;
        ResetPosition();
        UpdateName();
    }


    public void ResetPosition()
    {
        pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    void UpdateName()
    {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }



    public void MovePosition(Vector2 move)
    {
        rect.anchoredPosition += move * Time.deltaTime * 16f;
    }

    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    public bool UpdatePiece()
    {
        if(Vector3.Distance(rect.anchoredPosition, pos) > 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
        ///return false if it is not moving
    }
     
    public void OnPointerDown(PointerEventData eventData)
    {
        
        if(updating) return;
        ptn = new Point(index.x, index.y);
        MovePieces.instance.MovePiece(this);
          
       
    }
  
    public void OnPointerUp(PointerEventData eventData)
    {
       Debug.Log("Let go "+ transform.name);
       
        MovePieces.instance.DropPiece();

    }

    public static Point getClickPoint()
    {
        return ptn;
    }

    
}
