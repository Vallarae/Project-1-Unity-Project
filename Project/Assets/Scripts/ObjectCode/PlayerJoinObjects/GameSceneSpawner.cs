using PlayerCode.BaseCode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSceneSpawner : MonoBehaviour
{
    public GameObject defaultPlayer; //temporary for testing

    private void Start()
    {
        foreach (var player in PlayerManager.instance.players)
        {
            var obj = Instantiate(defaultPlayer, transform.position, Quaternion.identity);

            BasePlayerController _playerController = obj.GetComponent<BasePlayerController>();
            _playerController.device = player.device;
        }
    }
}