using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //References
    public GameObject controller;
    public GameObject movePlate;

    //positions
    private int xBoard = -1;
    private int yBoard = -1;

    //Variable to keep track of "black" or "white" player
    private string player;

    //References for all the sprites that are chess pieces
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;
    public Sprite mine;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;

            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;

            case "mine": this.GetComponent<SpriteRenderer>().sprite = mine; player = ""; break;

        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.64f;
        y *= 0.64f;

        x += -2.25f;
        y += -2.25f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }
    public int GetXBoard()
    {
        return xBoard;
    }
    public int GetYBoard()
    {
        return yBoard;
    }
    public void SetXBoard(int x)
    {
        xBoard = x;
    }
    public void SetYBoard(int y)
    {
        yBoard = y;
    }
    public string GetPiecePlayer()
    {
        return player;
    }
    private void OnMouseUp()
    {
        DestroyMovePlates();

        Game sc = controller.GetComponent<Game>();
        if (player == sc.GetPlayer() && player == "white")
        {
            InitiateMovePlates();
        }
    }
    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }
    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;

        }

    }
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }
    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);
            if (cp == null) {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }
    //controls how pawns move
    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            //if nothing is directly in front of pawn, spawn moveplate
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
            }
            //if nothing is diagonally from pawn, spawn moveplates
            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) == null)
            {
                MovePlateSpawn(x + 1, y);
            }
            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) == null)
            {
                MovePlateSpawn(x - 1, y);
            }
            //if there is an enemy piece diagonally from pawn, attack
            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null
                && sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }
            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null
                && sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
            //initial double move check
            //white pawn: if on second row, nobody is occupying spot, and piece is white, allow double move
            if (y == 2 && sc.GetPosition(x, y + 1) == null && sc.GetPosition(x, y - 1).GetComponent<Chessman>().player == "white")
            {
                MovePlateSpawn(x, y + 1);
            }
            //black pawn: if on seventh row, nobody is occupying spot, and piece is black, allow double move
            if (y == 5 && sc.GetPosition(x, y - 1) == null && sc.GetPosition(x, y + 1).GetComponent<Chessman>().player == "black")
            {
                MovePlateSpawn(x, y - 1);
            }
        }
    }
    public void MovePlateSpawn(int matrixX, int matrixY)
        {
            float x = matrixX;
            float y = matrixY;
            x *= 0.64f;
            y *= 0.64f;

            x += -2.25f;
            y += -2.25f;

            GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

            MovePlate mpScript = mp.GetComponent<MovePlate>();
            mpScript.SetReference(gameObject);
            mpScript.SetCoords(matrixX, matrixY);

        }
        public void MovePlateAttackSpawn(int matrixX, int matrixY)
        {
            float x = matrixX;
            float y = matrixY;

            x *= 0.64f;
            y *= 0.64f;

            x += -2.25f;
            y += -2.25f;

            GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

            MovePlate mpScript = mp.GetComponent<MovePlate>();
            mpScript.attack = true;
            mpScript.SetReference(gameObject);
            mpScript.SetCoords(matrixX, matrixY);

        }
    
}
