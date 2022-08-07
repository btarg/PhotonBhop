using System.Collections.Generic;
using Fusion;

/// <summary>
/// Session gets created when a game session starts and exists in only one instance.
/// It survives scene loading and can be used to control game-logic inside a session, across scenes.
/// </summary>

public class Session : NetworkBehaviour
{
	[Networked] public TickTimer PostLoadCountDown { get; set; }
	public SessionProps Props => new SessionProps(Runner.SessionInfo.Properties);
	public SessionInfo Info => Runner.SessionInfo;
	public Map Map { get; set; }

	private HashSet<PlayerRef> _finishedLoading = new HashSet<PlayerRef>();

	public override void Spawned()
	{
		App.Instance.Session = this;
		if (Object.HasStateAuthority && (Runner.CurrentScene == 0 || Runner.CurrentScene == SceneRef.None))
		{
			SessionProps props = new SessionProps(Runner.SessionInfo.Properties);
			if (props.SkipStaging)
				LoadMap(props.StartMap);
			else
				Runner.SetActiveScene((int)MapIndex.Staging);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
	public void RPC_FinishedLoading(PlayerRef playerRef)
	{
		_finishedLoading.Add(playerRef);
		if (_finishedLoading.Count >= App.Instance.Players.Count)
		{
			PostLoadCountDown = TickTimer.CreateFromSeconds(Runner,3);
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (PostLoadCountDown.Expired(Runner))
		{
			PostLoadCountDown = TickTimer.None;
			foreach (Player player in App.Instance.Players)
				player.InputEnabled = true;
		}
	}

	public void LoadMap(MapIndex mapIndex)
	{
		_finishedLoading.Clear();
		foreach (Player player in App.Instance.Players)
			player.InputEnabled = false;
		Runner.SetActiveScene((int)mapIndex);
	}
}