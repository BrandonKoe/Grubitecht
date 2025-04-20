///*****************************************************************************
//// File Name : TutorializedObjectEditor.cs
//// Author : Brandon Koederitz
//// Creation Date : March 20, 2025
////
//// Brief Description : Custom editor for TutorializedObject so thet it only shows the necessary references for each 
//// tutorial type.
//*****************************************************************************/
//using UnityEditor;

//namespace Grubitecht.UI.Tutorial.Editors
//{
//    [CustomEditor(typeof(TutorializedObject), true)]
//    public class TutorializedObjectEditor : Editor
//    {
//        private SerializedProperty Type;
//        private SerializedProperty TutorialText;
//        private SerializedProperty TutorialPrefab;
//        private SerializedProperty OverridePosition;
//        private SerializedProperty TargetPosition;

//        // Allows us to access and modify our GroundBrush object from this editor script.
//        public TutorializedObject tutorializedObject { get { return target as TutorializedObject; } }

//        /// <summary>
//        /// Assign serialized properties.
//        /// </summary>
//        private void OnEnable()
//        {
//            Type = serializedObject.FindProperty(nameof(Type));
//            TutorialText = serializedObject.FindProperty(nameof(TutorialText));
//            TutorialPrefab = serializedObject.FindProperty(nameof(TutorialPrefab));
//            OverridePosition = serializedObject.FindProperty(nameof(OverridePosition));
//            TargetPosition = serializedObject.FindProperty(nameof(TargetPosition));
//        }

//        /// <summary>
//        /// Display Properties to the editor.
//        /// </summary>
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            return;
//            EditorGUILayout.PropertyField(Type);

//            switch (tutorializedObject.Type)
//            {
//                case TutorializedObject.TutorialType.Text:
//                    EditorGUILayout.PropertyField(TutorialText);
//                    break;
//                case TutorializedObject.TutorialType.GameObject:
//                    EditorGUILayout.PropertyField(TutorialPrefab);
//                    break;
//                default:
//                    break;
//            }
//            EditorGUILayout.PropertyField(OverridePosition);
//            if (tutorializedObject.OverridePosition)
//            {
//                EditorGUILayout.PropertyField(TargetPosition);
//            }
            
//        }
//    }
//}