using System.Collections.Generic;
using Fusion;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Intro
{
	public class SessionListPanel : MonoBehaviour
	{
		[SerializeField] private Text _header;
		[SerializeField] private NewSessionPanel _newSessionPanel;
		[SerializeField] private GridBuilder _sessionGrid;
		[SerializeField] private SessionListItem _sessionListItemPrefab;
		[SerializeField] private Text _error;

		private PlayMode _playMode;

		public async void Show(PlayMode mode)
		{
			gameObject.SetActive(true);
			_playMode = mode;
			_error.text = "";
			_header.text = $"{mode} Lobby";
			OnSessionListUpdated(new List<SessionInfo>());
			await App.Instance.EnterLobby($"GameMode{mode}", OnSessionListUpdated);
		}

		public void Hide()
		{
			_newSessionPanel.Hide();
			gameObject.SetActive(false);
			App.Instance.Disconnect();
		}

		public void OnSessionListUpdated(List<SessionInfo> sessions)
		{
			_sessionGrid.BeginUpdate();
			if (sessions != null)
			{
				foreach (SessionInfo info in sessions)
				{
					_sessionGrid.AddRow(_sessionListItemPrefab, item => item.Setup(info, selectedSession => App.Instance.JoinSession(selectedSession)));
				}
			}
			else
			{
				Hide();
				_error.text = "Failed to join lobby";
			}
			_sessionGrid.EndUpdate();
		}
		
		public void OnShowNewSessionUI()
		{
			_newSessionPanel.Show(_playMode);
		}
	}
}