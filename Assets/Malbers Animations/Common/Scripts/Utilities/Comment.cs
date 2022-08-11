 
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>Adding Coments on the Inspector</summary>.
    public class Comment : MonoBehaviour {[Multiline] public string text; }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Comment))]
    public class CommentEditor : UnityEditor.Editor
    {
        private Comment script;
        GUIStyle style;

        private void OnEnable()
        {
            script = target as Comment;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            style = new GUIStyle(UnityEditor.EditorStyles.helpBox)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
            };

            UnityEditor.EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            string text = UnityEditor.EditorGUILayout.TextArea(script.text, style);
            UnityEditor.EditorGUILayout.EndVertical();
            if (text != script.text)
            {
                UnityEditor.Undo.RecordObject(script, "Edit Comments");
                script.text = text;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}