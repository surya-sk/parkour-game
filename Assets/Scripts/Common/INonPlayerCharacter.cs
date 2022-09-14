using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ParkourGame.Common
{
    /// <summary>
    /// An interface that has common NPC functions
    /// </summary>
    public interface INonPlayerCharacter
    {
        /// <summary>
        /// Moves the agent from origin to destination
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        void Move(NavMeshAgent agent, Transform destination);

        /// <summary>
        /// Moves the agent through the patrol points
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        IEnumerator Patrol(NavMeshAgent agent, List<Transform> patrolPoints);

        /// <summary>
        /// Face the agent in the direction of the target
        /// </summary>
        /// <param name="target"></param>
        void FaceTarget(Transform target);
    }
}
