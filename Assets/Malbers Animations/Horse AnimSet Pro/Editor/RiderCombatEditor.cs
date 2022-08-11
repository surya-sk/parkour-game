using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

namespace MalbersAnimations.HAP
{
    [CustomEditor(typeof(RiderCombat))]
    public class RiderCombatEditor : Editor
    {
        private MonoScript script;
        RiderCombat M;

        private SerializedProperty
            InputWeapon,
            InputAttack1, InputAttack2,
            InputAim, Reload,
            HitMask,
            HBack,
            HLeft, CombatAbilities,
            HRight,
            HolderLeft,
            HolderRight,
            HolderBack,
            debug;

        private void OnEnable()
        {
            M = (RiderCombat)target;

            script = MonoScript.FromMonoBehaviour(M);


            CombatAbilities = serializedObject.FindProperty("CombatAbilities");

            InputWeapon = serializedObject.FindProperty("InputWeapon");
            HitMask = serializedObject.FindProperty("HitMask");
            Reload = serializedObject.FindProperty("Reload");

            InputAttack1 = serializedObject.FindProperty("InputAttack1");
            InputAttack2 = serializedObject.FindProperty("InputAttack2");
            InputAim = serializedObject.FindProperty("InputAim");

            HLeft = serializedObject.FindProperty("HLeft");
            HRight = serializedObject.FindProperty("HRight");
            HBack = serializedObject.FindProperty("HBack");

            HolderLeft = serializedObject.FindProperty("HolderLeft");
            HolderRight = serializedObject.FindProperty("HolderRight");
            HolderBack = serializedObject.FindProperty("HolderBack");

            debug = serializedObject.FindProperty("debug");


        }

        /// <summary>
        /// Draws all of the fields for the selected ability.
        /// </summary>
        private void DrawAbility(RiderCombatAbility ability)
        {
            if (ability == null) return;

            SerializedObject abilitySerializedObject;
            abilitySerializedObject = new SerializedObject(ability);
            abilitySerializedObject.Update();

            EditorGUI.BeginChangeCheck();

            var property = abilitySerializedObject.GetIterator();
            property.NextVisible(true);
            property.NextVisible(true);
            do
            {
                EditorGUILayout.PropertyField(property, true);
            } while (property.NextVisible(false));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(ability, "Ability Changed");
                abilitySerializedObject.ApplyModifiedProperties();
                if (ability != null)
                {
                    MalbersEditor.SetObjectDirty(ability);
                }
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("The Combat Mode is managed here", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(HitMask, new GUIContent("Hit Mask", "What to Hit"));
                    M.Target = (Transform)EditorGUILayout.ObjectField(new GUIContent("Target", "If the Rider has a Target"), M.Target, typeof(Transform), true);
                    M.AimDot = (RectTransform)EditorGUILayout.ObjectField(new GUIContent("Aim Dot","UI for Aiming"), M.AimDot, typeof(RectTransform), true);

                    M.StrafeOnTarget = EditorGUILayout.Toggle(new GUIContent("Strafe on Target", "If is there a Target change the mount Input to Camera Input "),M.StrafeOnTarget);
                }
                EditorGUILayout.EndVertical();

                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    M.Editor_ShowEquipPoints = EditorGUILayout.Foldout(M.Editor_ShowEquipPoints, "Weapon Equip Points");

                    if (M.Editor_ShowEquipPoints)
                    {
                        M.LeftHandEquipPoint = (Transform)EditorGUILayout.ObjectField("Left Hand", M.LeftHandEquipPoint, typeof(Transform), true);
                        M.RightHandEquipPoint = (Transform)EditorGUILayout.ObjectField("Right Hand", M.RightHandEquipPoint, typeof(Transform), true);
                    }

                    Animator rideranimator = M.GetComponent<Animator>();
                    if (rideranimator)
                    {
                        if (!M.LeftHandEquipPoint)
                        {
                            M.LeftHandEquipPoint = rideranimator.GetBoneTransform(HumanBodyBones.LeftHand);
                        }

                        if (!M.RightHandEquipPoint)
                        {
                            M.RightHandEquipPoint = rideranimator.GetBoneTransform(HumanBodyBones.RightHand);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();


                //INPUTS 
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    M.Editor_ShowInputs = EditorGUILayout.Foldout(M.Editor_ShowInputs, "Inputs");

                    if (M.Editor_ShowInputs)
                    {
                       
                        EditorGUILayout.PropertyField(InputAttack1, new GUIContent("Attack1", "Attack Right Side "));
                        EditorGUILayout.PropertyField(InputAttack2, new GUIContent("Attack2", "Attack Left Side "));
                        EditorGUILayout.PropertyField(InputAim, new GUIContent("Aim Mode", "Enable Aim mode for Ranged Weapons"));
                        EditorGUILayout.PropertyField(Reload, new GUIContent("Reload", "To Reload Guns"));
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                M.ActiveHolderSide = (WeaponHolder)EditorGUILayout.EnumPopup(new GUIContent("Active Holder Side", "Holder to draw weapons from, When weapons dont have specific holder"), M.ActiveHolderSide);
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    M.SetActiveHolder(M.ActiveHolderSide);
                }
                EditorGUI.indentLevel++;

                ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
                //Inventory and Holders
                EditorGUILayout.BeginHorizontal();

                M.UseInventory = GUILayout.Toggle(M.UseInventory, new GUIContent("Use Inventory", "Get the Weapons from an Inventory"), EditorStyles.toolbarButton);

                if (M.UseInventory)
                    M.UseHolders = false;
                else M.UseHolders = true;


                M.UseHolders = GUILayout.Toggle(M.UseHolders, new GUIContent("Use Holders", "The Weapons are child of the Holders Transform"), EditorStyles.toolbarButton);

                if (M.UseHolders)
                    M.UseInventory = false;
                else M.UseInventory = true;

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                if (M.UseInventory)
                {

                    EditorGUILayout.BeginVertical(MalbersEditor.StyleGreen);
                    EditorGUILayout.HelpBox("The weapons gameobjects are received by the method 'SetWeaponByInventory(GameObject)'", MessageType.None);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    M.AlreadyInstantiated = EditorGUILayout.ToggleLeft(new GUIContent("Already Instantiated", "The weapon is already instantiated before entering 'GetWeaponByInventory'"), M.AlreadyInstantiated);
                    EditorGUILayout.EndVertical();

                    //if (MyRiderCombat.GetComponent<MInventory>())
                    //{
                    //    MyRiderCombat.GetComponent<MInventory>().enabled = true;
                    //}
                }

                //Holder Stufss
                if (M.UseHolders)
                {
                    EditorGUILayout.BeginVertical(MalbersEditor.StyleGreen);
                    EditorGUILayout.HelpBox("The weapons are child of the Holders", MessageType.None);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {

                        EditorGUI.indentLevel++;

                        M.Editor_ShowHolders = EditorGUILayout.Foldout(M.Editor_ShowHolders, "Holders");
                        if (M.Editor_ShowHolders)
                        {
                            EditorGUILayout.PropertyField(HolderLeft, new GUIContent("Holder Left", "The Tranform that has the weapons on the Left  Side"));
                            EditorGUILayout.PropertyField(HolderRight, new GUIContent("Holder Right", "The Tranform that has the weapons on the Right Side"));
                            EditorGUILayout.PropertyField(HolderBack, new GUIContent("Holder Back", "The Tranform that has the weapons on the Back  Side"));

                            M.Editor_ShowHoldersInput = GUILayout.Toggle(M.Editor_ShowHoldersInput, "Holders Input", EditorStyles.toolbarButton);
                            {
                                if (M.Editor_ShowHoldersInput)
                                {
                                    EditorGUILayout.PropertyField(InputWeapon, new GUIContent("Input Weapon", "Draw/Store the Last Weapon"));
                                    EditorGUILayout.PropertyField(HLeft, new GUIContent("Left", "Input to get Weapons from the Left Holder"));
                                    EditorGUILayout.PropertyField(HRight, new GUIContent("Right", "Input to get Weapons from the Right Holder"));
                                    EditorGUILayout.PropertyField(HBack, new GUIContent("Back", "Input to get Weapons from the Back Holder"));
                                }
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();

                    if (M.GetComponent<MInventory>())
                    {
                        M.GetComponent<MInventory>().enabled = false;
                    }
                }
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(CombatAbilities, new GUIContent("Rider Combat Abilities", ""), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.Editor_ShowAbilities =  EditorGUILayout.Foldout(M.Editor_ShowAbilities, "Abilities Properties");
            EditorGUI.indentLevel--;

            if (M.Editor_ShowAbilities)
            {
                if (M.CombatAbilities != null)
                    foreach (var combatAbility in M.CombatAbilities)
                    {
                        if (combatAbility != null)
                        {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(combatAbility.name, EditorStyles.toolbarButton);
                            //EditorGUILayout.Separator();
                            DrawAbility(combatAbility);
                            EditorGUILayout.EndVertical();
                        }

                    }
            }
            EditorGUILayout.EndVertical();


            EditorGUI.indentLevel++;
            DrawEvents();

            EditorGUILayout.PropertyField(debug, new GUIContent("Debug", ""));

            Animator anim = M.GetComponent<Animator>();

            AnimatorController controller = null;
            if (anim) controller = (AnimatorController)anim.runtimeAnimatorController;

            if (controller)
            {
                List<AnimatorControllerLayer> layers = controller.layers.ToList();

                if (layers.Find(layer => layer.name == "Mounted") == null)
                //if (anim.GetLayerIndex("Mounted") == -1)
                {
                    EditorGUILayout.HelpBox("No Mounted Layer Found, Add it the Mounted Layer using the Rider 3rd Person Script", MessageType.Warning);
                }
                else
                {
                    if (layers.Find(layer => layer.name == "Rider Combat") == null)
                    //if (anim.GetLayerIndex("Rider Combat") == -1)
                    {
                        if (GUILayout.Button(new GUIContent("Add Rider Combat Layers", "Used for adding the parameters and Layer from the Mounted Animator to your custom character controller animator ")))
                        {
                            AddLayerMountedCombat(controller);
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Rider Combat Change");
            }

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
        bool EventHelp = false;

        void DrawEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            M.Editor_ShowEvents = EditorGUILayout.Foldout(M.Editor_ShowEvents, new GUIContent("Events"));
            EventHelp = GUILayout.Toggle(EventHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            if (M.Editor_ShowEvents)
            {
                if (EventHelp)
                {
                EditorGUILayout.HelpBox("On Equip Weapon: Invoked when the rider equip a weapon. \n\nOn Unequip Weapon: Invoked when the rider unequip a weapon.\n\nOn Weapon Action: Gets invoked when a new WeaponAction is set\n(See Weapon Actions enum for more detail). \n\nOn Attack: Invoked when the rider is about to Attack(Melee) or Fire(Range)\n\nOn AimSide: Invoked when the rider is Aiming\n 1:The camera is on the Right Side\n-1 The camera is on the Left Side\n 0:The Aim is Reseted\n\nOn Target: Invoked when the Target is changed", MessageType.None);
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEquipWeapon"  ), new GUIContent("On Equip Weapon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUnequipWeapon"), new GUIContent("On Unequip Weapon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnWeaponAction" ), new GUIContent("On Weapon Action"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAttack"), new GUIContent("On Attack"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAimSide"), new GUIContent("On Aim Side"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTarget"), new GUIContent("On Target"));
            }

            EditorGUILayout.EndVertical();
        }

        void AddLayerMountedCombat(AnimatorController CurrentAnimator)
        {
            AnimatorController MountedLayerFile = Resources.Load<AnimatorController>("Mounted Layer");

            Rider3rdPersonEditor.UpdateParametersOnAnimator(CurrentAnimator);
            UpdateParametersOnAnimator(CurrentAnimator);                                                    //Adding the Parameters Needed

            AnimatorControllerLayer RiderCombatLayers = MountedLayerFile.layers[2];                         //Search For the 2nd Layer to Add
            CurrentAnimator.AddLayer(RiderCombatLayers);                  //Add "Rider Arm Right" Layer

            RiderCombatLayers = MountedLayerFile.layers[3];
            CurrentAnimator.AddLayer(RiderCombatLayers);                  //Add "Rider Arm Left"  Layer


            RiderCombatLayers = MountedLayerFile.layers[4];
            CurrentAnimator.AddLayer(RiderCombatLayers);                  //Add "Rider Combat" Layer

        }


        #region Working Great!

        // Copy all parameters to the new animator
        void UpdateParametersOnAnimator(AnimatorController AnimController)
        {
            AnimatorControllerParameter[] parameters = AnimController.parameters;

            //RIDER COMBAT!!!!!!!!!!

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponAim"))
                AnimController.AddParameter("WeaponAim", UnityEngine.AnimatorControllerParameterType.Float);

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponType"))
                AnimController.AddParameter("WeaponType", UnityEngine.AnimatorControllerParameterType.Int);

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponHolder"))
                AnimController.AddParameter("WeaponHolder", UnityEngine.AnimatorControllerParameterType.Int);

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponAction"))
                AnimController.AddParameter("WeaponAction", UnityEngine.AnimatorControllerParameterType.Int);

        }
        #endregion

    }
}