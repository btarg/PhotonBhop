using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Map represents the network aspects of a game scene. It is itself spawned as part of the scene
/// and delegates game-scene specific logic as well as handles spawning of player avatars
/// </summary>
public class Map : SimulationBehaviour, ISpawned
{
	[SerializeField] private Text _countdownMessage;
	[SerializeField] private Transform[] _spawnPoints;

	private Dictionary<Player, Character> _playerCharacters = new Dictionary<Player, Character>();
	
	public void Spawned()
	{
		Debug.Log("Map spawned");
		// Spawn player avatars
		foreach(Player player in App.Instance.Players)
		{
			SpawnAvatar(player, false);
		}
		// Tell the master that we're done loading
		App.Instance.Session.RPC_FinishedLoading(Runner.LocalPlayer);
		// Show the countdown message
		_countdownMessage.gameObject.SetActive(true);

		App.Instance.Session.Map = this;
	}
	
	public void SpawnAvatar(Player player, bool lateJoiner)
	{
		if (_playerCharacters.ContainsKey(player))
			return;
		if (player.Object.HasStateAuthority) // We have StateAuth over the player if we are the host or if we're the player self in shared mode
		{
			Debug.Log($"Spawning avatar for player {player.Name} with input auth {player.Object.InputAuthority}");
			// Note: This only works if the number of spawnpoints in the map matches the maximum number of players - otherwise there's a risk of spawning multiple players in the same location.
			// For example, with 4 spawnpoints and a 5 player limit, the first player will get index 4 (max-1) and the second will get index 0, and both will then use the first spawn point.
			Transform t = _spawnPoints[((int)player.Object.InputAuthority) % _spawnPoints.Length];
			Character character = Runner.Spawn(player.CharacterPrefab, t.position, t.rotation, player.Object.InputAuthority);
			_playerCharacters[player] = character;
			player.InputEnabled = lateJoiner;
		}
	}

	public void DespawnAvatar(Player ply)
	{
		if (_playerCharacters.TryGetValue(ply, out Character c))
		{
			Runner.Despawn(c.Object);
			_playerCharacters.Remove(ply);
		}
	}

	public override void FixedUpdateNetwork()
	{
		// Update the countdown message
		Session session = App.Instance.Session;
		if (session.Object == null || !session.Object.IsValid)
			return;
		if (session.PostLoadCountDown.Expired(Runner))
			_countdownMessage.gameObject.SetActive(false);
		else if (session.PostLoadCountDown.IsRunning)
			_countdownMessage.text = Mathf.CeilToInt(session.PostLoadCountDown.RemainingTime(Runner)??0 ).ToString();
	}

	/// <summary>
	/// UI hooks
	/// </summary>

	public void OnDisconnect()
	{
		App.Instance.Disconnect();
	}

	public void OnLoadMap1()
	{
		App.Instance.Session.LoadMap(MapIndex.Map1);
	}

	public void OnGameOver()
	{
		App.Instance.Session.LoadMap(MapIndex.GameOver);
	}
}