using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour
{
	private StateManager m_TheStateManager;

	private void Start ()
	{
		m_TheStateManager = GameObject.FindObjectOfType<StateManager>();	
	}
	
	private void Update ()
	{
		if (m_TheStateManager.m_IsDoneRolling == false)
		{
			GetComponent<Text>().text = "= ?";
		}
		else
		{
			GetComponent<Text>().text = "= " + m_TheStateManager.m_DiceTotal;
		}
	}
}
