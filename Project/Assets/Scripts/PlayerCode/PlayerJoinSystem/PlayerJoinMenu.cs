using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerJoinMenu : MonoBehaviour {
    public GameObject playerJoinPrefab;

    private void Start() {
        GetComponent<PlayerInputManager>().onPlayerJoined += AddPlayer;
    }

    private void Update() {
        /*foreach (var device in InputSystem.devices)
        {
            if (WasJoinedThisFrame(device))
            {
                if (!PlayerManager.instance.players.Exists(p => p.device == device))
                {
                    AddPlayer(device);
                }
            }
        }*/

        if (PlayerManager.instance.players.Count == 2) {
            //SceneManager.LoadScene(1);
        }
    }

    private bool WasJoinedThisFrame(InputDevice device) {
        var gamepad = device as Gamepad;
        if (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame) return true;

        var keyboard = device as Keyboard;
        if (keyboard != null && keyboard.enterKey.wasPressedThisFrame) return true;

        return false;
    }

    private void AddPlayer(PlayerInput player) {
        Debug.Log($"Adding {player.GetDevice<InputDevice>()}");
        /*PlayerInput playerInput = PlayerInput.Instantiate(
            playerJoinPrefab,
            pairWithDevice: device,
            controlScheme: null,
            playerIndex: PlayerManager.instance.players.Count
        );*/

        PlayerManager.instance.RegisterPlayer(player);
        /*Destroy(playerInput.gameObject);*/
    }
}