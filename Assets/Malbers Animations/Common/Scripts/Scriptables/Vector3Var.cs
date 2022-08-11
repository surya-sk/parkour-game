using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary> V3 Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple  </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Variables/Vector3")]
    public class Vector3Var : ScriptableVar
    {
        /// <summary>The current value</summary>
        [SerializeField] private Vector3 value = Vector3.zero;
       
 
        ///// <summary>When active OnValue changed will ve used every time the value changes (you can subscribe only at runtime .?)</summary>
        //public bool UseEvent = true;

        ///// <summary>Invoked when the value changes</summary>
        //public Events.Vector3Event OnValueChanged = new Events.Vector3Event();

        /// <summary> Value of the Float Scriptable variable</summary>
        public virtual Vector3 Value
        {
            get { return value; }
            set
            {
                this.value = value;
                //if (this.value != value)                                //If the value is diferent change it
                //{
                //    this.value = value;
                //    if (UseEvent) OnValueChanged.Invoke(value);         //If we are using OnChange event Invoked
                //}
            }
        }

        public virtual void SetValue(Vector3Var var)
        {
            Value = var.Value;
        }

        public static implicit operator Vector3(Vector3Var reference)
        {
            return reference.Value;
        }

        public static implicit operator Vector2(Vector3Var reference)
        {
            return reference.Value;
        }

    }

    [System.Serializable]
    public class Vector3Reference
    {
        public bool UseConstant = true;

        public Vector3 ConstantValue = Vector3.zero;
        [RequiredField] public Vector3Var Variable;

        public Vector3Reference()
        {
            UseConstant = true;
            ConstantValue = Vector3.zero;
        }

        public Vector3Reference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = Vector3.zero;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<Vector3Var>();
                Variable.Value = Vector3.zero;
            }
        }

        public Vector3Reference(Vector3 value)
        {
            Value = value;
        }

        public Vector3 Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
            set
            {
                if (UseConstant)
                    ConstantValue = value;
                else
                    Variable.Value = value;
            }
        }

        #region Operators
        public static implicit operator Vector3(Vector3Reference reference)
        {
            return reference.Value;
        }

        public static implicit operator Vector2(Vector3Reference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}