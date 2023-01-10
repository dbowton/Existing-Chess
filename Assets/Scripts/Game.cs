using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //Reference from Unity IDE
    public GameObject chesspiece;

    //Matrices needed, positions of each of the GameObjects
    //Also separate arrays for the players in order to easily keep track of them all
    //Keep in mind that the same objects are going to be in "positions" and "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    //current turn
    private string currentPlayer = "white";

    //Game Ending
    private bool gameOver = false;

    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void StartChess(bool regular = true)
    {
        if (started) return;
        started = true;

        playerWhite = new GameObject[] { null, null, null, null, null, null, null, null,
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1) };
        playerBlack = new GameObject[] { null, null, null, null, null, null, null, null,
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6) };

        string[] arrPieces = GenerateBoard(regular);

        for (int i = 0; i < 8; i++)
        {
            playerWhite[i] = Create("white_" + arrPieces[i], i, 0);
            playerBlack[(regular ? 7 - i : i)] = Create("black_" + arrPieces[i], (regular ? 7 - i : i), 7);
        }

        //Set all piece positions on the positions board
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public static string[] GenerateBoard(bool regularChess = true)
    {
		string[] arrPieces = new string[] { "rook", "knight", "bishop", "queen", "king", "bishop", "knight", "rook" };

		if (regularChess == false)
		{
			arrPieces = new string[8];

			int kingPos = Random.Range(1, arrPieces.Length - 1);

			arrPieces[kingPos] = "king";
			arrPieces[GenerateAvailablePos(arrPieces, 0, kingPos)] = "rook";
			arrPieces[GenerateAvailablePos(arrPieces, kingPos, arrPieces.Length)] = "rook";

			int bishop1pos = GenerateAvailablePos(arrPieces);
			arrPieces[bishop1pos] = "bishop";

			int bishop2pos = GenerateAvailablePos(arrPieces);

			while ((bishop1pos % 2 == 0 && bishop2pos % 2 == 0) || (bishop1pos % 2 == 1 && bishop2pos % 2 == 1)) bishop2pos = GenerateAvailablePos(arrPieces);

			arrPieces[bishop2pos] = "bishop";

			arrPieces[GenerateAvailablePos(arrPieces)] = "queen";
			arrPieces[GenerateAvailablePos(arrPieces)] = "knight";
			arrPieces[GenerateAvailablePos(arrPieces)] = "knight";
		}

        return arrPieces;
	}

    public static int GenerateAvailablePos(string[] strings, int min = -1, int max = -1)
    {
        if (min == -1) min = 0;
        if (max == -1) max = strings.Length;

        while (true)
        {
            int ranNum = Random.Range(min, max);
            if (strings[ranNum] == null || string.IsNullOrEmpty(strings[ranNum])) return ranNum;
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); //We have access to the GameObject, we need the script
        cm.name = name; //This is a built in variable that Unity has, so we did not have to declare it before
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); //It has everything set up so it can now Activate()
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        //Overwrites either empty space or whatever was there
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
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    bool started = false;

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            //Using UnityEngine.SceneManagement is needed here
            SceneManager.LoadScene("Game"); //Restarts the game by loading the scene over again
        }
    }
    
    public void Winner(string playerWinner)
    {
        gameOver = true;

        //Using UnityEngine.UI is needed here
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}
