using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This is used to track players in the scene
/// </summary>

public class Character : NetworkBehaviour
{
	[SerializeField] private TextMeshPro _name;
	[SerializeField] private MeshRenderer _mesh;

	private Transform _camera;
	private Player _player;

	public override void Spawned()
	{
		_player = App.Instance.GetPlayer(Object.InputAuthority);
		_name.text = _player.Name.Value;
		_mesh.material.color = _player.Color;
	}

}