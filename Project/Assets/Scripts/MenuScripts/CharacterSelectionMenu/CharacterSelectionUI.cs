using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectionUI : MonoBehaviour
{
    public GameObject characterOne;
    public GameObject characterTwo;

    public static int checkedIndex = 0;

    //    int index = PlayerManager.instance.players.FindIndex(player => player.device == playerInput.devices[0]);
    //input.SwitchCurrentControlScheme(player.controlScheme, player.device);

    private void Start()
    {
        PlayerInfo player = PlayerManager.instance.players[checkedIndex];
        Debug.Log(checkedIndex);

        PlayerInput input = GetComponent<PlayerInput>();
        input.SwitchCurrentControlScheme(player.controlScheme, player.device);

        checkedIndex++;
    }

    private void Update()
    {
        foreach (PlayerInfo player in PlayerManager.instance.players)
        {
            if (player.selectedCharacter == null) return;
        }

        checkedIndex = 0;

        SceneManager.LoadScene(3);
    }

    public void SelectCharacterOne()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        int index = PlayerManager.instance.players.FindIndex(player => player.device == playerInput.devices[0]);

        PlayerInfo playerInfo = PlayerManager.instance.players[index];
        playerInfo.selectedCharacter = characterOne;
    }

    public void SelectCharacterTwo()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        int index = PlayerManager.instance.players.FindIndex(player => player.device == playerInput.devices[0]);

        PlayerInfo playerInfo = PlayerManager.instance.players[index];
        playerInfo.selectedCharacter = characterTwo;
    }
}