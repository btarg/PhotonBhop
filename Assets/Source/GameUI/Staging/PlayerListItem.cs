using UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Staging
{
	public class PlayerListItem : GridCell
	{
		[SerializeField] private Text _name;
		[SerializeField] private Image _color;
		[SerializeField] private GameObject _ready;

		public void Setup(Player ply)
		{
			_name.text = ply.Name.Value;
			_color.color = ply.Color;
			_ready.SetActive(ply.Ready);
		}
	}
}