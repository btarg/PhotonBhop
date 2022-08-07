using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class ErrorBox : MonoBehaviour
	{
		[SerializeField] private Text _status;
		[SerializeField] private Text _message;

		private void Awake()
		{
			gameObject.SetActive(false);
		}

		public void Show(ConnectionStatus stat, string message)
		{
			_status.text = stat.ToString();
			_message.text = message;
			gameObject.SetActive(true);
		}

		public void OnClose()
		{
			gameObject.SetActive(false);
		}
	}
}