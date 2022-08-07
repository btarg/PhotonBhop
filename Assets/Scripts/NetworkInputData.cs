using UnityEngine;
using Fusion;

enum MyButtons {
    Forward = 0,
    Backward = 1,
    Left = 2,
    Right = 3,
    Jump = 4
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
}
