using ParkourGame.Player.Controllers;
using ParkourGame.Player.Stealth;
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
        public Transform LineOfSightPivot;

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
                Undetected();
                return;
            }
            if (!PlayerInAngle()) 
            {
                Undetected();
                return;
            }
            if(!PlayerInLineOfSight())
            {
                Undetected();
                return;
            }

            b_Detected = true;
            OnDetected?.Invoke();
        }

        private void Undetected()
        {
            if (b_Detected)
            {
                b_Detected = false;
                OnUndetected?.Invoke();
            }
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
        private bool PlayerInAngle()
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

        /// <summary>
        /// Check if there are no obstructions between player and enemy
        /// </summary>
        /// <returns></returns>
        private bool PlayerInLineOfSight()
        {
            VisionPoints _visionPoints = m_Target.GetComponent<VisionPoints>();
            foreach(Transform t in _visionPoints.PointsofVision)
            {
                Vector3 _rayDirection = (t.position - LineOfSightPivot.position).normalized;
                Ray _ray = new Ray(LineOfSightPivot.position, _rayDirection);
                RaycastHit _hit;
                if(Physics.Raycast(_ray, out _hit))
                {
                    if(_hit.transform == m_Target)
                    {
                        return true;
                    }
                }
            }
            return false;
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
