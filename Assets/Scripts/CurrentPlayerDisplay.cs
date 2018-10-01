using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour
{
	private Text m_PlayerColor;
	private StateManager m_TheStateManager;
	private string[] m_NumberToColors = { "White", "Red" };

	private void Start()
	{
		m_TheStateManager = GameObject.FindObjectOfType<StateManager>();
		m_PlayerColor = GetComponent<Text>();
	}

	private void Update()
	{
		m_PlayerColor.text = m_NumberToColors[m_TheStateManager.m_CurrentPlayerId];
		if (m_TheStateManager.m_CurrentPlayerId == 0)
		{
			m_PlayerColor.color = Color.white;
		}
		else
		{
			m_PlayerColor.color = Color.red;
		}
	}
}
