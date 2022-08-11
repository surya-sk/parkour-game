using UnityEngine;

namespace MalbersAnimations
{
    public abstract class VarListener : MonoBehaviour
    {
        [HideInInspector] public bool ShowEvents = false;
        
        [Tooltip("ID value is used on the AI Brain to know which Var Listener is picked, in case there more than one on one Game Object")]
        public int ID;

        public bool Enable => gameObject.activeInHierarchy && enabled;

        public string Description = "";
        [HideInInspector] public bool editDescription = false;
        [ContextMenu("Edit Description")]
        internal void EditDescription() => editDescription ^= true;
    }


    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(VarListener))]
    public class VarListenerEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty value, Description, Index, ShowEvents, editDescription;

        void OnEnable()    { SetEnable(); }

        protected void SetEnable()
        {
            value = serializedObject.FindProperty("value");
            Description = serializedObject.FindProperty("Description");
            editDescription = serializedObject.FindProperty("editDescription");
            Index = serializedObject.FindProperty("ID");
            ShowEvents = serializedObject.FindProperty("ShowEvents");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (editDescription.boolValue)
                UnityEditor.EditorGUILayout.PropertyField(Description);
            else
             if (!string.IsNullOrEmpty(Description.stringValue)) MalbersEditor.DrawDescription(Description.stringValue);

            UnityEditor.EditorGUILayout.BeginHorizontal(UnityEditor.EditorStyles.helpBox);
            UnityEditor.EditorGUILayout.PropertyField(value, GUILayout.MinWidth(60));
            UnityEditor.EditorGUIUtility.labelWidth = 18;
            UnityEditor.EditorGUILayout.PropertyField(Index, GUILayout.Width(50));
            UnityEditor.EditorGUIUtility.labelWidth = 0;
            ShowEvents.boolValue = GUILayout.Toggle(ShowEvents.boolValue, new GUIContent("", "Show Events"), UnityEditor.EditorStyles.miniButton, GUILayout.Width(15));
            UnityEditor.EditorGUILayout.EndHorizontal();

            if (ShowEvents.boolValue)
            {
                DrawEvents();
            }
            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void DrawEvents()
        {

        }
    }
#endif
}