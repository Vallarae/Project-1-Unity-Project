using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerJoinHandler : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (PlayerManager.instance.players.Exists(p => p.device == playerInput.devices[0])) return;

        PlayerManager.instance.RegisterPlayer(playerInput);

        foreach (var device in playerInput.devices)
        {
            Debug.Log($"Device paired: {device.displayName} ({device.deviceId})");
        }

        if (PlayerManager.instance.players.Count == 2) SceneManager.LoadScene(1);
    }
}