using Fusion;

[System.Flags]
public enum ButtonFlag
{
	FORWARD = 1 << 0,
	BACKWARD = 1 << 1,
	LEFT = 1 << 2,
	RIGHT = 1 << 3,
	RESPAWN = 1 << 4,
}

public struct InputData : INetworkInput
{
	public ButtonFlag ButtonFlags;

	public bool GetButton(ButtonFlag button)
	{
		return (ButtonFlags & button) == button;
	}
}