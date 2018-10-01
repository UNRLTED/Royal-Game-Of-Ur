using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStorage : MonoBehaviour
{
	public GameObject m_StonePrefab;
	public GameTile m_StartingTile;

	// Use this for initialization
	void Start ()
	{
		// Create one stone for each placeholder location

		for (int i = 0; i < this.transform.childCount; i++)
		{
			// Instantiate a new object of the stone prefab
			GameObject theStone = Instantiate(m_StonePrefab);
			theStone.GetComponent<PlayerStone>().m_StartingTile = this.m_StartingTile;
			theStone.GetComponent<PlayerStone>().m_StoneStorage = this;
			AddStoneToStorage(theStone, this.transform.GetChild(i));
		}
	}
	
	public void AddStoneToStorage (GameObject Stone, Transform Placeholder = null)
	{
		if (Placeholder == null)
		{
			// Find the first empty placeholder
			for (int i = 0; i < this.transform.childCount; i++)
			{
				Transform temp = this.transform.GetChild(i);
				if (temp.childCount == 0)
				{
					// Placeholder is empty
					Placeholder = temp;
					break;
				}
			}

			if (Placeholder == null)
			{
				return;
			}
		}

		// Parent the stone
		Stone.transform.SetParent(Placeholder);

		// Reset the stone's local position to 0, 0, 0
		Stone.transform.localPosition = Vector3.zero;
	}
}
