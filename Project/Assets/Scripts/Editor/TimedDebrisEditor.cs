//using System;
//using Horde.Gameplay.Debris;
//using UnityEditor;
//using UnityEngine;

//namespace Horde.Editor.Debris
//{
//    [CustomEditor(typeof(TimedDebris))]
//    public class TimedDebrisEditor : UnityEditor.Editor
//    {
//        private TimedDebris _Debris;

//        // Debris Tab Properties

//        private SerializedProperty _DebrisNameField;
//        private SerializedProperty _DebrisCostField;
//        private SerializedProperty _WaitTimeField;

//        // Affectations Tab Properties

//        private SerializedProperty _AffectedObjectsField;
//        private SerializedProperty _SpawnsToEnableField;

//        // Enemies Tab Properties

//        private SerializedProperty _EnemyFloodCheckField;
//        private SerializedProperty _EnemyListField;
//        private SerializedProperty _ScaleMultiplierField;

//        private Int32 _Tab;

//        private void OnEnable()
//        {
//            _Debris = (TimedDebris)target;

//            _DebrisNameField = serializedObject.FindProperty(nameof(_Debris.DebrisName));
//            _DebrisCostField = serializedObject.FindProperty(nameof(_Debris.DebrisCost));
//            _WaitTimeField = serializedObject.FindProperty(nameof(_Debris.WaitTime));

//            _AffectedObjectsField = serializedObject.FindProperty(nameof(_Debris.AffectedObjects));
//            _SpawnsToEnableField = serializedObject.FindProperty(nameof(_Debris.SpawnersToEnable));

//            _EnemyFloodCheckField = serializedObject.FindProperty(nameof(_Debris.DoesStartEnemyFlood));
//            _EnemyListField = serializedObject.FindProperty(nameof(_Debris.EnemyTypes));
//            _ScaleMultiplierField = serializedObject.FindProperty(nameof(_Debris.ScaleMultiplier));
//        }

//        public override void OnInspectorGUI()
//        {
//            serializedObject.Update();

//            GUI.enabled = true;

//            _Tab = GUILayout.Toolbar(_Tab, new String[] { "Debris", "Affectations", "Enemies" });

//            switch (_Tab)
//            {
//                case 0:
//                    EditorGUILayout.PropertyField(_DebrisNameField);
//                    EditorGUILayout.PropertyField(_DebrisCostField);
//                    EditorGUILayout.PropertyField(_WaitTimeField);
//                    break;
//                case 1:
//                    EditorGUILayout.PropertyField(_AffectedObjectsField);
//                    EditorGUILayout.PropertyField(_SpawnsToEnableField);
//                    break;
//                case 2:
//                    EditorGUILayout.PropertyField(_EnemyFloodCheckField);
//                    if (_EnemyFloodCheckField.boolValue) EditorGUILayout.PropertyField(_EnemyListField);
//                    if (_EnemyFloodCheckField.boolValue) EditorGUILayout.PropertyField(_ScaleMultiplierField);
//                    break;
//            }

//            GUI.enabled = true;
//            serializedObject.ApplyModifiedProperties();
//        }
//    }
//}