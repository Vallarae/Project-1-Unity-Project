using System.Collections.Generic;
using PlayerCode.PlayerJoinSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace MenuScripts.CharacterSelectionMenu {
    public class SpawnPlayerUIObject : MonoBehaviour {
        public GameObject playerUIObject;

        private void Start() {
            List<PlayerInfo> players = PlayerManager.instance.players;

            foreach (PlayerInfo player in players) {
                Instantiate(playerUIObject);
            }
        }
    }
}