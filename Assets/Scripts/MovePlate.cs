using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    //Board Positions not world positions
    int matrixX;
    int matrixY;

    //is the piece attacking?
    //false is movement, true is attacking
    public bool attack = false;

public void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        if (attack)
        {
            //change to red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
    //move piece event
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        //take piece
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);
            Destroy(cp);
        }
        //move piece to new square
        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
        reference.GetComponent<Chessman>().GetYBoard());
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();
        controller.GetComponent<Game>().SetPosition(reference);
        //discard move plates
        reference.GetComponent<Chessman>().DestroyMovePlates();

        //blow up piece if mine stepped on
        int mineCount = 5;
        mineCount = PlayerPrefs.GetInt("minecount");
        for (int i = 0; i < mineCount; i++)
        {
            if (reference.GetComponent<Chessman>().GetXBoard() == controller.GetComponent<Game>().minesArray[i, 0] && reference.GetComponent<Chessman>().GetYBoard() == controller.GetComponent<Game>().minesArray[i, 1])
            {
                //todo: mine explode effect
                GameObject cp = controller.GetComponent<Game>().GetPosition(controller.GetComponent<Game>().minesArray[i, 0], controller.GetComponent<Game>().minesArray[i, 1]);
                Destroy(cp);
                //defuse mine afterward
                controller.GetComponent<Game>().minesArray[i, 0] = -1;
                controller.GetComponent<Game>().minesArray[i, 1] = -1;
            }
        }
        //change player
        if (reference.GetComponent<Chessman>().GetPiecePlayer() == "white")
        {
            controller.GetComponent<Game>().SetPlayer("black");
            controller.GetComponent<Game>().statusText.text = "Black to move, AI is thinking...";
        }
        else
        {
            controller.GetComponent<Game>().SetPlayer("white");
            controller.GetComponent<Game>().statusText.text = "White to move";
        }
    }
    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }
    public GameObject GetReference()
    {
        return reference;
    }
}
