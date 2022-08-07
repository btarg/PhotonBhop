using System.Text;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Staging
{
	public class Staging : MonoBehaviour
	{
		[SerializeField] private GridBuilder _playerGrid;
		[SerializeField] private PlayerListItem _playerListItemPrefab;
		[SerializeField] private Slider _sliderR;
		[SerializeField] private Slider _sliderG;
		[SerializeField] private Slider _sliderB;
		[SerializeField] private Image _color;
		[SerializeField] private Button _startButton;
		[SerializeField] private Text _startLabel;
		[SerializeField] private Text _sessionInfo;
		[SerializeField] private GameObject _playerReady;

		private float _sessionRefresh;

		private void Awake()
		{
			App.Instance.GetPlayer()?.RPC_SetIsReady(false);
			_playerReady.SetActive(false);
		}

		private void UpdateSessionInfo()
		{
			Session s = App.Instance.Session;
			StringBuilder sb = new StringBuilder();
			if (s != null)
			{
				sb.AppendLine($"Session Name: {s.Info.Name}");
				sb.AppendLine($"Region: {s.Info.Region}");
				sb.AppendLine($"Game Type: {s.Props.PlayMode}");
				sb.AppendLine($"Map: {s.Props.StartMap}");
			}
			_sessionInfo.text = sb.ToString();
		}

		void Update()
		{
			int count = 0;
			int ready = 0;
			_playerGrid.BeginUpdate();
			foreach (Player ply in App.Instance.Players)
			{
				_playerGrid.AddRow(_playerListItemPrefab, item => item.Setup(ply));
				count++;
				if (ply.Ready)
					ready++;
			}

			string wait = null;
			if (ready < count)
				wait = $"Waiting for {count - ready} of {count} players";
			else if (!App.Instance.IsMaster)
				wait = "Waiting for master to start";

			_startButton.enabled = wait==null;
			_startLabel.text = wait ?? "Start";
	  
			_playerGrid.EndUpdate();

			if (_sessionRefresh <= 0)
			{
				UpdateSessionInfo();
				_sessionRefresh = 2.0f;
			}
			_sessionRefresh -= Time.deltaTime;
		}

		public void OnStart()
		{
			SessionProps props = App.Instance.Session.Props;
			App.Instance.Session.LoadMap(props.StartMap);
		}

		public void OnToggleIsReady()
		{
			Player ply = App.Instance.GetPlayer();
			_playerReady.SetActive(!ply.Ready);
			ply.RPC_SetIsReady(!ply.Ready);
		}

		public void OnNameChanged(string name)
		{
			Player ply = App.Instance.GetPlayer();
			ply.RPC_SetName(name);
		}
	
		public void OnColorUpdated()
		{
			Player ply = App.Instance.GetPlayer();
			Color c = new Color(_sliderR.value, _sliderG.value, _sliderB.value);
			_color.color = c;
			ply.RPC_SetColor( c);
		}

		public void OnDisconnect()
		{
			App.Instance.Disconnect();
		}
	}
}
