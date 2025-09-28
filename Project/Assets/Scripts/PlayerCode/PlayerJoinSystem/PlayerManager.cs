using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<PlayerInfo> players = new List<PlayerInfo>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        players.Add(
            new PlayerInfo
            {
                playerIndex = playerInput.playerIndex,
                device = playerInput.devices[0],
                controlScheme = playerInput.currentControlScheme
            }
        );

        foreach (var device in playerInput.devices)
        {
            Debug.Log($"Device paired: {device.displayName} ({device.deviceId})");
        }

        Destroy(playerInput.gameObject);
        Debug.Log("Player registered!");
    }
}