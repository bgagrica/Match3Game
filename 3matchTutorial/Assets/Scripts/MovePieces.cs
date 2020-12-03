﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
   
   public static MovePieces instance;
    Match3 game;
    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;
    

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
       
        game = GetComponent<Match3>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(moving != null)
        {
            Vector2 dir = ((Vector2) Input.mousePosition - mouseStart);
            Vector2 nDir = dir.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y) );
        
            newIndex = Point.clone(moving.index);
            Point add = Point.zero;
            if(dir.magnitude > 32)// If our mouse is 32 pixels away from the starting point of the mouse
            {
                    // Make add either (1, 0) or (-1, 0) or (0, 1) or (0, -1) depending on the direction of the mouse
                    if(aDir.x > aDir.y)
                        add = new Point((nDir.x > 0) ? 1 : -1, 0);
                    else if(aDir.y > aDir.x)
                         add = new Point( 0, (nDir.y > 0) ? -1 : 1);

                      
            }
            newIndex.add(add);
            

            Vector2 pos = game.getPositionFromPoint(moving.index);
            if(!newIndex.Equals(moving.index))
                 pos += Point.mult(new Point(add.x, -add.y),16).toVector();
            moving.MovePositionTo(pos);
        }


    }

    public void MovePiece(NodePiece piece)
    {
        if(game.isMatching){
           if(moving != null) return;
           moving = piece;
           mouseStart = Input.mousePosition;
        
        
        }
        
    
    }

    public void DropPiece()
    {
         if(game.isMatching){
        if(moving == null)return;
       // Debug.Log("Dropped");
        // if newIndex != moving.index
        if(!newIndex.Equals(moving.index))
            game.FlipPieces(moving.index, newIndex, true);
        else
            game.ResetPiece(moving);
        
        // Flip the pieces around in the game board
        // else
        
        moving = null;
        }
    }
}
