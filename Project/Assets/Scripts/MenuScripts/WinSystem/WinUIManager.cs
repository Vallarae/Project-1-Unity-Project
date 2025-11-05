using System.Collections;
using PlayerCode.PlayerJoinSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MenuScripts.WinSystem {
    public class WinUIManager : MonoBehaviour {
        public RawImage icon;
        public Texture2D playerOneWin;
        public Texture2D playerTwoWin;
        public GameObject fadeOut;
        
        private void Start() {
            icon.texture = !PlayerManager.instance.players[0].isLoser ?  playerOneWin : playerTwoWin;
            StartCoroutine(RestartGame());
        }

        private IEnumerator RestartGame() {
            yield return new WaitForSeconds(5);
            Instantiate(fadeOut);
            yield return new WaitForSeconds(5);

            for (int i = 0; i < PlayerManager.instance.players.Count; i++) {
                PlayerInfo playerInfo = PlayerManager.instance.players[i];
                playerInfo.isLoser = false;
                PlayerManager.instance.players[i] = playerInfo;
            }
            
            SceneManager.LoadScene(1);
        }
    }
}
