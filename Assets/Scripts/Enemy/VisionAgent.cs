using ParkourGame.Player.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Enemy
{
    public class VisionAgent : MonoBehaviour
    {
        public float VisionRange = 5f;
        public float VisionAngle = 30f;
        public Transform Player;
        public Transform CrouchedPlayer;
        public ActivationController ActivationController;
        public Action OnDetected;
        public Action OnUndetected;

        private Transform m_Target;
        private bool b_Detected;

        private void Start()
        {
            m_Target = Player;
            ActivationController.OnCrouched += PlayerCrouched;
            ActivationController.OnUncrouched += PlayerUncrouched;
        }

        // Update is called once per frame
        void Update()
        {
            if(!IsInRange())
            {
                if(b_Detected)
                {
                    b_Detected = false;
                    OnUndetected?.Invoke();
                }
                return;
            }
            if(!CheckAngle()) 
            {
                if(b_Detected)
                {
                    b_Detected = false;
                    OnUndetected?.Invoke();
                }
                return;
            }

            b_Detected = true;
            OnDetected?.Invoke();
        }

        /// <summary>
        /// Returns true if the player is in range
        /// </summary>
        /// <returns></returns>
        private bool IsInRange()
        {
            var _distance = Vector3.Distance(transform.position, m_Target.position);
            return _distance < VisionRange;
        }

        /// <summary>
        /// Returns true if the player falls under the vision angle
        /// </summary>
        /// <returns></returns>
        private bool CheckAngle()
        {
            var _targetDirection = m_Target.position - transform.position;
            float _angle = Vector3.Angle(_targetDirection, transform.forward);
            return _angle < VisionAngle;
        }

        private void PlayerCrouched()
        {
            m_Target = CrouchedPlayer;
        }

        private void PlayerUncrouched()
        {
            m_Target = Player;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, VisionRange);

            Quaternion _rot = Quaternion.AngleAxis(VisionAngle, Vector3.up);
            Vector3 _endPoint = transform.position + (_rot * transform.forward * VisionRange);
            Gizmos.DrawLine(transform.position, _endPoint);

            _rot = Quaternion.AngleAxis(-VisionAngle, Vector3.up);
            _endPoint = transform.position + (_rot * transform.forward * VisionRange);
            Gizmos.DrawLine(transform.position, _endPoint);
        }

    }
}
