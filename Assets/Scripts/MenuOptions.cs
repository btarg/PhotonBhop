using UnityEngine;
using Fusion;

public class MenuOptions : MonoBehaviour
{
    public void Disconnect() {
        FindObjectOfType<Map>().OnDisconnect();
    }
}