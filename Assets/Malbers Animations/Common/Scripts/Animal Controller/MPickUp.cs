using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    public class MPickUp : MonoBehaviour, IAnimatorListener
    {
        [RequiredField, Tooltip("Trigger used to find Items that can be picked Up")]
        public Collider PickUpArea;
        [SerializeField, Tooltip("Allows Picking Items without any Input")]
        private BoolReference m_AutoPick = new BoolReference(false);
        [SerializeField, Tooltip("When an Item is Piked and Hold, the Pick Area will be hidden")]
        private BoolReference m_HidePickArea = new BoolReference(true);
        //public bool AutoPick { get => m_AutoPick.Value; set => m_AutoPick.Value = value; }

        [Tooltip("Bone to Parent the Picked Item")]
        public Transform Holder;
        public Vector3 PosOffset;
        public Vector3 RotOffset;

        public Pickable FocusedItem;
        public Pickable item;

        // [Header("Events")]
        public BoolEvent CanPickUp = new BoolEvent();
        public GameObjectEvent OnItem = new GameObjectEvent();
        public BoolEvent OnFocusedItem = new BoolEvent();
        public IntEvent OnPicking = new IntEvent();
        public IntEvent OnDropping = new IntEvent();

        public float DebugRadius = 0.02f;
        public Color DebugColor = Color.yellow;


        private ICharacterAction character;


        private TriggerProxy AreaTrigger;

        /// <summary>Does the Animal is holding an Item</summary>

        public bool Has_Item => item != null;

        private void Awake()
        {
            character = GetComponent<ICharacterAction>();

            if (PickUpArea)
            {
                if (AreaTrigger == null)
                {
                    AreaTrigger = PickUpArea.GetComponent<TriggerProxy>();
                    if (AreaTrigger == null) AreaTrigger = PickUpArea.gameObject.AddComponent<TriggerProxy>();
                }
            }
            else
            {
                Debug.LogWarning("Please set a Pick up Area");
            }
        }

        private void OnEnable()
        {
            AreaTrigger.OnTrigger_Enter.AddListener(_OnTriggerEnter);
            AreaTrigger.OnTrigger_Exit.AddListener(_OnTriggerExit);

            if (Has_Item) PickUpItem();         //If the animal has an item at start then make all the stuff to pick it up

            CanPickUp.Invoke(Has_Item);
        }

        private void OnDisable()
        {
            AreaTrigger.OnTrigger_Enter.RemoveListener(_OnTriggerEnter);
            AreaTrigger.OnTrigger_Exit.RemoveListener(_OnTriggerExit);
        }

        void _OnTriggerEnter(Collider col)
        {
            var newItem = col.FindComponent<Pickable>();

            if (newItem)
            {
                FocusedItem = newItem;
                FocusedItem.OnFocused.Invoke(true);
                OnFocusedItem.Invoke(FocusedItem);
                CanPickUp.Invoke(true);

                if (FocusedItem.AutoPick)
                    TryPickUp();
            }
        }

        void _OnTriggerExit(Collider col)
        {
            if (FocusedItem != null) //Means there's a New Focused Item
            {
                var newItem = col.GetComponent<Pickable>() ?? col.GetComponentInParent<Pickable>();

                if (newItem && newItem == FocusedItem)
                {
                    FocusedItem.OnFocused.Invoke(false);
                    FocusedItem = null;
                    CanPickUp.Invoke(false);
                }
            }
        }


        public virtual void TryPickUpDrop()
        { 
            if (character != null && character.IsPlayingAction) return; //Do not try if the Character is doing an action

            if (!Has_Item)
            {
                TryPickUp();
            }
            else
            {
                if (item.DropReaction) //means that have Drop Animations
                    TryDropAnimations();
                else
                    DropItem();
            }
        }


        private void TryPickUpAnimations()
        {
            if (character != null && !character.IsPlayingAction) //Try Picking UP WHEN THE CHARACTER IS NOT MAKING ANY ANIMATION
            {
                if (item.Align)
                {
                    StartCoroutine(MTools.AlignLookAtTransform(transform, item.transform, item.AlignTime));
                    StartCoroutine(MTools.AlignTransformRadius(transform, item.transform, item.AlignTime, item.AlignDistance * transform.localScale.y));
                }

                if (!item.PickReaction.TryReact(gameObject)) //Means if the animal does not have a pick up Animation that calls the PickUP method then It will do it manually
                    PickUpItem();
            }
            else
            {
                PickUpItem();
            }
        }

        private void TryDropAnimations()
        {
            if (character != null && !character.IsPlayingAction)
            {
                if (!item.DropReaction.TryReact(gameObject))
                    DropItem();
            }
            else
            {
                DropItem();
            }
        }

        public void ResetPickUp()
        {
            FocusedItem = null;

            if (item) item.IsPicked = false;
            item = null;

            PickUpArea.gameObject.SetActive(true);         //Enable the Pick up Area
            AreaTrigger.ResetTrigger();
        }

        public virtual void TryPickUp()
        {
            if (FocusedItem)
            {
                item = FocusedItem;

                if (item.PickReaction)
                {
                    TryPickUpAnimations();
                }
                else
                {
                    PickUpItem();
                }
            }
        }

        /// <summary>Pick Up Logic. It can be called by the ANimator</summary>
        public void PickUpItem()
        {
            item = FocusedItem;

            if (item)
            {
                item.Picker = gameObject; //Set on the Item who did the Picking
                item.Pick();                                  //Tell the Item that it was picked
                
                OnPicking.Invoke(item.ID);                      //Invoke the Method
                CanPickUp.Invoke(false);
                OnItem.Invoke(item.gameObject);

                if (Holder)
                {
                    item.transform.parent = Holder;                 //Parent it to the Holder
                    item.transform.localPosition = PosOffset;       //Offset the Position
                    item.transform.localEulerAngles = RotOffset;    //Offset the Rotation
                }

               // FocusedItem.OnFocused.Invoke(false);
                FocusedItem = null;                             //Remove the Focused Item
               

                if (m_HidePickArea.Value)
                    PickUpArea.gameObject.SetActive(false);        //Disable the Pick Up Area

               // Debug.Log("PickUpItem");
            }
        }


        public virtual void DropItem()
        {
            if (Has_Item)
            {
                //Debug.Log("DropItem");
              
                item.Drop();                                 //Tell the item is being droped
                OnDropping.Invoke(item.ID);                     //Invoke the method
                OnItem.Invoke(null);
                item = null;                                    //Remove the Item

                if (m_HidePickArea.Value)
                    PickUpArea.gameObject.SetActive(true);         //Enable the Pick up Area

                if (FocusedItem != null && !FocusedItem.AutoPick) AreaTrigger.ResetTrigger();
            }
        }


        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);


        [HideInInspector] public bool ShowEvents = true;
        private void OnDrawGizmos()
        {
            if (Holder)
            {
                Gizmos.color = DebugColor;
                Gizmos.DrawWireSphere(Holder.TransformPoint(PosOffset), 0.02f);
                Gizmos.DrawSphere(Holder.TransformPoint(PosOffset), 0.02f);

            }
        }
    }

    #region INSPECTOR
#if UNITY_EDITOR
    [CustomEditor(typeof(MPickUp)), CanEditMultipleObjects]
    public class MPickUpEditor : Editor
    {
        private MonoScript script;
        private SerializedProperty
            PickUpArea, FocusedItem, AutoPick, Holder, RotOffset, item, m_HidePickArea,
            PosOffset, CanPickUp, /*CanDrop,*/ OnDropping, OnPicking, ShowEvents, DebugRadius, OnItem, DebugColor;

        private void OnEnable()
        {
            script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);

            PickUpArea = serializedObject.FindProperty("PickUpArea");
            m_HidePickArea = serializedObject.FindProperty("m_HidePickArea");

            Holder = serializedObject.FindProperty("Holder");
            PosOffset = serializedObject.FindProperty("PosOffset");
            RotOffset = serializedObject.FindProperty("RotOffset");

            FocusedItem = serializedObject.FindProperty("FocusedItem");
            item = serializedObject.FindProperty("item");

            CanPickUp = serializedObject.FindProperty("CanPickUp");
            //CanDrop = serializedObject.FindProperty("CanDrop");


            OnPicking = serializedObject.FindProperty("OnPicking");
            OnItem = serializedObject.FindProperty("OnItem");
            OnDropping = serializedObject.FindProperty("OnDropping");
            ShowEvents = serializedObject.FindProperty("ShowEvents");
            AutoPick = serializedObject.FindProperty("m_AutoPick");
            DebugColor = serializedObject.FindProperty("DebugColor");
            DebugRadius = serializedObject.FindProperty("DebugRadius");
        }

        public override void OnInspectorGUI()
        {
            MalbersEditor.DrawDescription("Pick Up Logic for Pickable Items");
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                MalbersEditor.DrawScript(script);
                serializedObject.Update();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(PickUpArea);
                EditorGUILayout.PropertyField(AutoPick);
                EditorGUILayout.PropertyField(m_HidePickArea);
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(Holder);
                    if (Holder.objectReferenceValue)
                    {
                        EditorGUILayout.LabelField("Offsets", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(PosOffset, new GUIContent("Position", "Position Local Offset to parent the item to the holder"));
                        EditorGUILayout.PropertyField(RotOffset, new GUIContent("Rotation", "Rotation Local Offset to parent the item to the holder"));
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(item);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(FocusedItem);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndVertical();


                GUIStyle styles = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    ShowEvents.boolValue = EditorGUILayout.Foldout(ShowEvents.boolValue, "Events", styles);
                    EditorGUI.indentLevel--;

                    if (ShowEvents.boolValue)
                    {
                        EditorGUILayout.PropertyField(CanPickUp, new GUIContent("On Can Pick Item"));
                        EditorGUILayout.PropertyField(OnItem, new GUIContent("On Item Picked"));
                        EditorGUILayout.PropertyField(OnPicking);
                        EditorGUILayout.PropertyField(OnDropping);
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(DebugRadius);
                    EditorGUILayout.PropertyField(DebugColor, GUIContent.none, GUILayout.MaxWidth(40));
                }
                EditorGUILayout.EndHorizontal();

                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }
        }
    }
#endif
    #endregion
}