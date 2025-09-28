using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinHandler : MonoBehaviour
{
    public void OnPlayerJoin(PlayerInput playerInput)
    {
        PlayerManager.instance.RegisterPlayer(playerInput);

        foreach (var device in playerInput.devices)
        {
            Debug.Log($"Device paired: {device.displayName} ({device.deviceId})");
        }
    }
}