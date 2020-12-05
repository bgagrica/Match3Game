using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    public Sprite[] pieces;
    public Sprite superPiece;
    public Sprite markerSprite;
    public Button confirmButton;
    public Button cancelButton;
    public int typeOFLevel; // 1 = score and moves ; 2 = score and time  ; 3 = item and moves  ; 4 = item and time  --- score exist in all levels




    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject killedPiece;
    public GameObject selectMarkerPiece;
    public Image elementPiece;

    public bool isMatching = true;
    int width = 9;
    int height = 9;
    int[] fills;
    Node[,] board;
    SelectMarkerPiece[,] selectinBoard;
    public int score = 0;
    public int moves;
    public int typeOfPowerUp = 0; // 0 nothing ; 1 hammer ; 2 destroy All of elements


    Point newSelectMarker;
    Point lastSelectMarker;
    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    List<KilledPiece> killed;
    System.Random random;
    public static Point ptn;
    LevelDataClass levelData;

    public Text scoreValueText;
    public Text movesText;
    public Text targetPieceCountText;
    public Text levelText;
    public int targetCount = 0;
    public GameObject[] emptyStars;
    public GameObject[] fullStars;
    public GameObject[,] elementsOnBoard;


    // Start is called before the first frame update
    void Start()
    {

        startGame();
    }


    void startGame()
    {

        
        // MovePieces mp = new MovePieces();
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();
        fills = new int[width];

        newSelectMarker = new Point(4, 7);

        InitializeBoard();
        InitializeSelectionBoard();
        VerifyBoard();
        InstantiateBoard();
        levelData = SaveSystem.Load();
        setLevel();
        UpdateText();

        levelText.text = "Level: " + levelData.serialNumber.ToString();

    }

    void setLevel()
    {


        PauseMenu.winCondInt = levelData.elementCount;
        targetPieceCountText.gameObject.SetActive(true);
        elementPiece.sprite = pieces[levelData.spriteValue - 1];
        elementPiece.gameObject.SetActive(true);

        moves = levelData.criteria;

    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));

                int val = node.value;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1], 0);
                node.setPiece(piece);

            }
        }
    }

    void KillPiece(Point p)
    {

        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killed.Count; i++)
            if (!killed[i].falling) available.Add(killed[i]);

        KilledPiece set = null;
        if (available.Count > 0)
            set = available[0];
        else
        {
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);

        }

        int val = getValueAtPoint(p) - 1;
        if (set != null && val >= 0 && val < pieces.Length)
            set.Initialize(pieces[val], getPositionFromPoint(p));

    }

    void InitializeBoard()
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : fillPiece(), new Point(x, y));
            }

        }


    }

    void InitializeSelectionBoard()
    {
        selectinBoard = new SelectMarkerPiece[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                int val = (boardLayout.rows[y].row[x]) ? -1 : 1;
                if (val <= 0) continue;
                GameObject p = Instantiate(selectMarkerPiece, gameBoard);
                SelectMarkerPiece piece = p.GetComponent<SelectMarkerPiece>();
                selectinBoard[x, y] = piece;
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), markerSprite);
                piece.gameObject.SetActive(false);

            }

        }
    }

    void VerifyBoard()
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;
                remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {

                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);


                    setValueAtPoint(p, newValue(ref remove));

                }

            }

        }

    }

    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach (int i in remove)
        {
            if (available.Contains(i))
                available.Remove(i);
        }
        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];
    }

    List<Point> isConnected(Point p, bool main)
    {

        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);

        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach (Point dir in directions) // checking if there is 2 or more shapes in same direction
        {
            List<Point> line = new List<Point>();

            int same = 0;

            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;

                }
                if (same > 1)
                    AddPoints(ref connected, line);

            }
        }
        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();
            int same = 0;
            Point[] check =
            {
              Point.add(p, directions[i]),
              Point.add(p, directions[i+2])
          };
            foreach (Point next in check)
            {
                if (getValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;

                }
            }
            if (same > 1)
                AddPoints(ref connected, line);




        }

        if (main)
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, isConnected(connected[i], false));
            }
        }
        /* UNNESSASARY
        if (connected.Count > 0)
            connected.Add(p);
        */
        return connected;
    }

    List<Point> isConSuffle(int val, Point p, bool main)
    {

        List<Point> connected = new List<Point>();
       

        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach (Point dir in directions) // checking if there is 2 or more shapes in same direction
        {
            List<Point> line = new List<Point>();

            int same = 0;

            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;

                }
                if (same > 1)
                    AddPoints(ref connected, line);

            }
        }
        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();
            int same = 0;
            Point[] check =
            {
              Point.add(p, directions[i]),
              Point.add(p, directions[i+2])
          };
            foreach (Point next in check)
            {
                if (getValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;

                }
            }
            if (same > 1)
                AddPoints(ref connected, line);




        }

        if (main)
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, isConnected(connected[i], false));
            }
        }
        /* UNNESSASARY
        if (connected.Count > 0)
            connected.Add(p);
        */
        return connected;
    }

    bool canMakeMatch()
    {
        List<Point> helpList = null;
        Point[] directions =
       {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for(int h = 0; h < 4; h++)
                {
                    Point helpPoint = new Point(x, y);
                    helpPoint.add(directions[h]);
                    if (helpPoint.x < 0 || helpPoint.x > width || helpPoint.y < 0 || helpPoint.y > height) continue;
                    
                    int val = board[x, y].value;
                    board[x, y].value = 0;
                    helpList = isConSuffle(val, helpPoint, false);
                    board[x, y].value = val;
                    if (helpList.Count == 0) continue;
                    else
                    {
                       // Debug.Log(val);
                       
                        return true;
                        // zovi suggest
                       
                    }
                    
                }
               
                
               
            }

        }
        
        return false;
        


    }

    private void suggestHelp()
    {


    }

    private void shuffle()
    {

        foreach (Transform child in gameBoard.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        update.Clear();
        InitializeBoard();
        InitializeSelectionBoard();
        VerifyBoard();
        InstantiateBoard();

    }
   


    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;



            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }
            if (doAdd) points.Add(p);
        }
    }

    int getValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    void setValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    int fillPiece()
    {

        int val = 1;

        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;

        return val;
    }

    public String getRandomSeed()
    {
        String seed = "";
        String acceptableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[UnityEngine.Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }

    void ifWonIsActive()
    {


        int firstStar = levelData.oneStar;
        int secondStar = levelData.twoStar;
        int thirdStar = levelData.threeStar;

        if (score < levelData.oneStar)
            score = levelData.oneStar;
       // if (moves > 0)
       // {
        //    moves--;
        //    score += 300;
       // }

        if (levelData.score < score) ;
        levelData.score = score;

        if (score >= thirdStar)
            for (int i = 0; i < fullStars.Length; i++)
            {
                fullStars[i].gameObject.SetActive(true);

            }

        if (score >= secondStar)
        {
            for (int i = 0; i < fullStars.Length - 1; i++)
                fullStars[i].gameObject.SetActive(true);

        }
        if (score >= firstStar)
            fullStars[0].gameObject.SetActive(true);





    }

    void Update()
    {
        UpdateText();
        

        if (PauseMenu.winCond == true)
        {
            ifWonIsActive();
            SaveSystem.Save(levelData);
        }

       // if (canMakeMatch() == false) shuffle();

        if (!isMatching)
        {
            ptn = NodePiece.getClickPoint();
            changeSelection(ptn);
        }
        List<NodePiece> finishedUpdating = new List<NodePiece>();

        for (int i = 0; i < update.Count; i++)
        {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece())
            {
                finishedUpdating.Add(piece);
            }

        }
        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePiece flippedPiece = null;

            int x = (int)piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);
            Point flippedPoint = null;
            Point flippedPoint1 = piece.index;
            List<Point> connected = isConnected(piece.index, true);
            int connectedCount = connected.Count;
            int connectedCount1 =  0 ;


            bool wasFlipped = (flip != null);

            if (wasFlipped)// If we flipped to make this update
            {   
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
                if(flippedPiece.value == 6)
                {
                  
                    activateSuperElement(flippedPiece.index);
                    flipped.Remove(flip); // Remove the flip after update
                    update.Remove(piece);
                    break;
                }
                flippedPoint = flippedPiece.index;
            }
            
            if (connected.Count == 0) // If we didn't make match
            {
                
                if (wasFlipped) // If we Flipped
                    FlipPieces(piece.index, flippedPiece.index, false); // Flip back
            }
            else // If we made match
            {
                
                if (wasFlipped)
                 moves--;

                bool check = false;
                //Debug.Break();
                if(connected.Count >= 5)
                {
                    check = true;
                }
                List<Point> powerPoints = new List<Point>();
                if (check)
                {
                   // List<int> lista = new List<int>();
                    foreach (Point pnt in connected) // Remove the node piececs connected
                    {
                        if(isSuperElement(pnt))
                            powerPoints.Add(pnt);
                        
                    }

                }
                foreach (Point pnt in connected) // Remove the node piececs connected
                {
                    
                   
                    
                    Node node = getNodeAtPoint(pnt);
                    NodePiece nodePiece = node.getPiece();
                    // KillPiece(pnt);
                    if (nodePiece != null)
                    {

                      
                        //dead.Add(nodePiece);
                        nodePiece.gameObject.SetActive(false);
                            checkElement(pnt);
                            node.setPiece(null);
   

                    }

                    score += 100;


                }

                if(powerPoints.Count != 0)
                {
                    Debug.Log("Usao u super element creation");
                    foreach (Point point in powerPoints)
                    {

                       

                        Node node = getNodeAtPoint(point);

                        int val = 6;
                        
                        if (val <= 0) continue;
                        GameObject p = Instantiate(nodePiece, gameBoard);
                        NodePiece piece1 = p.GetComponent<NodePiece>();
                        RectTransform rect = p.GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(32 + (64 * point.x), -32 - (64 * point.y));
                        piece1.Initialize(val, point, superPiece, 0);
                        node.setPiece(piece1);
                        Debug.Log("NAPRAVIO");

                    }
                    powerPoints.Clear();

                }
                
                    ApplyGravityToBoard();
                
              
               
                    /*flippedPoint
                    if (getNodeAtPoint(flippedPoint1) != null)
                    {
                       

                        NodePiece piece2;

                        GameObject obj = Instantiate(nodePiece, gameBoard);
                        NodePiece n = obj.GetComponent<NodePiece>();
                        RectTransform rect = obj.GetComponent<RectTransform>();
                        rect.anchoredPosition = getPositionFromPoint(flippedPoint1);
                        piece2 = n;

                    }*/





                


               
            }

            flipped.Remove(flip); // Remove the flip after update
            update.Remove(piece);

        }

    }

    void activateSuperElement(Point point)
    {

        Point[] directions =
       {
            new Point(0,0),
           
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach(Point dir in directions)
        {

            Point pon = Point.add(point,dir);
            int x = pon.x;
            int y = pon.y;
            if (x < 0 || x > height || y < 0 || y > width) continue;
            
            //KillPiece(pon);
            Node node = getNodeAtPoint(pon);
            NodePiece nodePiece = node.getPiece();
            if (nodePiece != null)
            { 
                nodePiece.gameObject.SetActive(false);
            }
            
            score += 100;
            checkElement(pon);
            node.setPiece(null);

        }
       

    }

    bool isSuperElement(Point point)
    {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(point);
        //Debug.Log("Usao u metodu isSUperEllement");
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };
        int same = 0;
        foreach (Point dir in directions) // checking if there is 2 or more shapes in same direction
        {
            // Debug.Log("Usao u metodu isSUperEllement for each");


            int same1 = 0;
            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(point, Point.mult(dir, i));

                
                if (getValueAtPoint(check) == val)
                {
                   // Debug.Log("Usao u metodu isSUperEllement proverio dir");
                    same1++;
                    //Debug.Log(same + " za tacku "+ point.x +"/"+ point.y);
                }
                if(same1 >= 2)
                {
                    same += same1;
                    if (same >= 4)
                        return true;

                }

            }
        }
        

        return false;
    }




    public void checkElement(Point pnt)
    {
       if(levelData.spriteValue == getValueAtPoint(pnt))
                        targetCount++;
    }

    void UpdateText()
    {
        
        PauseMenu.moves = moves;
        PauseMenu.targetPieceCount = targetCount;
        targetPieceCountText.text = targetCount.ToString() + "/" + levelData.elementCount.ToString() ;
        scoreValueText.text = score.ToString();
        movesText.text = moves.ToString();
            //moves.ToString();
    }

    void ApplyGravityToBoard()
    {
        
        for(int x = 0 ; x < width ; x++)
        {
            for(int y = (height-1) ; y >= 0 ; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if(val != 0)continue; // if it is not a hole do nothing
                
                for(int ny = (y-1); ny >= -1 ; ny--)
                {
                    Point next = new Point(x,ny);
                    int nextVal = getValueAtPoint(next);
                    if(nextVal == 0)
                        continue;
                    if(nextVal != -1)// If we did not hit an end but its not 0 then use this to fill the current hole
                    {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.getPiece();

                        // Set the hole
                        node.setPiece(piece);
                        update.Add(piece);

                        // Replace the hole
                        got.setPiece(null);                  
                    }
                    else // Hit the end
                    {
                        int newVal = fillPiece();
                        NodePiece piece;
                        Point fallPnt = new Point(x, (-1-fills[x]));

                        if(dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            revived.rect.anchoredPosition = getPositionFromPoint(fallPnt);
                            piece = revived;
                            
                           
                            dead.RemoveAt(0);
                        }
                        else
                        {
                            
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            rect.anchoredPosition = getPositionFromPoint(fallPnt);
                            piece = n;
                        }

                        piece.Initialize(newVal, p, pieces[newVal - 1], 0);
                            
                         Node hole = getNodeAtPoint(p);
                         hole.setPiece(piece);
                         ResetPiece(piece);
                        fills[x]++;
                    
                    } 
                    break;
                }


            }
        
        }
    
    
    }

    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for(int i = 0 ; i < flipped.Count ; i++)
        {
           if( flipped[i].getOtherPiece(p) != null)
           {
            flip = flipped[i];
            break;
           }
        }

        return flip;
    }  

    public void ResetPiece(NodePiece piece)
    {

        piece.ResetPosition();
        
        update.Add(piece); 

    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if(getValueAtPoint(one) < 0) return;

        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();

        if(getValueAtPoint(two) > 0)
        {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();
            nodeOne.setPiece(pieceTwo);
            nodeTwo.setPiece(pieceOne);

            if(main)
                flipped.Add(new FlippedPieces(pieceOne,pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        
        }
        else
        ResetPiece(pieceOne);
    }

    public void activatePowerUp()
    {
        isMatching = false;
        selectinBoard[newSelectMarker.x,newSelectMarker.y].gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    
    }

     public void endPowerUp()
    {
         cancelButton.gameObject.SetActive(false);
         confirmButton.gameObject.SetActive(false);
         setSellectionToFalse();
         isMatching = true;
    }
    
    public void OnButtonHammerClick()
    {
        typeOfPowerUp = 1;
        activatePowerUp();
    }

    public void OnButtonDestroyAllTypeOfElement()
    {
        typeOfPowerUp = 2;
        activatePowerUp();
    }

    public void OnButtonDestroyRowAndColPowerUp()
    {
        typeOfPowerUp = 3;
        activatePowerUp();
    }

    public void OnButtonConfirmClick()
    {
        if(typeOfPowerUp == 1)
        {
            hammerPowerUp();
        }
        
        if(typeOfPowerUp == 2)
        {
            DestroyAllTypeOfElementPowerUp();
        }

        if(typeOfPowerUp == 3)
        {
               destroyRowAndColPowerUp();
        }
    
        if(typeOfPowerUp != 0)endPowerUp();
    }

    public void hammerPowerUp()
    {
         KillPiece(ptn);
            Node node = getNodeAtPoint(ptn);
            NodePiece nodePiece = node.getPiece();
            if(nodePiece != null)
                {
                     dead.Add(nodePiece);
                     nodePiece.gameObject.SetActive(false);
                } 
            score += 100; 
        checkElement(ptn);
            node.setPiece(null);
              
            ApplyGravityToBoard();
    
    }

    public void DestroyAllTypeOfElementPowerUp()
    {
        int valueOfObj = getValueAtPoint(ptn);
            foreach(Node node1 in board) // Remove the node piececs connected
            {   
                    Point point1 = new Point(node1.index.x,node1.index.y);
                    int node1Val = node1.value;
                    if(valueOfObj == node1Val)
                    {
                    KillPiece(point1);
                    Node node = getNodeAtPoint(point1);
                    NodePiece nodePiece = node.getPiece();
                    if(nodePiece != null)
                    {
                        dead.Add(nodePiece);
                        nodePiece.gameObject.SetActive(false);
                        checkElement(point1);
                    }
                    score += 100; 
                    node.setPiece(null);




                    }
                    
                    
            }
             ApplyGravityToBoard();
        
    }

    public void destroyRowAndColPowerUp()
    {
        
        for(int i = 0; i < height ; i++){
            
            for(int j = 0; j < width ; j++){
            
                if( i == ptn.x || j == ptn.y )
                {   
                    Point pon = new Point(i,j);
                     KillPiece(pon);
                     Node node = getNodeAtPoint(pon);
                     NodePiece nodePiece = node.getPiece();
                      if(nodePiece != null)
                      {
                          dead.Add(nodePiece);
                          nodePiece.gameObject.SetActive(false);
                      } 
                     score += 100; 
                     checkElement(pon);
                     node.setPiece(null);      
                }
            }
        }
         ApplyGravityToBoard();
    
    }
    
    public void OnButtonCancelClick()
    {
        endPowerUp();
    }

    public void setSellectionToFalse()
    {
        selectinBoard[newSelectMarker.x,newSelectMarker.y].gameObject.SetActive(false);
    }

    public void changeSelection(Point npoint)
    {

        lastSelectMarker = newSelectMarker;
        newSelectMarker = npoint;
        selectinBoard[lastSelectMarker.x,lastSelectMarker.y].gameObject.SetActive(false);
        selectinBoard[newSelectMarker.x,newSelectMarker.y].gameObject.SetActive(true);


    }

}

[System.Serializable]
public class Node
{
    public int value; // -1 = hole 0 = blank, 1 = cube, 2 = sphere, 3 = cylinder, 4 = pyramid, 5 = diamond
    public Point index;
    public NodePiece piece;

    public Node(int v, Point i)
    {
        value = v;
        index = i;

    }

    public void setPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if(piece == null)return;
        piece.SetIndex(index);
    }
    
    public NodePiece getPiece()
    {
        return piece;
    }

}

[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t)
    {
        one = o;
        two = t;
    }
    
    public NodePiece getOtherPiece(NodePiece p)
    {
        if(p == one)
            return two;
        else if(p == two)
            return one;
        else
            return null;
    }
}

