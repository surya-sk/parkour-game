using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>  Vector2 Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Variables/Vector2")]
    public class Vector2Var : ScriptableVar
    {
        /// <summary>The current value</summary>
        [SerializeField] private Vector2 value = Vector2.zero;


        ///// <summary>When active OnValue changed will ve used every time the value changes (you can subscribe only at runtime .?)</summary>
        //public bool UseEvent = true;

        ///// <summary>Invoked when the value changes</summary>
        //public Events.Vector3Event OnValueChanged = new Events.Vector3Event();

        /// <summary> Value of the Float Scriptable variable</summary>
        public virtual Vector2 Value
        {
            get => value;
            set => this.value = value;

        }

        public virtual void SetValue(Vector2Var var)
        { Value = var.Value; }

        public static implicit operator Vector2(Vector2Var reference)
        { return reference.Value; }
    }

    [System.Serializable]
    public class Vector2Reference
    {
        public bool UseConstant = true;

        public Vector2 ConstantValue = Vector2.zero;
        [RequiredField] public Vector2Var Variable;

        public Vector2Reference()
        {
            UseConstant = true;
            ConstantValue = Vector2.zero;
        }

        public Vector2Reference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = Vector2.zero;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<Vector2Var>();
                Variable.Value = Vector2.zero;
            }
        }

        public Vector2Reference(Vector2 value)
        {
            UseConstant = true;
            Value = value;
        }

        public Vector2Reference(float x, float y)
        {
            UseConstant = true;
            Value = new Vector2(x, y);
        }

        public Vector2 Value
        {
            get => UseConstant ? ConstantValue : Variable.Value;
            set
            {
                if (UseConstant)
                    ConstantValue = value;
                else
                    Variable.Value = value;
            }
        }

        #region Operators
        public static implicit operator Vector2(Vector2Reference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}