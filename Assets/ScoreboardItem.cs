using UIComponents;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class ScoreboardItem : GridCell
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private RawImage _pfp;
    [SerializeField] private TextMeshProUGUI _ping;

    public void Setup(Player ply)
    {
        _name.text = ply.PlayerName.Value;
        _name.color = ply.Color;

        // Get the current profile pic
        Texture2D profilePic = SteamHelper.GetSteamImageAsTexture(SteamFriends.GetMediumFriendAvatar((CSteamID)SteamUser.GetSteamID()));
        _pfp.texture = profilePic;

    }
}
