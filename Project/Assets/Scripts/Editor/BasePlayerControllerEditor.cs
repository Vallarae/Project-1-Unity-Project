using UnityEngine;
using UnityEditor;
using PlayerCode.BaseCode;
using System;

namespace Editor {
    public class BasePlayerControllerEditor : UnityEditor.Editor {
        #region Variables

        private BasePlayerController _controller;

        //Movement Variables
        private SerializedProperty _walkSpeedField;
        private SerializedProperty _jumpHeightField;
        private SerializedProperty _dashForceField;
        private SerializedProperty _dashCooldownField;

        //Light Attack Variables
        private SerializedProperty _lightAttackDamageField;
        private SerializedProperty _lightAttackStunDurationField;
        private SerializedProperty _lightAttackKnockbackForceField;
        private SerializedProperty _lightAttackCooldownDurationField;
        private SerializedProperty _lightAttackSoundField;

        //Heavy Attack Variables
        private SerializedProperty _heavyAttackDamageField;
        private SerializedProperty _heavyAttackStunDurationField;
        private SerializedProperty _heavyAttackKnockbackForceField;
        private SerializedProperty _heavyAttackCooldownDurationField;
        private SerializedProperty _heavyAttackSoundField;

        //Hitbox Settings Variables
        private SerializedProperty _hitBoxField;
        
        //Block Settings Variables
        private SerializedProperty _blockCooldownField;
        private SerializedProperty _maxBlockHoldTimeField;
        private SerializedProperty _blockAfterAttackCooldownField;
        private SerializedProperty _blockSoundField;
        
        //Health Settings Variables
        private SerializedProperty _maxHealthField;
        private SerializedProperty _takeDamageSoundField;
        
        //Input Variables
        private SerializedProperty _moveInputField;
        private SerializedProperty _jumpButtonDownField;
        private SerializedProperty _dashButtonDownField;
        private SerializedProperty _attackButtonDownField;
        private SerializedProperty _blockButtonDownField;
        private SerializedProperty _abilityButtonDownField;

        private Int32 _tab;

        #endregion

        private void OnEnable() {
            _controller = (BasePlayerController)target;
            _walkSpeedField = serializedObject.FindProperty(nameof(_controller.walkSpeed));
            _jumpHeightField = serializedObject.FindProperty(nameof(_controller.jumpHeight));
            _dashForceField = serializedObject.FindProperty(nameof(_controller.dashForce));
            _dashCooldownField = serializedObject.FindProperty(nameof(_controller.dashCooldown));

            _lightAttackDamageField = serializedObject.FindProperty(nameof(_controller.lightAttackDamage));
            _lightAttackStunDurationField = serializedObject.FindProperty(nameof(_controller.lightAttackStunDuration));
            _lightAttackKnockbackForceField = serializedObject.FindProperty(nameof(_controller.lightAttackKnockbackForce));
            _lightAttackCooldownDurationField = serializedObject.FindProperty(nameof(_controller.lightAttackCooldownDuration));
            _lightAttackSoundField = serializedObject.FindProperty(nameof(_controller.lightAttackSound));
            
            _heavyAttackDamageField = serializedObject.FindProperty(nameof(_controller.lightAttackDamage));
            _heavyAttackStunDurationField = serializedObject.FindProperty(nameof(_controller.lightAttackStunDuration));
            _heavyAttackKnockbackForceField = serializedObject.FindProperty(nameof(_controller.lightAttackKnockbackForce));
            _heavyAttackCooldownDurationField = serializedObject.FindProperty(nameof(_controller.lightAttackCooldownDuration));
            _heavyAttackSoundField = serializedObject.FindProperty(nameof(_controller.lightAttackSound));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            GUI.enabled = true;

            _tab = GUILayout.Toolbar(_tab, new String[] { "Movement", "Light Attack", "Heavy Attack", "Hitbox", "Block", "Health", "Debug"} );
        }
    }
}