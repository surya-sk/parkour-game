using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>  Bool Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Variables/Bool")]
    public class BoolVar : ScriptableVar
    {
        [SerializeField] private bool value;

        /// <summary>Invoked when the value changes </summary>
        public Action<bool> OnValueChanged;

        /// <summary> Value of the Bool variable</summary>
        public virtual bool Value
        {
            get => value; 
            set
            {
                if (this.value != value)                  //If the value is diferent change it
                {
                    this.value = value;
                    OnValueChanged?.Invoke(value);         //If we are using OnChange event Invoked
                }
            }
        }

        public virtual void SetValue(BoolVar var) => SetValue(var.Value);

        public virtual void SetValue(bool var) => Value = var;
        public virtual void Toggle() => Value ^= true;
        public virtual void UpdateValue() => OnValueChanged?.Invoke(value);

        public static implicit operator bool(BoolVar reference) => reference.Value;
    }

    [System.Serializable]
    public class BoolReference
    {
        public bool UseConstant = true;

        public bool ConstantValue;
#pragma warning disable CA2235 // Mark all non-serializable fields
        [RequiredField] public BoolVar Variable;
#pragma warning restore CA2235 // Mark all non-serializable fields

        public BoolReference() => Value = false;

        public BoolReference(bool value) => Value = value;

        public BoolReference(BoolVar value) => Value = value.Value;

        public bool Value
        {
            get => UseConstant || Variable == null ? ConstantValue : Variable.Value;
            set
            {
               // Debug.Log(value);
                if (UseConstant || Variable == null)
                    ConstantValue = value;
                else
                    Variable.Value = value;
            }
        }

        #region Operators
        public static implicit operator bool(BoolReference reference) => reference.Value;
        #endregion
    }
}
