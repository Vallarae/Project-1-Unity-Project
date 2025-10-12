using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIPlayerController : MonoBehaviour
{
    public int playerIndex; // 0 = PlayerOne UI, 1 = PlayerTwo UI
    public EventSystem eventSystem; // Reference to that player's EventSystem
    public GameObject defaultSelection; // First selected object in UI

    void Start()
    {
        // Get the right device from PlayerManager
        var playerInfo = PlayerManager.instance.players[playerIndex];

        var uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (uiInputModule != null)
        {
            // Restrict to this player's device
            uiInputModule.actionsAsset.devices = new[] { playerInfo.device };
        }

        // Highlight the first button for that player
        eventSystem.firstSelectedGameObject = defaultSelection;
        eventSystem.SetSelectedGameObject(defaultSelection);
    }
}