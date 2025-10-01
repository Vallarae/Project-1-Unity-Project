using UnityEngine.InputSystem;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public int playerIndex;
    public int deviceId;
    public InputDevice device;
    public string controlScheme;
    public GameObject selectedCharacter;
}