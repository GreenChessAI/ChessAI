using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];
    public int[,] minesArray = new int[5, 2] { { 0, 3 }, { 2, 3 }, { 4, 3 }, { 6, 3 }, { 7, 3 } };
    public TMP_Text statusText;
    float startTime = 0.0f;
    bool timerStart = true;

    private string currentPlayer = "white";

    private bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        //create mines
        int mineCount = 5;
        mineCount = PlayerPrefs.GetInt("minecount");
        for (int i = 0; i < mineCount; i++)
        {
            int randX = Random.Range(0, 8);
            int randY = Random.Range(2, 6);
            minesArray[i,0] = randX;
            minesArray[i,1] = randY;
        }

        //display for 5 seconds
        ShowMines(mineCount);


        //piece randomization - shuffle around all starting piece spawns
        int[] randomList = new int[] {0, 1, 2, 3, 4, 5, 6, 7};
        for (int i = 7; i > 1; i--)
        {
            int randInt = Random.Range(0, i + 1);
            int value = randomList[randInt];
            randomList[randInt] = randomList[i];
            randomList[i] = value;
        }

        //places pieces according to randomly generated list
        playerWhite = new GameObject[]
        {
            Create("white_rook", randomList[0], 0),
            Create("white_knight", randomList[1], 0),
            Create("white_bishop", randomList[2], 0),
            Create("white_queen", randomList[3], 0),
            Create("white_king", randomList[4], 0),
            Create("white_bishop", randomList[5], 0),
            Create("white_knight", randomList[6], 0),
            Create("white_rook", randomList[7], 0),
            Create("white_pawn", 0, 1),
            Create("white_pawn", 1, 1),
            Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1),
            Create("white_pawn", 4, 1),
            Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1),
            Create("white_pawn", 7, 1)
        };
        playerBlack = new GameObject[]
        {
            Create("black_rook", randomList[0], 7),
            Create("black_knight", randomList[1], 7),
            Create("black_bishop", randomList[2], 7),
            Create("black_queen", randomList[3], 7),
            Create("black_king", randomList[4], 7),
            Create("black_bishop", randomList[5], 7),
            Create("black_knight", randomList[6], 7),
            Create("black_rook", randomList[7], 7),
            Create("black_pawn", 0, 6),
            Create("black_pawn", 1, 6),
            Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6),
            Create("black_pawn", 4, 6),
            Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6),
            Create("black_pawn", 7, 6)
        };

        //Set all piece positions on position board
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }
    
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void ShowMines(int mineCount)
    {
        for (int i = 0; i < mineCount; i++)
        {
            GameObject tempMine = Create("mine", minesArray[i, 0], minesArray[i, 1]);
        }
    }

    public void HideMines()
    {
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();
        for (int i = 0; i < allSprites.Length; i++)
        {
            if (allSprites[i].sprite.name == "mine2") {
                Destroy(allSprites[i]);
            }
        }
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >=positions.GetLength(0)||y>=positions.GetLength(1))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public string GetPlayer()
    {
        return currentPlayer;
    }

    public void SetPlayer(string player)
    {
        currentPlayer = player;
    }

    // Update is called once per frame
    void Update()
    {
        //hide mines after 5 seconds
        if (timerStart)
        {
            startTime = Time.time;
            timerStart = false;
        }
        if (Time.time - startTime > 5)
        {
            //Hide mines
            HideMines();

            //Change label if not already changed
            if (statusText.text == "Mines will disappear in 5 seconds...")
            {
                statusText.text = "White to move";
            }
        }

        //scan for keyboard input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0); //load scene 0 (quit to main menu)
        }
        //debug: end game
        if (Input.GetKeyDown(KeyCode.T))
        {
            gameOver = true;
        }
        //destroy move plates if piece not clicked
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider == null)
            {
                GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                for (int i = 0; i < movePlates.Length; i++)
                {
                    Destroy(movePlates[i]);
                }
            }
        }
        if (gameOver)
        {
            statusText.text = "Checkmate! White wins! Press (esc) to continue...";
        }
    }

    public class Board
    {
        private GameObject[,] positions;

        public Board()
        {
            positions = new GameObject[8, 8];
        }

        public bool IsEmpty(int x, int y)
        {
            return positions[x, y] == null;
        }

        public bool IsOpponent(int x, int y, PieceColor color)
        {
            if (!IsEmpty(x, y))
            {
                Chessman chessman = positions[x, y].GetComponent<Chessman>();
                return chessman.color() != color.ToString();
            }
            return false;
        }

        public void SetPiece(GameObject piece, int x, int y)
        {
            positions[x, y] = piece;
        }

        public void RemovePiece(int x, int y)
        {
            positions[x, y] = null;
        }

        public Chessman GetPosition(int i, int j)
        {
            return positions[i, j].GetComponent<Chessman>();
        }
    }
    public string GetPlayer()
    {
        return currentPlayer;
    }

    public void SetPlayer(string player)
    {
        currentPlayer = player;
    }

    public class LegalMoves
    {
        private GameObject[,] positions;
        private string currentPlayer;

        public LegalMoves(GameObject[,] positions, string currentPlayer)
        {
            this.positions = positions;
            this.currentPlayer = currentPlayer;
        }

        public List<Vector2Int> GetLegalMoves(int x, int y)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();
            Chessman chessman = positions[x, y].GetComponent<Chessman>();
            PieceType type = chessman.type;
            PieceColor color = System.Enum.Parse<PieceColor>(chessman.color());
            switch (type)
            {
                case PieceType.Pawn:
                    // Logic for pawn moves
                    int direction = color == PieceColor.White ? 1 : -1;
                    int startingRow = color == PieceColor.White ? 1 : 6;
                    int forwardOneRow = x + direction;
                    int forwardTwoRows = x + (direction * 2);

                    if (Board.IsEmpty(forwardOneRow, y))
                    {
                        AddIfValid(forwardOneRow, y, legalMoves);
                    }
                    if (x == startingRow && Board.IsEmpty(forwardOneRow, y) && Board.IsEmpty(forwardTwoRows, y))
                    {
                        AddIfValid(forwardTwoRows, y, legalMoves);
                    }
                    if (Board.IsOpponent(x + direction, y, color))
                    {
                        AddIfValid(x + direction, y + 1, legalMoves);
                    }
                    if (Board.IsOpponent(x + direction, y, color))
                    {
                        AddIfValid(x + direction, y - 1, legalMoves);
                    }
                    break;
                case PieceType.Knight:
                    // Logic for knight moves
                    AddIfValid(x + 1, y + 2, legalMoves);
                    AddIfValid(x + 2, y + 1, legalMoves);
                    AddIfValid(x + 2, y - 1, legalMoves);
                    AddIfValid(x + 1, y - 2, legalMoves);
                    AddIfValid(x - 1, y - 2, legalMoves);
                    AddIfValid(x - 2, y - 1, legalMoves);
                    AddIfValid(x - 2, y + 1, legalMoves);
                    AddIfValid(x - 1, y + 2, legalMoves);
                    break;
                case PieceType.Bishop:
                    // Logic for bishop moves
                    int[,] directions = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
                    for (int i = 0; i < 4; i++)
                    {
                        int dx = directions[i, 0];
                        int dy = directions[i, 1];
                        int currX = x + dx;
                        int currY = y + dy;
                        while (currX >= 0 && currX <= 7 && currY >= 0 && currY <= 7)
                        {
                            if (positions[currX, currY] == null)
                            {
                                AddIfValid(currX, currY, legalMoves);
                            }
                            else if (positions[currX, currY].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(currX, currY, legalMoves);
                                break;
                            }
                            else
                            {
                                break;
                            }
                            currX += dx;
                            currY += dy;
                        }
                    }
                    break;
                case PieceType.Rook:
                    // Logic for rook moves
                    for (int i = x + 1; i <= 7; i++)
                    {
                        if (positions[i, y] == null)
                        {
                            AddIfValid(i, y, legalMoves);
                        }
                        else
                        {
                            if (positions[i, y].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(i, y, legalMoves);
                            }
                            break;
                        }
                    }
                    for (int i = x - 1; i >= 0; i--)
                    {
                        if (positions[i, y] == null)
                        {
                            AddIfValid(i, y, legalMoves);
                        }
                        else
                        {
                            if (positions[i, y].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(i, y, legalMoves);
                            }
                            break;
                        }
                    }
                    for (int j = y + 1; j <= 7; j++)
                    {
                        if (positions[x, j] == null)
                        {
                            AddIfValid(x, j, legalMoves);
                        }
                        else
                        {
                            if (positions[x, j].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(x, j, legalMoves);
                            }
                            break;
                        }
                    }
                    for (int j = y - 1; j >= 0; j--)
                    {
                        if (positions[x, j] == null)
                        {
                            AddIfValid(x, j, legalMoves);
                        }
                        else
                        {
                            if (positions[x, j].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(x, j, legalMoves);
                            }
                            break;
                        }
                    }
                    break;
                case PieceType.Queen:
                    // Logic for queen moves
                    int[,] queenDirections = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 }, { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
                    for (int i = 0; i < 8; i++)
                    {
                        int dx = queenDirections[i, 0];
                        int dy = queenDirections[i, 1];
                        int currX = x + dx;
                        int currY = y + dy;
                        while (currX >= 0 && currX <= 7 && currY >= 0 && currY <= 7)
                        {
                            if (positions[currX, currY] == null)
                            {
                                AddIfValid(currX, currY, legalMoves);
                            }
                            else if (positions[currX, currY].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(currX, currY, legalMoves);
                                break;
                            }
                            else
                            {
                                break;
                            }
                            currX += dx;
                            currY += dy;
                        }
                    }
                    break;

                case PieceType.King:
                    // Logic for king moves
                    int[,] kingMoves = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 }, { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
                    for (int i = 0; i < 8; i++)
                    {
                        int dx = kingMoves[i, 0];
                        int dy = kingMoves[i, 1];
                        int currX = x + dx;
                        int currY = y + dy;
                        if (currX >= 0 && currX <= 7 && currY >= 0 && currY <= 7)
                        {
                            if (positions[currX, currY] == null || positions[currX, currY].GetComponent<Chessman>().color() != currentPlayer)
                            {
                                AddIfValid(currX, currY, legalMoves);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }

            return legalMoves;
        }

        private void AddIfValid(int x, int y, List<Vector2Int> moves)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7)
            {
                return;
            }
            if (positions[x, y] == null)
            {
                moves.Add(new Vector2Int(x, y));
            }
            else if (positions[x, y].GetComponent<Chessman>().color() != currentPlayer)
            {
                moves.Add(new Vector2Int(x, y));
            }
        }
    }

    //here will be the hueristic values for the AI
    public float reverseArray(float[][] array)
    {
        return array.Reverse();
    }

    public float evaluateBoard(Board board)
    {
        float totalEvaluation = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                totalEvaluation += GetPieceValue(board.GetPosition(i, j), i, j);
            }
        }

        return totalEvaluation;
    }

    public float GetPieceValue(Piece piece, int x, int y)
    {

        if (piece == null)
        {
            return 0;
        }

        Func<Piece, bool, int, int, int> getAbsoluteValue = (p, isWhite, row, col) =>
        {
            if (p.Type == PieceType.Pawn)
            {
                return 10 + (isWhite ? PawnEvalWhite[row, col] : PawnEvalBlack[row, col]);
            }
            else if (p.Type == PieceType.Rook)
            {
                return 50 + (isWhite ? RookEvalWhite[row, col] : RookEvalBlack[row, col]);
            }
            else if (p.Type == PieceType.Knight)
            {
                return 30 + KnightEval[row, col];
            }
            else if (p.Type == PieceType.Bishop)
            {
                return 30 + (isWhite ? BishopEvalWhite[row, col] : BishopEvalBlack[row, col]);
            }
            else if (p.Type == PieceType.Queen)
            {
                return 90 + EvalQueen[row, col];
            }
            else if (p.Type == PieceType.King)
            {
                return 900 + (isWhite ? KingEvalWhite[row, col] : KingEvalBlack[row, col]);
            }
            throw new Exception("Unknown piece type: " + p.Type);
        };

        int absoluteValue = getAbsoluteValue(piece, piece.Color == PieceColor.White, y, x);
        return piece.Color == PieceColor.White ? absoluteValue : -absoluteValue;

    }
    static float[][] pawnEvalWhite =
{
    new float[] {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f},
    new float[] {5f, 5f, 5f, 5f, 5f, 5f, 5f, 5f},
    new float[] {1f, 1f, 2f, 3f, 3f, 2f, 1f, 1f},
    new float[] {0.5f, 0.5f, 1f, 2.5f, 2.5f, 1f, 0.5f, 0.5f},
    new float[] {0f, 0f, 0f, 2f, 2f, 0f, 0f, 0f},
    new float[] {0.5f, -0.5f, -1f, 0f, 0f, -1f, -0.5f, 0.5f},
    new float[] {0.5f, 1f, 1f, -2f, -2f, 1f, 1f, 0.5f},
    new float[] {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}
};
    public float[][] pawnEvalBlack = reverseArray(pawnEvalWhite);

    public float[][] knightEval = new float[][]
        {
        new float[]{-5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f},
        new float[]{-4.0f, -2.0f, 0.0f, 0.0f, 0.0f, 0.0f, -2.0f, -4.0f},
        new float[]{-3.0f, 0.0f, 1.0f, 1.5f, 1.5f, 1.0f, 0.0f, -3.0f},
        new float[]{-3.0f, 0.5f, 1.5f, 2.0f, 2.0f, 1.5f, 0.5f, -3.0f},
        new float[]{-3.0f, 0.0f, 1.5f, 2.0f, 2.0f, 1.5f, 0.0f, -3.0f},
       new float[] {-3.0f, 0.5f, 1.0f, 1.5f, 1.5f, 1.0f, 0.5f, -3.0f},
        new float[]{-4.0f, -2.0f, 0.0f, 0.5f, 0.5f, 0.0f, -2.0f, -4.0f},
        new float[]{-5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f}
    };
    static float[][] bishopEvalWhite = new float[][] {
        new float[]{ -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f },
       new float[] { -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f },
        new float[]{ -1.0f, 0.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.0f, -1.0f },
        new float[]{ -1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 0.5f, 0.5f, -1.0f },
        new float[]{ -1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, -1.0f },
        new float[]{ -1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, -1.0f },
       new float[] { -1.0f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.5f, -1.0f },
        new float[]{ -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f }
    };

    public float[,] bishopEvalBlack = reverseArray(bishopEvalWhite);
    static float[][] rookEvalWhite = new float[][] {
        new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
        new float[] { 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f },
        new float[] { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f },
        new float[] { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f },
        new float[] { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f },
        new float[] { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f },
        new float[] { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f },
        new float[] { 0.0f, 0.0f, 0.0f, 0.5f, 0.5f, 0.0f, 0.0f, 0.0f }
    };
    public float[][] rookEvalBlack = reverseArray(rookEvalWhite);
    public float[][] evalQueen =
       {
        new[] {-2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f},
        new[] {-1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f},
        new[] {-1.0f, 0.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.0f, -1.0f},
        new[] {-0.5f, 0.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.0f, -0.5f},
        new[] {0.0f, 0.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.0f, -0.5f},
        new[] {-1.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.0f, -1.0f},
        new[] {-1.0f, 0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f},
        new[] {-2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f}
    };
    static float[][] kingEvalWhite = new float[][]
       {
        new float[] {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        new float[] {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        new float[] {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        new float[] {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        new float[] {-2.0f, -3.0f, -3.0f, -4.0f, -4.0f, -3.0f, -3.0f, -2.0f},
        new float[] {-1.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -1.0f},
        new float[] { 2.0f,  2.0f,  0.0f,  0.0f,  0.0f,  0.0f,  2.0f,  2.0f},
        new float[] { 2.0f,  3.0f,  1.0f,  0.0f,  0.0f,  1.0f,  3.0f,  2.0f},
       };

    public float[][] kingEvalBlack = reverseArray(kingEvalWhite);
    // Update is called once per frame
    public LegalMoves GetBestMove(Board board)
    {
        LegalMoves bestMove = null;
        int bestScore = int.MinValue;
        int alpha = int.MinValue;
        int beta = int.MaxValue;

        foreach (LegalMoves move in board.GetLegalMoves())
        {
            Board newBoard = board.Clone();
            newBoard.MakeMove(move);

            int score = Minimax(newBoard, maxDepth - 1, alpha, beta, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }

            alpha = Mathf.Max(alpha, bestScore);
            if (beta <= alpha)
            {
                break;
            }
        }

        return bestMove;
    }

    private int Minimax(Board board, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        if (depth == 0 || board.IsCheckmate())
        {
            return Evaluate(board);
        }

        if (maximizingPlayer)
        {
            int bestScore = int.MinValue;

            foreach (LegalMoves move in board.GetLegalMoves())
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(move);

                int score = Minimax(newBoard, depth - 1, alpha, beta, false);

                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, bestScore);
                if (beta <= alpha)
                {
                    break;
                }
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            foreach (LegalMoves move in board.GetLegalMoves())
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(move);

                int score = Minimax(newBoard, depth - 1, alpha, beta, true);

                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, bestScore);
                if (beta <= alpha)
                {
                    break;
                }
            }

            return bestScore;
        }
    }

}
