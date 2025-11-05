using PlayerCode.BaseCode;
using PlayerCode.PlayerJoinSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//file contained a lot of redundant code
namespace ObjectCode.PlayerJoinObjects {
    public class GameSceneSpawner : MonoBehaviour {
        public GameObject defaultPlayer; //temporary for testing
        public Slider healthBarPlayerOne;
        public Slider healthBarPlayerTwo;
        public Transform spawnPositionOne;
        public Transform spawnPositionTwo;

        private void Start() {
            int index = 0;
            foreach (PlayerInfo player in PlayerManager.instance.players) {
                Transform currentSpawnPosition = spawnPositionOne != null ? spawnPositionOne : spawnPositionTwo;
                spawnPositionOne = null;
                
                GameObject obj = Instantiate(player.selectedCharacter, currentSpawnPosition.position, Quaternion.identity);

                PlayerInput input = obj.GetComponent<PlayerInput>();
                input.SwitchCurrentControlScheme(player.controlScheme, player.device);

                BasePlayerController controller = obj.GetComponent<BasePlayerController>();

                Slider sliderToUse = healthBarPlayerOne != null ? healthBarPlayerOne : healthBarPlayerTwo;
                healthBarPlayerOne = null;

                controller.healthBarUI = sliderToUse;
                controller.playerId = index;
                index++;
            }
        }
    }
}