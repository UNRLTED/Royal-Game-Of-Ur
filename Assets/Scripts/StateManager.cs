using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
	public int m_CurrentPlayerId;
	public int m_NumberOfPlayers;
	public int m_DiceTotal;
	public int m_AnimationsPlaying;
	public bool m_IsDoneRolling;
	public bool m_IsDoneClicking;
	public bool m_IsDoneMoving;
	public Text m_NoMovesText;

	private void Start()
	{
		m_AnimationsPlaying = 0;
		m_CurrentPlayerId = 0;
		m_NumberOfPlayers = 2;
		m_IsDoneRolling = false;
		m_IsDoneClicking = false;
		m_IsDoneMoving = false;
	}
	
	private void Update()
	{
		// Check to see if turn is done
		if (m_IsDoneRolling && m_IsDoneClicking && m_AnimationsPlaying == 0)
		{
			StartOfTurn();
			return;
		}

		// Return to main menu
		if (Input.GetKeyDown("escape"))
		{
			SceneManager.LoadScene(0);
		}
	}

	public void StartOfTurn()
	{
		m_IsDoneRolling   = false;
		m_IsDoneClicking  = false;
		m_CurrentPlayerId = (m_CurrentPlayerId + 1) % m_NumberOfPlayers;
	}

	public void CheckLegalMoves()
	{
		// If we rolled zero, then no moves are possible
		if (m_DiceTotal == 0)
		{
			StartCoroutine(NoLegalMovesWaiting());
			return;
		}

		// Loop through all of a given player's stones
		PlayerStone[] PlayerStones = GameObject.FindObjectsOfType<PlayerStone>();
		bool hasLegalMove = false;
		foreach (PlayerStone Stone in PlayerStones)
		{
			if (Stone.m_PlayerId == m_CurrentPlayerId)
			{
				if (Stone.CanLegallyMoveAhead(m_DiceTotal))
				{
					hasLegalMove = true;
				}
			}
		}

		// If no moves are legal, wait a second then pass turn
		if (hasLegalMove == false)
		{
			StartCoroutine(NoLegalMovesWaiting());
			return;
		}
	}

	private IEnumerator NoLegalMovesWaiting()
	{
		// Display message
		m_NoMovesText.gameObject.SetActive(true);
		// Wait 1 second
		yield return new WaitForSeconds(2f);
		m_NoMovesText.gameObject.SetActive(false);
		StartOfTurn();
	}

	public void RollAgain()
	{
		m_IsDoneRolling = false;
		m_IsDoneClicking = false;
	}
}
