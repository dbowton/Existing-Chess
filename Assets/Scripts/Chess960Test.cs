using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess960Test : MonoBehaviour
{
	[SerializeField] int runCount = 1;

	string[] boardState = new string[8];


	private void Start()
	{
		bool cleared = true;

		for (int i = 0; i < runCount; i++)
		{
			boardState = Game.GenerateBoard(false);

			cleared &= CheckPieces();
			cleared &= CheckKingPos();
			cleared &= CheckFirstRook();
			cleared &= CheckSecondRook();
			cleared &= CheckBishops();

			if (!cleared)
			{
				print(string.Join(" ", boardState));
				break;
			}
		}

		if(cleared) print("All " + runCount + " runs passed");

		UnityEditor.EditorApplication.isPlaying = false;
	}

	private bool CheckPieces()
	{
		bool status = true;
		status &= hasPiece("king");
		status &= hasPiece("queen");

		status &= hasPieces("rook");
		status &= hasPieces("bishop");
		status &= hasPieces("knight");

		return true;
	}

	private bool hasPiece(string pieceName)
	{
		for(int i = 0; i < boardState.Length; i++) 
		{
			if (boardState[i] == pieceName) return true;
		}

		print("hasPiece (" + pieceName + ") Failed");
		return false;
	}

	private bool hasPieces(string pieceName) 
	{
		bool foundFirst = false;

		for (int i = 0; i < boardState.Length; i++)
		{
			if (boardState[i].Equals(pieceName))
			{
				if (foundFirst) return true;
				foundFirst = true;
			}
		}

		print("hasPieces (" + pieceName + ") Failed");
		return false;
	}

	private bool CheckKingPos()
	{
		bool status = !(boardState[0].Equals("king") || boardState[7].Equals("king"));
		if (!status) print("CheckKingPos: Failed");
		return status;
	}

	private bool CheckFirstRook()
	{
		for(int i = 0; i < boardState.Length; i++) 
		{
			if (boardState[i].Equals("rook")) return true;

			if (boardState[i].Equals("king"))
			{
				print("CheckFirstRook: Failed - King Before Rook");
				return false;
			}
		}

		print("CheckFirstRook: Failed - No Rook or King Found");
		return false;
	}

	private bool CheckSecondRook()
	{
		bool foundKing = false;
		for(int i = 0; i < boardState.Length; i++) 
		{
			if (boardState[i].Equals("king")) foundKing = true;

			if (foundKing && boardState[i].Equals("rook")) return true;
		}

		print("CheckSecondRook: Failed");
		return false;
	}

	private bool CheckBishops()
	{
		int bishop1pos = -1;
		int bishop2pos = -1;

		for (int i = 0; i < boardState.Length; i++)
		{
			if (boardState[i].Equals("bishop"))
			{
				if (bishop1pos == -1) bishop1pos = i;
				else bishop2pos = i;
			}
		}

		if (bishop1pos == -1 || bishop2pos == -1 || (bishop1pos % 2 == 0 && bishop2pos % 2 == 0) || (bishop1pos % 2 == 1 && bishop2pos % 2 == 1))
		{
			print("CheckBishops: Failed");
			return false;
		}

		return true;
	}
}
