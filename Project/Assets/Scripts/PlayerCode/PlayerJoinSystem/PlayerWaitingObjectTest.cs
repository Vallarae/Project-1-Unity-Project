using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWaitingObjectTest : MonoBehaviour {
    public void OnAttack(InputAction.CallbackContext cc)
    {
        InputDevice device = cc.control.device;

        Debug.Log($"Registered input from {device.displayName}, {device.deviceId}");
    }
}