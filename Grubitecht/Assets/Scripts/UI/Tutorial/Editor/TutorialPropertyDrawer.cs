///*****************************************************************************
//// File Name : TutorializedObjectEditor.cs
//// Author : Brandon Koederitz
//// Creation Date : March 20, 2025
////
//// Brief Description : Custom editor for TutorializedObject so thet it only shows the necessary references for each 
//// tutorial type.
//*****************************************************************************/
//using UnityEditor;
//using UnityEngine;

//namespace Grubitecht.UI.Tutorial.Editors
//{
//    [CustomPropertyDrawer(typeof(Tutorial))]
//    public class TutorialPropertyDrawer : PropertyDrawer
//    {
//        //private SerializedProperty Type;
//        //private SerializedProperty TutorialText;
//        //private SerializedProperty TutorialPrefab;
//        //private SerializedProperty OverridePosition;
//        //private SerializedProperty TargetPosition;

//        /// <summary>
//        /// Draw the property on GUI.
//        /// </summary>
//        /// <param name="position">The position of the property.</param>
//        /// <param name="property">The property to draw GUI for</param>
//        /// <param name="label">The label of the property.</param>
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginProperty(position, label, property);

//            Tutorial tutorialObj = property.boxedValue as Tutorial;

//            Rect r = new Rect(position.x, position.y, position.width, position.height);
//            EditorGUI.PropertyField(r, property.FindPropertyRelative("targetObject"));
//            r.y += 30;
//            EditorGUI.PropertyField(r, property.FindPropertyRelative("type"));
//            r.y += 30;

//            switch (tutorialObj.Type)
//            {
//                case Tutorial.TutorialType.Text:
//                    EditorGUI.PropertyField(r,property.FindPropertyRelative("tutorialText"));
//                    r.y += 30;
//                    break;
//                case Tutorial.TutorialType.GameObject:
//                    EditorGUI.PropertyField(r,property.FindPropertyRelative("tutorialPrefab"));
//                    r.y += 30;
//                    break;
//                default:
//                    break;
//            }
//            EditorGUI.PropertyField(r,property.FindPropertyRelative("overridePosition"));
//            r.y += 30;
//            if (tutorialObj.OverridePosition)
//            {
//                EditorGUI.PropertyField(r,property.FindPropertyRelative("targetPosition"));
//                r.y += 30;
//            }

//            EditorGUI.EndProperty();

//            base.OnGUI(position, property, label);
//        }

//        /// <summary>
//        /// Changes the height of the property to accurately reflect the amount of things in it.
//        /// </summary>
//        /// <param name="property">The property we're modifying.</param>
//        /// <param name="label">The label of the property</param>
//        /// <returns>The height of the property.</returns>
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            Tutorial tutorialObj = property.boxedValue as Tutorial;
//            int lineCount = tutorialObj.OverridePosition ? 5 : 4;
//            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
//        }
//    }
//}