using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
	public int[] m_DiceValues;

	public Sprite[] m_DiceImageOne;
	public Sprite[] m_DiceImageZero;

	private StateManager m_TheStateManager;

	// Use this for initialization
	void Start ()
	{
		m_DiceValues = new int[4];
		m_TheStateManager = GameObject.FindObjectOfType<StateManager>();
	}
	
	public void RollDice()
	{
		if (m_TheStateManager.m_IsDoneRolling == true)
		{
			// Already rolled this turn
			return;
		}

		m_TheStateManager.m_DiceTotal = 0;

		// Roll four dice
		for (int i = 0; i < m_DiceValues.Length; i++)
		{
			m_DiceValues[i] = Random.Range(0, 2);
			m_TheStateManager.m_DiceTotal += m_DiceValues[i];

			// Update the dice on screen to reflect the dice that were rolled
			if (m_DiceValues[i] == 0)
			{
				this.transform.GetChild(i).GetComponent<Image>().sprite = 
					m_DiceImageZero[Random.Range(0, m_DiceImageZero.Length)];
			}

			else
			{
				this.transform.GetChild(i).GetComponent<Image>().sprite =
					m_DiceImageOne[Random.Range(0, m_DiceImageOne.Length)];
			}
		}

		//m_TheStateManager.m_DiceTotal = 14;
		m_TheStateManager.m_IsDoneRolling = true;
		m_TheStateManager.CheckLegalMoves();
	}
}
