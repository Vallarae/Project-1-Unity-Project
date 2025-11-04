using PlayerCode.BaseCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCode.BaseCode {
    public class PlayerInputHandler : MonoBehaviour {
        private InputActionMap _inputActions;
        private BasePlayerController _playerController;

        private void Awake() {
            _inputActions = InputSystem.actions.FindActionMap("Player", true);
            _playerController = GetComponent<BasePlayerController>();
        }

        private void Start() {
            BindToEvents();
        }

        private void BindToEvents() {
            // _inputActions["Move"].performed += ctx => _playerController.HandleMoveInput(ctx);
            // _inputActions["Move"].canceled += ctx => _playerController.HandleMoveInputEnd(ctx);

            // _inputActions["Jump"].performed += ctx => _playerController.HandleJumpPressed(ctx);
            // _inputActions["Jump"].canceled += ctx => _playerController.HandleJumpRelease(ctx);

            // _inputActions["Attack"].performed += ctx => _playerController.HandleAttackPressed(ctx);
            // _inputActions["Attack"].canceled += ctx => _playerController.HandleAttackReleased(ctx);

            // _inputActions["Block"].performed += ctx => _playerController.HandleBlockPressed(ctx);
            // _inputActions["Block"].canceled += ctx => _playerController.HandleBlockReleased(ctx);

            // _inputActions["Ability"].performed += ctx => _playerController.HandleAbilityPressed(ctx);
            // _inputActions["Ability"].canceled += ctx => _playerController.HandleAbilityRelease(ctx);

            // _inputActions["Ultimate"].performed += ctx => _playerController.HandleUltimatePressed(ctx);
            // _inputActions["Ultimate"].canceled += ctx => _playerController.HandleUltimateRelease(ctx);
        }
    }
}