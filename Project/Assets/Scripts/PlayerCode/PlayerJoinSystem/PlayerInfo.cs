using UnityEngine.InputSystem;
using UnityEngine;

namespace PlayerCode.PlayerJoinSystem {
    [System.Serializable]
    public class PlayerInfo {
        public int playerIndex;
        public int deviceId;
        public bool isLoser = false;
        public InputDevice device;
        public string controlScheme;
        public GameObject selectedCharacter;
    }
}