using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private GameObject m_CreditsScreen;
	[SerializeField] private GameObject m_RulesScreen;

	public void Play()
	{
		SceneManager.LoadScene(1);
	}

	public void Rules()
	{
		m_RulesScreen.SetActive(true);
	}

	public void Credits()
	{
		m_CreditsScreen.SetActive(true);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Back()
	{
		m_CreditsScreen.SetActive(false);
		m_RulesScreen.SetActive(false);
	}
}
