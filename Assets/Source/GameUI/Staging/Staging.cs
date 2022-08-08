using System.Text;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections.Generic;
using Fusion;

namespace GameUI.Staging
{
    public class Staging : SimulationBehaviour
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

		List<string> playerNames;

        private float _sessionRefresh;

        private void Start()
        {
            SteamAPI.Init();
        }
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
			// Null name? set it to steam name
			string name = App.Instance.GetPlayer().PlayerName.ToString();
			if (string.IsNullOrEmpty(App.Instance.GetPlayer().PlayerName.ToString())) {

				App.Instance.GetPlayer().RPC_SetName(SteamFriends.GetPersonaName());
					
			}

            int count = 0;
            int ready = 0;
			playerNames = new List<string>();
            _playerGrid.BeginUpdate();
            foreach (Player ply in App.Instance.Players)
            {
				if (playerNames.Contains(ply.PlayerName.ToString())) {
					ply.RPC_SetName(ply.PlayerName.ToString() + $" ({count})");
				}

				playerNames.Add(ply.PlayerName.ToString());
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

            _startButton.enabled = wait == null;
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

        public void OnColorUpdated()
        {
            Player ply = App.Instance.GetPlayer();
            Color c = new Color(_sliderR.value, _sliderG.value, _sliderB.value);
            _color.color = c;
            ply.RPC_SetColor(c);
        }

        public void OnDisconnect()
        {
            App.Instance.Disconnect();
        }
    }
}
