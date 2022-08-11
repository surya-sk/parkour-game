using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    public class RequiredFieldAttribute : PropertyAttribute
    {
        public Color color;

        public RequiredFieldAttribute(FieldColor Fieldcolor =  FieldColor.Red)
        {
            switch (Fieldcolor)
            {
                case FieldColor.Red:
                    color = Color.red;
                    break;
                case FieldColor.Green:
                    color = Color.green;
                    break;
                case FieldColor.Blue:
                    color = Color.blue;
                    break;
                case FieldColor.Magenta:
                    color = Color.magenta;
                    break;
                case FieldColor.Cyan:
                    color = Color.cyan;
                    break;
                case FieldColor.Yellow:
                    color = Color.yellow;
                    break;
                case FieldColor.Orange:
                    color = new Color(1, 0.5f, 0);
                    break;
                case FieldColor.Gray:
                    color = Color.gray;
                    break;
                default:
                    color = Color.red;
                    break;
            }
        }

        public RequiredFieldAttribute()
        {
            color = new Color(1,0,0,0.7f);
        }
    }

#if UNITY_EDITOR
    /// <summary>  Required Field Property Drawer from https://twitter.com/Rodrigo_Devora/status/1204031607583264769 Thanks for sharing! </summary>
    [CustomPropertyDrawer(typeof(RequiredFieldAttribute))]
    public class RequiredFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RequiredFieldAttribute rf = attribute as RequiredFieldAttribute;

            if (property.objectReferenceValue == null)
            {
                var oldColor = GUI.color;

                GUI.color = rf.color;
                EditorGUI.PropertyField(position, property, label);
                GUI.color = oldColor;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
#endif
}