using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParkourGame.Common;
using UnityEngine.AI;

namespace ParkourGame.Enemy
{
    /// <summary>
    /// Controls AI of enemies
    /// </summary>
    public class AI : MonoBehaviour, INonPlayerCharacter
    {
        [Header("Patrolling")]
        public List<Transform> PatrolPoints;
        public bool PatrolOnStart;
        public bool LoopPatrolPoints;

        private Animator m_Animator;
        private NavMeshAgent m_NavMeshAgent;

        /// <summary>
        /// Face the target, either the next waypoint or the player
        /// </summary>
        /// <param name="target"></param>
        public void FaceTarget(GameObject target)
        {
            Vector3 _direction = (target.transform.position - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime);
        }

        public IEnumerator Move(NavMeshAgent agent, Transform destination)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Patrol(NavMeshAgent agent, List<Transform> patrolPoints)
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {
            m_Animator = GetComponent<Animator>();
            if(m_Animator == null)
            {
                Debug.LogError($"Enemy {gameObject.name} animator not found!!");
            }

            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            if(m_NavMeshAgent == null)
            {
                Debug.LogError($"Enemy {gameObject.name} navmeshagent not found!!");
            }
            
            if(PatrolPoints == null || PatrolPoints.Count < 1)
            {
                Debug.LogError($"{gameObject.name} - No patrol points found!!");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
