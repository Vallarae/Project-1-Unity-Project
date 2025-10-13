using PlayerCode.BaseCode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSceneSpawner : MonoBehaviour {
    public GameObject defaultPlayer; //temporary for testing

    private void Start() {
        foreach (PlayerInfo player in PlayerManager.instance.players) {
            var obj = Instantiate(player.selectedCharacter, transform.position, Quaternion.identity);

            var input = obj.GetComponent<PlayerInput>();
            input.SwitchCurrentControlScheme(player.controlScheme, player.device);
        }
    }
}