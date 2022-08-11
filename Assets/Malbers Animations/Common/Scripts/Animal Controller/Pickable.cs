using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using MalbersAnimations.Controller.Reactions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    public class Pickable : MonoBehaviour , ICollectable
    {
        public enum CollectType { Collectable, Hold, OneUse } //For different types of collectable items? FOR ANOTHER UPDATE

        public bool Align = true;
        public bool AlignPos = true;
        public float AlignTime = 0.15f;
        public float AlignDistance = 1f;

        /// <summary> The Character has Pick Animations</summary>
        public MReaction PickReaction;
        
        /// <summary> The Character has Drop Animations</summary>
        public MReaction DropReaction;


        public FloatReference m_Value = new FloatReference(1f); //Not done yet
        public BoolReference m_AutoPick = new BoolReference(false); //Not done yet
        public IntReference m_ID = new IntReference();         //Not done yet

        /// <summary>Who Did the Picking </summary>
        public GameObject Picker { get; set; }


        public BoolEvent OnFocused = new BoolEvent();
        public GameObjectEvent OnPicked = new GameObjectEvent();
        public GameObjectEvent OnDropped = new GameObjectEvent();

        private Rigidbody rb;
        [RequiredField] public Collider m_collider;

        /// <summary>Return all Pickable objects on the active scene</summary>
        public static List<Pickable> AllPickables;


        /// <summary>Is this Object being picked </summary>
        public bool IsPicked { get; set; }
        public float Value { get => m_Value.Value; set => m_Value.Value = value; }
        public bool AutoPick { get => m_AutoPick.Value; set => m_AutoPick.Value = value; }
        public int ID { get => m_ID.Value; set => m_ID.Value = value; }


        private void OnEnable()
        {
            if (AllPickables == null) AllPickables = new List<Pickable>();
            AllPickables.Add(this);
        }

        private void OnDisable()
        {
            AllPickables.Remove(this);
            OnFocused.Invoke(false);
        }

        private void Awake() { rb = GetComponent<Rigidbody>(); }

        public virtual void Pick()
        {
            if (rb)
            {
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.isKinematic = true;
            }

            m_collider.enabled = false;
            IsPicked = true;

            OnPicked.Invoke(Picker);
        }

        public virtual void Drop()
        {
            IsPicked = false;

            if (rb)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            m_collider.enabled = true;

            transform.parent = null;

            OnDropped.Invoke(Picker);

            Picker = null; //Reset who did the picking
        }

        [HideInInspector] public bool ShowEvents = true;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, AlignDistance);
        }

        private void Reset()
        {
            m_collider = GetComponent<Collider>();
#if UNITY_EDITOR
            var TryPickUp = MTools.GetInstance<MEvent>("Try Pick Up");

            if (TryPickUp)
            {
                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<Transform>(OnFocused, TryPickUp.Invoke, transform);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(OnFocused, TryPickUp.Invoke);
                UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(OnPicked, TryPickUp.Invoke,false);
            }
#endif
        }
#endif
    }


    //INSPECTOR
#if UNITY_EDITOR
    [CustomEditor(typeof(Pickable)), CanEditMultipleObjects]
    public class PickableEditor : Editor
    {
        private SerializedProperty //   PickAnimations, PickUpMode, PickUpAbility, DropMode, DropAbility,DropAnimations, 
            Align, AlignTime, AlignDistance, AlignPos, PickReaction, DropReacion, m_AutoPick,
            OnFocused, OnPicked, OnDropped, ShowEvents, FloatID, IntID, m_collider, Collectable;

        private Pickable m;

        private void OnEnable()
        {
            m = (Pickable)target;

            PickReaction = serializedObject.FindProperty("PickReaction");
            DropReacion = serializedObject.FindProperty("DropReaction");


            Align = serializedObject.FindProperty("Align");
            AlignTime = serializedObject.FindProperty("AlignTime");
            AlignDistance = serializedObject.FindProperty("AlignDistance");
            OnFocused = serializedObject.FindProperty("OnFocused");
            OnPicked = serializedObject.FindProperty("OnPicked");
            OnDropped = serializedObject.FindProperty("OnDropped");
            ShowEvents = serializedObject.FindProperty("ShowEvents");
            FloatID = serializedObject.FindProperty("m_Value");
            IntID = serializedObject.FindProperty("m_ID");
            m_collider = serializedObject.FindProperty("m_collider");
            AlignPos = serializedObject.FindProperty("AlignPos");
            //Collectable = serializedObject.FindProperty("Collectable");
            m_AutoPick = serializedObject.FindProperty("m_AutoPick");
        }

        public override void OnInspectorGUI()
        {
            MalbersEditor.DrawDescription("Pickable - Collectable Object");
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                serializedObject.Update();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(m_collider);
                EditorGUILayout.PropertyField(m_AutoPick, new GUIContent("Auto Pick", "The Item will be Picked Automatically"));
                //EditorGUILayout.PropertyField(Collectable, new GUIContent("Item Type", "The Type of logic this item will apply when is picked"));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ToggleLeft("Is Picked", m.IsPicked);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUIUtility.labelWidth = 60;
                EditorGUILayout.PropertyField(IntID, new GUIContent("ID", "Int value the Pickable Item can store.. that it can be use for anything"));
                EditorGUILayout.PropertyField(FloatID, new GUIContent("Value", "Float value the Pickable Item can store.. that it can be use for anything"));
                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(PickReaction, new GUIContent("Pick Reaction", "The Reaction will be applied to the Character when it can pick"));
                EditorGUILayout.PropertyField(DropReacion, new GUIContent("Drop Reaction ", "The Reaction will be applied to the Character when it can drop an Item"));
                EditorGUILayout.EndVertical();



                if (PickReaction.objectReferenceValue != null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.PropertyField(Align, new GUIContent("Align On Pick", "Align the Animal to the Item"));

                    if (Align.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(AlignPos, new GUIContent("Align Pos", "Also Align the Position"));
                        EditorGUIUtility.labelWidth = 60;
                        EditorGUILayout.PropertyField(AlignDistance, new GUIContent("Distance", "Distance to move the Animal towards the Item"), GUILayout.MinWidth(50));
                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(AlignTime, new GUIContent("Time", "Time needed to do the Alignment"));
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    ShowEvents.boolValue = EditorGUILayout.Foldout(ShowEvents.boolValue, "Events");
                    EditorGUI.indentLevel--;

                    if (ShowEvents.boolValue)
                    {
                        EditorGUILayout.PropertyField(OnFocused);
                        EditorGUILayout.PropertyField(OnPicked, new GUIContent("On Picked By"));
                        EditorGUILayout.PropertyField(OnDropped, new GUIContent("On Dropped By"));
                    }
                }
                EditorGUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }
        }
    }
#endif
}