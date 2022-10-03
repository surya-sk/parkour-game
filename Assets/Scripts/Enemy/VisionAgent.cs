using ParkourGame.Environment;
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
            if(!IsInRange(VisionRange))
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
        private bool IsInRange(float maxRange)
        {
            var _distance = Vector3.Distance(transform.position, m_Target.position);
            return _distance < maxRange;
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

                float _distanceToAgent = Vector3.Distance(t.position, LineOfSightPivot.position);
                RaycastHit[] _hits = Physics.RaycastAll(_ray, _distanceToAgent);
                List<VisionObstacle> _obstacles = new List<VisionObstacle>();
                foreach(RaycastHit rh in _hits)
                {
                    var v = rh.transform.GetComponent<VisionObstacle>();
                    if(v != null)
                    {
                        _obstacles.Add(v);
                    }
                }

                float _vision = VisionRange;
                foreach(VisionObstacle v in _obstacles)
                {
                    if (v.Solid)
                    {
                        return false;
                    }
                    _vision *= v.ActualStrength;
                    if(_vision < v.CutoffPoint)
                    {
                        return false;
                    }
                }

                if(!IsInRange(_vision))
                {
                    return false;
                }

                return true;
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
