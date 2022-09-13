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
        public List<Transform> PatrolPoints;
        public bool PatrolOnStart;

        private Animator m_Animator;

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
            if(PatrolPoints == null || PatrolPoints.Count < 2)
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
