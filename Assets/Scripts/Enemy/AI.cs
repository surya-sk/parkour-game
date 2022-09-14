using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParkourGame.Common;
using UnityEngine.AI;
using System;

namespace ParkourGame.Enemy
{
    public delegate void PatrolPointReached();
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
        private int m_CurrentPatrolIndex;
        private bool b_PatrolComplete;
        private bool b_Patrolling;

        event PatrolPointReached m_PatrolPointReached;

        /// <summary>
        /// Face the target, either the next waypoint or the player
        /// </summary>
        /// <param name="target"></param>
        public void FaceTarget(Transform target)
        {
            Vector3 _direction = (target.position - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime);
        }

        /// <summary>
        /// Moves the enenmy to the destination
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="destination"></param>
        public void Move(NavMeshAgent agent, Transform destination)
        {
            FaceTarget(destination);
            try
            {
                m_Animator.SetTrigger("Move");
                m_NavMeshAgent.SetDestination(destination.position);
            }
            catch(Exception ex)
            {
                Debug.LogError($"Could not move {gameObject.name}. {ex.Message}\n{ex.StackTrace}");
            }
        }

        protected virtual void OnPatrolPointReached()
        {
            m_PatrolPointReached?.Invoke();
        }

        public void Patrol(NavMeshAgent agent, List<Transform> patrolPoints)
        {
            b_Patrolling = true;
            Move(agent,patrolPoints[m_CurrentPatrolIndex]);
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
            else
            {
                m_PatrolPointReached += PatrolPointReached;
                m_CurrentPatrolIndex = 0;
            }

            if(PatrolOnStart)
            {
                Patrol(m_NavMeshAgent, PatrolPoints);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(b_Patrolling)
            {
                if(Vector3.Distance(transform.position, PatrolPoints[m_CurrentPatrolIndex].position) < 1.0)
                {
                    OnPatrolPointReached();
                }
            }
        }

        public void PatrolPointReached()
        {
            if (m_CurrentPatrolIndex == PatrolPoints.Count - 1)
            {
                if (LoopPatrolPoints)
                {
                    b_PatrolComplete = false;
                    m_CurrentPatrolIndex = -1;
                }
                else
                {
                    b_PatrolComplete = true;
                    Debug.Log($"{gameObject.name} - Patrol complete");
                }
                b_Patrolling = !b_PatrolComplete;
            }

            m_CurrentPatrolIndex++;
            if(!b_PatrolComplete)
            {
                Patrol(m_NavMeshAgent, PatrolPoints);
            }
        }
    }
}
