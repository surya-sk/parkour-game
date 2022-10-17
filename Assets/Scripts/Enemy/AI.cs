using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParkourGame.Common;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;
using ParkourGame.Player.Controllers;

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
        public float WaitTimeAtPatrolPoint = 2.0f;

        [Header("Player Refs")]
        public Transform DefaultPlayer;
        public Transform CrouchedPlayer;
        public ActivationController ActivationController;

        private Transform m_Player;
        private Animator m_Animator;
        private NavMeshAgent m_NavMeshAgent;
        private int m_CurrentPatrolIndex;
        private bool b_PatrolComplete;
        private bool b_Patrolling;
        private bool b_PatrolPointReached;
        private bool b_Detected;
        private VisionAgent m_VisionAgent;
        private Transform m_LastKnownPosition;
        private Base m_Base;
        private float m_DistanceToPlayer;
        private float m_Magnitude;
        private bool m_IsDead;

        // Start is called before the first frame update
        void Start()
        {
            m_Animator = GetComponent<Animator>();
            if (m_Animator == null)
            {
                Debug.LogError($"Enemy {gameObject.name} animator not found!!");
            }

            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            if (m_NavMeshAgent == null)
            {
                Debug.LogError($"Enemy {gameObject.name} navmeshagent not found!!");
            }

            m_VisionAgent = GetComponent<VisionAgent>();
            m_VisionAgent.OnDetected += OnPlayerDetected;
            m_VisionAgent.OnUndetected += OnLoseDetection;

            m_Player = DefaultPlayer;
            ActivationController.OnCrouched += PlayerCrouched;
            ActivationController.OnUncrouched += PlayerUncrouched;

            m_Base = GetComponent<Base>();
            m_Base.OnDeath += OnDeath;

            if (PatrolPoints == null || PatrolPoints.Count < 1)
            {
                Debug.LogError($"{gameObject.name} - No patrol points found!!");
            }
            else
            {
                //m_PatrolPointReached += PatrolPointReached;
                m_CurrentPatrolIndex = 0;
            }

            if (PatrolOnStart)
            {
                Patrol(m_NavMeshAgent, PatrolPoints);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(m_IsDead)
            {
                return;
            }

            Debug.Log(b_Detected);

            m_Magnitude = m_NavMeshAgent.velocity.magnitude;
            if(m_Magnitude > 0.5)
            {
                m_Animator.SetFloat("Magnitude", 1f);
            } else
            {
                m_Animator.SetFloat("Magnitude", 0f);
            }
            
            if (b_Patrolling)
            {
                if (Vector3.Distance(transform.position, PatrolPoints[m_CurrentPatrolIndex].position) < 1.0 && !b_PatrolPointReached)
                {
                    b_PatrolPointReached = true;
                    StartCoroutine(PatrolPointReached());
                }
            }

            if(b_Detected)
            {
               m_DistanceToPlayer = Vector3.Distance(transform.position, m_Player.position);
            }
        }

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
                m_NavMeshAgent.SetDestination(destination.position);
            }
            catch(Exception ex)
            {
                Debug.LogError($"Could not move {gameObject.name}. {ex.Message}\n{ex.StackTrace}");
            }
        }

        #region Patrol
        /// <summary>
        /// Begins patrol
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="patrolPoints"></param>
        public void Patrol(NavMeshAgent agent, List<Transform> patrolPoints)
        {
            if(!b_Detected && patrolPoints.Count > 0)
            {
                b_Patrolling = true;
                Move(agent, patrolPoints[m_CurrentPatrolIndex]);
            }
        }

        /// <summary>
        /// Either ends the patrol, or restarts it
        /// </summary>
        public IEnumerator PatrolPointReached()
        {
            //m_Animator.Rebind();
            yield return new WaitForSeconds(Random.Range(WaitTimeAtPatrolPoint - 1, WaitTimeAtPatrolPoint + 3));
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
                b_PatrolPointReached = false;
                Patrol(m_NavMeshAgent, PatrolPoints);
            }
            yield return new WaitForSeconds(0);
        }
        #endregion

        /// <summary>
        /// When player is detected, move to player position and if lost, set last known position
        /// </summary>
        public void OnPlayerDetected()
        {
            b_Detected = true;
            ActivationController.SetCombatMode(true);
            StopCoroutine(LookForPlayer());
            b_Patrolling = false;
            Move(m_NavMeshAgent, m_Player);
            if(m_DistanceToPlayer < m_NavMeshAgent.stoppingDistance)
            {
                StartCoroutine(m_Base.Attack());
            }
            m_LastKnownPosition = m_Player;
        }

        /// <summary>
        /// Player is no longer in FOV
        /// </summary>
        public void OnLoseDetection()
        {
            b_Detected = false;
            ActivationController.SetCombatMode(false);
            StartCoroutine(LookForPlayer());
        }

        private void PlayerCrouched()
        {
            m_Player = CrouchedPlayer;
        }

        private void PlayerUncrouched()
        {
            m_Player = DefaultPlayer;
        }

        /// <summary>
        /// Kill enemy and disable nav mesh
        /// </summary>
        private void OnDeath()
        {
            m_Animator.Rebind();
            m_Animator.SetTrigger("Die");
            m_NavMeshAgent.enabled = false;
            m_VisionAgent.enabled = false;
            m_IsDead = true;
        }

        /// <summary>
        /// Go to player's last known position and look around for a bit
        /// </summary>
        /// <returns></returns>
        IEnumerator LookForPlayer()
        {
            if(m_LastKnownPosition != null)
                Move(m_NavMeshAgent, m_LastKnownPosition);
            m_Animator.SetBool("LookAround", true);
            yield return new WaitForSeconds(3);
            Patrol(m_NavMeshAgent, PatrolPoints);
            yield return new WaitForSeconds(0);
        }
    }
}
