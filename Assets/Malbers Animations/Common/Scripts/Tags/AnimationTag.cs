using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations
{
    /// <summary>Animation Tags for Malbers</summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/AnimationTag")]
    public class AnimationTag : IDs
    {
        /// <summary> Re Calculate the ID on enable</summary>
        private void OnEnable()
        {
            ID = Animator.StringToHash(name);
        }
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(AnimationTag))]
    public class AnimTagEd : Editor
    {
        private void OnEnable()
        {
            SerializedProperty ID = serializedObject.FindProperty("ID");
            ID.intValue = target.name.GetHashCode();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Animation Tags ID are calculated using Animator.StringtoHash()", MessageType.None);
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}