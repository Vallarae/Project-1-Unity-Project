using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerCode.PlayerJoinSystem {
    public class PlayerJoinMenu : MonoBehaviour {
        public GameObject playerJoinPrefab;

        private void Start() {
            GetComponent<PlayerInputManager>().onPlayerJoined += AddPlayer;
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

            PlayerManager.instance.RegisterPlayer(player);
        }
    }
}