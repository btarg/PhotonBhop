
using UnityEngine;

namespace Utility
{
	public class LookAtCamera : MonoBehaviour
	{
		private void Update()
		{
			transform.LookAt(Camera.main.transform);
		}
	}
}