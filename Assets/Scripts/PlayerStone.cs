using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour
{
	private GameTile[] moveQueue;
	private StateManager m_StateManager;
	private PlayerStone m_StoneToBop;
	private Vector3 m_TargetPosition;
	private Vector3 m_Velocity;

	private float m_SmoothTime = 0.25f;
	private float m_SmoothTimeVertical = 0.1f;
	private float m_SmoothDistance = 0.01f;
	private float m_SmoothHeight = 0.5f;

	private int moveQueueIndex;

	private bool m_IsAnimating = false;
	private bool m_ScoreMe = false;

	public StoneStorage m_StoneStorage;
	public GameTile m_StartingTile;
	public GameTile m_CurrentTile { get; protected set; }

	public int m_PlayerId;

	private void Start()
	{
		m_StateManager = GameObject.FindObjectOfType<StateManager>();
		m_TargetPosition = this.transform.position;
	}

	private void Update()
	{
		if (m_IsAnimating == false)
		{
			return;
		}

		if (Vector3.Distance(
			   new Vector3(this.transform.position.x, m_TargetPosition.y, this.transform.position.z),
			   m_TargetPosition) < m_SmoothDistance)
		{
			// Reached the target position
			if (
				(moveQueue == null || moveQueueIndex == (moveQueue.Length))
				&&
				((this.transform.position.y - m_SmoothDistance) > m_TargetPosition.y)
			)
			{
				// Out of moves, need to do is drop down.
				this.transform.position = Vector3.SmoothDamp(
					this.transform.position,
					new Vector3(this.transform.position.x, m_TargetPosition.y, this.transform.position.z),
					ref m_Velocity,
					m_SmoothTimeVertical);

				// Check to see if we need to remove an opponents stone
				if (m_StoneToBop != null)
				{
					m_StoneToBop.ReturnToStorage();
					m_StoneToBop = null;
				}
			}
			else
			{
				AdvanceMoveQueue();
			}
		}
		else if (this.transform.position.y < (m_SmoothHeight - m_SmoothDistance))
		{
			// Rise up before moving sideways.
			this.transform.position = Vector3.SmoothDamp(
				this.transform.position,
				new Vector3(this.transform.position.x, m_SmoothHeight, this.transform.position.z),
				ref m_Velocity,
				m_SmoothTimeVertical);
		}
		else
		{
			// Sideways movement
			this.transform.position = Vector3.SmoothDamp(
				this.transform.position,
				new Vector3(m_TargetPosition.x, m_SmoothHeight, m_TargetPosition.z),
				ref m_Velocity,
				m_SmoothTime);
		}

	}

	private void AdvanceMoveQueue()
	{
		if (moveQueue != null && moveQueueIndex < moveQueue.Length)
		{
			GameTile nextTile = moveQueue[moveQueueIndex];
			if (nextTile == null)
			{
				// We are probably being scored
				SetNewTargetPosition(this.transform.position + Vector3.right * 10f);
			}
			else
			{
				SetNewTargetPosition(nextTile.transform.position);
				moveQueueIndex++;
			}
		}
		else
		{
			// Movement queue is empty
			this.m_IsAnimating = false;
			m_StateManager.m_AnimationsPlaying--;

			// Check to see if we landed on roll again space
			if (m_CurrentTile != null && m_CurrentTile.m_IsRollAgain)
			{
				m_StateManager.RollAgain();
			}
		}
	}

	private void SetNewTargetPosition(Vector3 pos)
	{
		m_TargetPosition = pos;
		m_Velocity = Vector3.zero;
		m_IsAnimating = true;
	}

	private void OnMouseUp()
	{
		MoveMe();
	}

	public void MoveMe()
	{
		// Make sure each player can only move their own stones
		if (m_StateManager.m_CurrentPlayerId != m_PlayerId)
		{
			return;
		}

		// Don't allow for stones to move if no roll has taken place
		if (m_StateManager.m_IsDoneRolling == false)
		{
			return;
		}

		// Don't allow player to roll again once they have moved
		if (m_StateManager.m_IsDoneClicking == true)
		{
			return;
		}

		int spacesToMove = m_StateManager.m_DiceTotal;

		if (spacesToMove == 0)
		{
			return;
		}

		moveQueue = GetTilesAhead(spacesToMove);
		GameTile finalTile = moveQueue[moveQueue.Length - 1];

		if (finalTile == null)
		{
			m_ScoreMe = true;
		}
		else
		{
			if (CanLegallyMoveTo(finalTile) == false)
			{
				finalTile = m_CurrentTile;
				moveQueue = null;
				return;
			}

			// If opponent's stone in on our space remove their stone from the space
			if (finalTile.m_PlayerStone != null)
			{
				m_StoneToBop = finalTile.m_PlayerStone;
				m_StoneToBop.m_CurrentTile.m_PlayerStone = null;
				m_StoneToBop.m_CurrentTile = null;
			}
		}

		this.transform.SetParent(null);

		// Remove our stone from the old tile
		if (m_CurrentTile != null)
		{
			m_CurrentTile.m_PlayerStone = null;
		}

		m_CurrentTile = finalTile;
		if (finalTile.m_IsScoringTile == false)
		{
			finalTile.m_PlayerStone = this;
		}

		moveQueueIndex = 0;

		m_StateManager.m_IsDoneClicking = true;
		this.m_IsAnimating = true;
		m_StateManager.m_AnimationsPlaying++;
	}

	// Determine the list of tiles X tiles ahead of us
	public GameTile[] GetTilesAhead(int spacesToMove)
	{
		if (spacesToMove == 0)
		{
			return null;
		}

		GameTile[] listOfTiles = new GameTile[spacesToMove];
		GameTile finalTile = m_CurrentTile;

		for (int i = 0; i < spacesToMove; i++)
		{
			if (finalTile == null)
			{
				finalTile = m_StartingTile;
			}
			else
			{
				if (finalTile.m_NextTiles == null || finalTile.m_NextTiles.Length == 0)
				{
					// Overshooting the victory
					break;
				}
				else if (finalTile.m_NextTiles.Length > 1)
				{
					// Determine the correct path based on the player ID
					finalTile = finalTile.m_NextTiles[m_PlayerId];
				}
				else
				{
					finalTile = finalTile.m_NextTiles[0];
				}
			}

			listOfTiles[i] = finalTile;
		}

		return listOfTiles;
	}

	public GameTile GetTileAhead()
	{
		return GetTileAhead(m_StateManager.m_DiceTotal);
	}

	// Determine final tile we'd end on if we moved X spaces
	public GameTile GetTileAhead(int spacesToMove)
	{
		GameTile[] tiles = GetTilesAhead(spacesToMove);

		if (tiles == null)
		{
			// We aren't moving at all, return current tile
			return m_CurrentTile;
		}

		return tiles[tiles.Length - 1];
	}

	public bool CanLegallyMoveAhead(int spacesToMove)
	{
		if (m_CurrentTile != null && m_CurrentTile.m_IsScoringTile)
		{
			return false;
		}

		GameTile theTile = GetTileAhead(spacesToMove);

		return CanLegallyMoveTo(theTile);
	}

	private bool CanLegallyMoveTo(GameTile destinationTile)
	{
		if (destinationTile == null)
		{
			// A null tile means we are overshooting the victory roll
			return false;
		}

		// Check to see if the tile is empty
		if (destinationTile.m_PlayerStone == null)
		{
			return true;
		}

		// If it isn't empty check to see if it's own stone that is on it
		if (destinationTile.m_PlayerStone.m_PlayerId == this.m_PlayerId)
		{
			// Can't land on our own stone
			return false;
		}

		// If there is an enemy stone on the space is it on a roll again "safe space"
		if (destinationTile.m_IsRollAgain == true)
		{
			// Can't remove a stone from a "safe tile"
			return false;
		}

		// If we've gotten here, it means we can legally land on the opponent's stone and
		// kick it off the board.
		return true;
	}

	public void ReturnToStorage()
	{
		this.m_IsAnimating = true;
		m_StateManager.m_AnimationsPlaying++;

		moveQueue = null;

		// Save stone's current position
		Vector3 savePosition = this.transform.position;

		m_StoneStorage.AddStoneToStorage(this.gameObject);

		// Set stone's new position to the animation target
		SetNewTargetPosition(this.transform.position);

		// Restore stone's saved position
		this.transform.position = savePosition;
	}
}