using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Intro
{
	public class NewSessionPanel : MonoBehaviour
	{
		[SerializeField] private InputField _inputName;
		[SerializeField] private Text _textMaxPlayers;
		[SerializeField] private Toggle _toggleMap1;
		[SerializeField] private Toggle _toggleMap2;
		
		private int _maxPly = 4;
		private PlayMode _playMode;

		public void Show(PlayMode mode)
		{
			gameObject.SetActive(true);
			_playMode = mode;
			UpdateUI();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void OnDecreaseMaxPlayers()
		{
			if(_maxPly>2)
				_maxPly--;
			UpdateUI();
		}
		public void OnIncreaseMaxPlayers()
		{
			if(_maxPly<16)
				_maxPly++;
			UpdateUI();
		}

		public void OnEditText()
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			_textMaxPlayers.text = $"Max Players: {_maxPly}";
			if(!_toggleMap1.isOn && !_toggleMap2.isOn)
				_toggleMap1.isOn = true;
			if(string.IsNullOrWhiteSpace(_inputName.text))
				_inputName.text = "Room1";
		}
		
		public void OnCreateSession()
		{
			SessionProps props = new SessionProps();
			props.StartMap = _toggleMap1.isOn ? MapIndex.Map0 : MapIndex.Map1;
			props.PlayMode = _playMode;
			props.PlayerLimit = _maxPly;
			props.RoomName = _inputName.text;
			App.Instance.CreateSession(props);
		}
	}
}