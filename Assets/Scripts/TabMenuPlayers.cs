using System;
using UnityEngine;
using UIComponents;
using TMPro;

public class TabMenuPlayers : MonoBehaviour
{
    [SerializeField] private GridBuilder _playerGrid;
    [SerializeField] private ScoreboardItem _playerListItemPrefab;
    [SerializeField] private TextMeshProUGUI _serverNameText;

    private void Awake() {
        string sessionName = App.Instance.Session.Props.RoomName;

        if (!String.IsNullOrEmpty(sessionName)) {
            _serverNameText.text = sessionName;
        }
        else {
            _serverNameText.text = "Multiplayer";
        }
        updatePlayers();
    }

    public void updatePlayers()
    {
        int count = 0;
        _playerGrid.BeginUpdate();
        foreach (Player ply in App.Instance.Players)
        {
            _playerGrid.AddRow(_playerListItemPrefab, item => item.Setup(ply));
            count++;
        }
        _playerGrid.EndUpdate();
    }

}
