﻿using UnityEngine;
using System.Collections;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.HAP
{
    /// <summary>This Enable the mounting System</summary> 
    [AddComponentMenu("Malbers/Riding/Mount Trigger")]
    public class MountTriggers : MonoBehaviour
    {

        [Tooltip("If true when the Rider enter the Trigger it will mount automatically")]
        public BoolReference AutoMount = new BoolReference(false);

        [Tooltip("Can be used also for dismounting")]
        public BoolReference Dismount = new BoolReference(true);

        /// <summary>Avoids Automount again after Dismounting and Automount was true</summary>
        public bool WasAutomounted { get; internal set; }

        ///// <summary>The name of the Animation we need to play to Mount the Animal</summary>
        //[Tooltip("The name of the Animation we need to play to Mount the Animal")]
        //public string MountAnimation = "Mount";

        /// <summary>The Transition ID value to dismount this kind of Montura.. (is Located on the Animator)</summary>
        [Tooltip("The Transition ID value to Mount and Dismount the Animal, to Play the correct Mount/Dismount Animation"),UnityEngine.Serialization.FormerlySerializedAs("DismountID")]
        public IntReference MountID;

        [Tooltip("the Transition ID value to dismount this kind of Montura.. (is Located on the Animator)")]
        /// <summary>The Local Direction of the Mount Trigger compared with the animal</summary>
        public Vector3Reference Direction;

        public GameObject PlayerModel;
        public GameObject Rider;
        public GameObject RiderFreeLookCam;

        public TransformAnimation Adjustment;
      // Mountable Montura;
        Mount Montura;
        //Rider rider;
        MRider rider;


        // Use this for initialization
        void Awake()
        {
            Montura = GetComponentInParent<Mount>(); //Get the Mountable in the parents
        }

        IEnumerator OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeInHierarchy ||  other.isTrigger) yield return null; // Do not allow triggers
            
            if(other.gameObject.tag == "Player")
            {
                yield return null;
                PlayerModel.SetActive(false);
                Rider.transform.position = new Vector3(transform.position.x + 0.5f, PlayerModel.transform.position.y, transform.position.z); ;
                Rider.transform.rotation = PlayerModel.transform.rotation;
                RiderFreeLookCam.SetActive(true);
                Rider.SetActive(true);
                
            }
            GetAnimal(other);
        }
        

        private void GetAnimal(Collider other)
        {
            if (!Montura)
            {
                Debug.LogError("No Mount Script Found... please add one");
                return;
            }
            if (!Montura.Mounted && Montura.CanBeMounted)                       //If there's no other Rider on the Animal or the the Animal isn't death
            {
                rider = other.FindComponent<MRider>();

                if (rider != null)
                {
                    if (rider.IsRiding) return;     //Means the Rider is already mounting an animal

                    rider.MountTriggerEnter(Montura,this); //Set Everything Requiered on the Rider in order to Mount

                    if (AutoMount.Value && !WasAutomounted)
                    {
                        rider.MountAnimal();
                    }
                }
            }
        }


        
        void OnTriggerExit(Collider other)
        {
            if (other.isTrigger) return; // Do not allow triggers

            if(other.gameObject.tag != "Animal")
            {
                rider = other.FindComponent<MRider>();
          

                if (rider != null && other.gameObject.tag != "Player")
                {
                    if (rider.IsMountingDismounting) return;                                         //You Cannot Mount if you are already mounted

                    if (rider.MountTrigger == this && !Montura.Mounted)                 //When exiting if we are exiting From the Same Mount Trigger means that there's no mountrigger Nearby
                    {
                        rider.MountTriggerExit();
                    }

                    //rider = null;
                    if (WasAutomounted) WasAutomounted = false;
                    Rider.SetActive(false);
                    RiderFreeLookCam.SetActive(false);
                    PlayerModel.transform.position = new Vector3(transform.position.x + 2, PlayerModel.transform.position.y, transform.position.z);
                    PlayerModel.transform.rotation = this.transform.rotation;
                    PlayerModel.SetActive(true);
                }
            }
        }
    }
}