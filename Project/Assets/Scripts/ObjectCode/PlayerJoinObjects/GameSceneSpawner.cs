using PlayerCode.BaseCode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameSceneSpawner : MonoBehaviour {
    public GameObject defaultPlayer; //temporary for testing
    public Slider healthBarPlayerOne;
    public Slider healthBarPlayerTwo;

    private void Start() {
        foreach (PlayerInfo player in PlayerManager.instance.players) {
            var obj = Instantiate(player.selectedCharacter, transform.position, Quaternion.identity);

            var input = obj.GetComponent<PlayerInput>();
            input.SwitchCurrentControlScheme(player.controlScheme, player.device);

            BasePlayerController controller = obj.GetComponent<BasePlayerController>();

            Slider sliderToUse = healthBarPlayerOne != null ? healthBarPlayerOne : healthBarPlayerTwo;
            healthBarPlayerOne = null;

            controller.healthBarUI = sliderToUse;
        }
    }
}
