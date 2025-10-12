using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public UIPlayerController playerOneUI;
    public UIPlayerController playerTwoUI;

    void Start()
    {
        playerOneUI.playerIndex = 0;
        playerTwoUI.playerIndex = 1;
    }
}