using System;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(KnightScript))]
    public class KnightEditor : UnityEditor.Editor{
        #region Variables

        private KnightScript _controller;

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

        //Ability Variables
        private SerializedProperty _abilityHitboxSizeField;
        private SerializedProperty _abilityPositionOffsetField;
        private SerializedProperty _abilityDashForceField;
        private SerializedProperty _animationTimeForceField;
        private SerializedProperty _abilityDurationTimeField;
        private SerializedProperty _maxHitCountField;
        private SerializedProperty _abilityDamageField;
        private SerializedProperty _abilityStunTime;
        private SerializedProperty _abilityKnockbackForceField;
        
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
        private SerializedProperty _canMoveField;

        private Int32 _tab;

        #endregion

        private void OnEnable() {
            _controller = (KnightScript)target;
            _walkSpeedField = serializedObject.FindProperty(nameof(_controller.walkSpeed));
            _jumpHeightField = serializedObject.FindProperty(nameof(_controller.jumpHeight));
            _dashForceField = serializedObject.FindProperty(nameof(_controller.dashForce));
            _dashCooldownField = serializedObject.FindProperty(nameof(_controller.dashCooldown));

            _lightAttackDamageField = serializedObject.FindProperty(nameof(_controller.lightAttackDamage));
            _lightAttackStunDurationField = serializedObject.FindProperty(nameof(_controller.lightAttackStunDuration));
            _lightAttackKnockbackForceField = serializedObject.FindProperty(nameof(_controller.lightAttackKnockbackForce));
            _lightAttackCooldownDurationField = serializedObject.FindProperty(nameof(_controller.lightAttackCooldownDuration));
            _lightAttackSoundField = serializedObject.FindProperty(nameof(_controller.lightAttackSound));
            
            _heavyAttackDamageField = serializedObject.FindProperty(nameof(_controller.heavyAttackDamage));
            _heavyAttackStunDurationField = serializedObject.FindProperty(nameof(_controller.heavyAttackStunDuration));
            _heavyAttackKnockbackForceField = serializedObject.FindProperty(nameof(_controller.heavyAttackKnockbackForce));
            _heavyAttackCooldownDurationField = serializedObject.FindProperty(nameof(_controller.heavyAttackCooldownDuration));
            _heavyAttackSoundField = serializedObject.FindProperty(nameof(_controller.heavyAttackSound));
            
            _abilityHitboxSizeField = serializedObject.FindProperty(nameof(_controller.abilityHitboxSize));
            _abilityPositionOffsetField = serializedObject.FindProperty(nameof(_controller.abilityPositionOffset));
            _abilityDashForceField = serializedObject.FindProperty(nameof(_controller.abilityDashForce));
            _animationTimeForceField = serializedObject.FindProperty(nameof(_controller.animationTime));
            _abilityDurationTimeField = serializedObject.FindProperty(nameof(_controller.abilityDurationTime));
            _maxHitCountField = serializedObject.FindProperty(nameof(_controller.maxHitCount));
            _abilityDamageField = serializedObject.FindProperty(nameof(_controller.abilityDamage));
            _abilityStunTime = serializedObject.FindProperty(nameof(_controller.abilityStunTime));
            _abilityKnockbackForceField = serializedObject.FindProperty(nameof(_controller.abilityKnockbackForce));

            _hitBoxField = serializedObject.FindProperty(nameof(_controller.hitbox));
            
            _blockCooldownField = serializedObject.FindProperty(nameof(_controller.blockCooldown));
            _maxBlockHoldTimeField = serializedObject.FindProperty(nameof(_controller.maxBlockHoldTime));
            _blockAfterAttackCooldownField = serializedObject.FindProperty(nameof(_controller.blockAfterAttackCooldown));
            _blockSoundField = serializedObject.FindProperty(nameof(_controller.blockSound));
            
            _maxHealthField = serializedObject.FindProperty(nameof(_controller.maxHealth));
            _takeDamageSoundField = serializedObject.FindProperty(nameof(_controller.takeDamageSound));

            _showInputValuesField = serializedObject.FindProperty(nameof(_controller.showInputVariables));
            _moveInputField = serializedObject.FindProperty(nameof(_controller._moveInput));
            _jumpButtonDownField = serializedObject.FindProperty(nameof(_controller._jumpButtonDown));
            _dashButtonDownField = serializedObject.FindProperty(nameof(_controller._dashButtonDown));
            _attackButtonDownField = serializedObject.FindProperty(nameof(_controller._attackKeyDown));
            _blockButtonDownField = serializedObject.FindProperty(nameof(_controller._blockKeyDown));
            _abilityButtonDownField = serializedObject.FindProperty(nameof(_controller._abilityKeyDown));

            _canMoveField = serializedObject.FindProperty(nameof(_controller.canMove));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            GUI.enabled = true;

            _tab = GUILayout.Toolbar(_tab, new String[] { "Movement", "Light Attack", "Heavy Attack", "Ability", "Hitbox", "Block", "Health", "Debug"} );

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
                    EditorGUILayout.PropertyField(_abilityDamageField);
                    EditorGUILayout.PropertyField(_abilityStunTime);
                    EditorGUILayout.PropertyField(_abilityKnockbackForceField);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(_abilityHitboxSizeField);
                    EditorGUILayout.PropertyField(_abilityPositionOffsetField);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(_abilityDashForceField);
                    EditorGUILayout.PropertyField(_abilityDurationTimeField);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(_maxHitCountField);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(_animationTimeForceField);
                    break;
                case 4:
                    EditorGUILayout.PropertyField(_hitBoxField);
                    break;
                case 5:
                    EditorGUILayout.PropertyField(_blockCooldownField);
                    EditorGUILayout.PropertyField(_maxBlockHoldTimeField);
                    EditorGUILayout.PropertyField(_blockAfterAttackCooldownField);
                    EditorGUILayout.PropertyField(_blockSoundField);
                    break;
                case 6:
                    EditorGUILayout.PropertyField(_maxHealthField);
                    EditorGUILayout.PropertyField(_takeDamageSoundField);
                    break;
                case 7:
                    EditorGUILayout.PropertyField(_showInputValuesField);
                    if (_showInputValuesField.boolValue) {
                        EditorGUILayout.PropertyField(_moveInputField);
                        EditorGUILayout.PropertyField(_jumpButtonDownField);
                        EditorGUILayout.PropertyField(_dashButtonDownField);
                        EditorGUILayout.PropertyField(_attackButtonDownField);
                        EditorGUILayout.PropertyField(_blockButtonDownField);
                        EditorGUILayout.PropertyField(_abilityButtonDownField);
                        EditorGUILayout.PropertyField(_canMoveField);
                    }

                    break;
            }
            
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}