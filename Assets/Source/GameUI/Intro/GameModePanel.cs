using UnityEngine;

namespace GameUI.Intro
{
	public class GameModePanel : MonoBehaviour
	{
		[SerializeField] private SessionListPanel _sessionsPanel;

		private void Awake()
		{
			_sessionsPanel.Hide();
		}
		public void OnGameModeSelected(int mode)
		{
			PlayMode playMode = (PlayMode) mode;
			_sessionsPanel.Show(playMode);
		}
	}
}