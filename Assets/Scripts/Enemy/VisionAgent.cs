using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Enemy
{
    public class VisionAgent : MonoBehaviour
    {
        public float VisionRange = 5f;
        public float VisionAngle = 30f;
        public Transform Target;

        private AI AI;

        private void Awake()
        {
            AI = GetComponent<AI>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!IsInRange()) { return; }
            if(!CheckAngle()) { return; }   


        }

        /// <summary>
        /// Returns true if the player is in range
        /// </summary>
        /// <returns></returns>
        private bool IsInRange()
        {
            var _distance = Vector3.Distance(transform.position, Target.position);
            return _distance < VisionRange;
        }

        /// <summary>
        /// Returns true if the player falls under the vision angle
        /// </summary>
        /// <returns></returns>
        private bool CheckAngle()
        {
            var _targetDirection = Target.position - transform.position;
            float _angle = Vector3.Angle(_targetDirection, transform.forward);
            return _angle < VisionAngle;
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
