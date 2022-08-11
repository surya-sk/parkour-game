using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>  Prefab Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Variables/Prefab")]
    public class GameObjectVar : ScriptableVar
    {
        [SerializeField] private GameObject value;

        /// <summary>Invoked when the value changes </summary>
        public Events.GameObjectEvent OnValueChanged = new Events.GameObjectEvent();

        /// <summary> Value of the Bool variable</summary>
        public virtual GameObject Value
        {
            get => value;
            set
            {
                if (this.value != value)                  //If the value is diferent change it
                {
                    this.value = value;
                    OnValueChanged.Invoke(value);         //If we are using OnChange event Invoked
                }
            }
        }

        public virtual void SetValue(GameObjectVar var) => Value = var.Value;
        public virtual void SetValue(GameObject var) => Value = var;

    }

    [System.Serializable]
    public class GameObjectReference
    {
        public bool UseConstant = true;

        public GameObject ConstantValue;
        [RequiredField] public GameObjectVar Variable;

        public GameObjectReference() => UseConstant = true;
        public GameObjectReference(GameObject value) => Value = value;

        public GameObjectReference(GameObjectVar value) => Value = value.Value;

        public GameObject Value
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
    }
}
