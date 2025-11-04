using UnityEngine;
using UnityEditor;
using PlayerCode.BaseCode;
using System;

namespace Editor {
    [CustomEditor(typeof(BasePlayerController))]
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
        private SerializedProperty _showInputValuesField;
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

            _hitBoxField = serializedObject.FindProperty(nameof(_controller.hitbox));
            
            _blockCooldownField = serializedObject.FindProperty(nameof(_controller.blockCooldown));
            _maxBlockHoldTimeField = serializedObject.FindProperty(nameof(_controller.maxBlockHoldTime));
            _blockAfterAttackCooldownField = serializedObject.FindProperty(nameof(_controller.blockAfterAttackCooldown));
            _blockSoundField = serializedObject.FindProperty(nameof(_controller.blockSound));
            
            _maxHealthField = serializedObject.FindProperty(nameof(_controller.maxHealth));
            _takeDamageSoundField = serializedObject.FindProperty(nameof(_controller.takeDamageSound));

            _showInputValuesField = serializedObject.FindProperty(nameof(_controller.showInputVariables));
            _moveInputField = serializedObject.FindProperty(nameof(_controller.moveInput));
            _jumpButtonDownField = serializedObject.FindProperty(nameof(_controller.jumpButtonDown));
            _dashButtonDownField = serializedObject.FindProperty(nameof(_controller.dashButtonDown));
            _attackButtonDownField = serializedObject.FindProperty(nameof(_controller.attackKeyDown));
            _blockButtonDownField = serializedObject.FindProperty(nameof(_controller.blockKeyDown));
            _abilityButtonDownField = serializedObject.FindProperty(nameof(_controller.abilityKeyDown));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            GUI.enabled = true;

            _tab = GUILayout.Toolbar(_tab, new String[] { "Movement", "Light Attack", "Heavy Attack", "Hitbox", "Block", "Health", "Debug"} );

            switch (_tab) {
                case 0:
                    EditorGUILayout.PropertyField(_walkSpeedField);
                    EditorGUILayout.PropertyField(_jumpHeightField);
                    EditorGUILayout.PropertyField(_dashForceField);
                    EditorGUILayout.PropertyField(_dashCooldownField);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(_lightAttackDamageField);
                    EditorGUILayout.PropertyField(_lightAttackStunDurationField);
                    EditorGUILayout.PropertyField(_lightAttackKnockbackForceField);
                    EditorGUILayout.PropertyField(_lightAttackCooldownDurationField);
                    EditorGUILayout.PropertyField(_lightAttackSoundField);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(_heavyAttackDamageField);
                    EditorGUILayout.PropertyField(_heavyAttackStunDurationField);
                    EditorGUILayout.PropertyField(_heavyAttackKnockbackForceField);
                    EditorGUILayout.PropertyField(_heavyAttackCooldownDurationField);
                    EditorGUILayout.PropertyField(_heavyAttackSoundField);
                    break;
                case 3:
                    EditorGUILayout.PropertyField(_hitBoxField);
                    break;
                case 4:
                    EditorGUILayout.PropertyField(_blockCooldownField);
                    EditorGUILayout.PropertyField(_maxBlockHoldTimeField);
                    EditorGUILayout.PropertyField(_blockAfterAttackCooldownField);
                    EditorGUILayout.PropertyField(_blockSoundField);
                    break;
                case 5:
                    EditorGUILayout.PropertyField(_maxHealthField);
                    EditorGUILayout.PropertyField(_takeDamageSoundField);
                    break;
                case 6:
                    EditorGUILayout.PropertyField(_showInputValuesField);
                    if (_showInputValuesField.boolValue) {
                        EditorGUILayout.PropertyField(_moveInputField);
                        EditorGUILayout.PropertyField(_jumpButtonDownField);
                        EditorGUILayout.PropertyField(_dashButtonDownField);
                        EditorGUILayout.PropertyField(_attackButtonDownField);
                        EditorGUILayout.PropertyField(_blockButtonDownField);
                        EditorGUILayout.PropertyField(_abilityButtonDownField);
                    }

                    break;
            }
            
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}