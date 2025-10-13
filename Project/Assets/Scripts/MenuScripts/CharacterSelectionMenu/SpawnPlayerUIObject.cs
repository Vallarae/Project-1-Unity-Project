using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SpawnPlayerUIObject : MonoBehaviour
{
    public GameObject playerUIObject;

    private void Start()
    {
        List<PlayerInfo> players = PlayerManager.instance.players;

        foreach (PlayerInfo player in players)
        {
            GameObject UIObject = Instantiate(playerUIObject);

            //PlayerInput input = UIObject.GetComponent<PlayerInput>();
            //input.SwitchCurrentControlScheme(player.controlScheme, player.device);
        }
    }

    //public void OnPlayerJoined(PlayerInput playerInput)
    //{
    //    int index = PlayerManager.instance.players.FindIndex(player => player.device == playerInput.devices[0]);
    //    PlayerInfo player = PlayerManager.instance.players[index];

    //    GameObject UIObject = Instantiate(playerUIObject);
    //    playerInput.SwitchCurrentControlScheme(player.controlScheme, player.device);

    //    PlayerManager.instance.players[index] = player;
    //}
}