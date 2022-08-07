using UnityEngine;

namespace GameUI
{
	public class GameOver : MonoBehaviour
	{
		public void OnContinue()
		{
			App.Instance.Session.LoadMap(MapIndex.Staging);
		}
	}
}