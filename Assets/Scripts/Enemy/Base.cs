using ParkourGame.Player.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Enemy
{
    /// <summary>
    /// Base enemy class that other enemies inherit from
    /// </summary>
    public class Base : MonoBehaviour
    {
        public float Health = 100f;
        public float Damage = 10f;
        public float AttackDelay = 2f;
        public ActivationController ActivationController;

        Animator m_Animator;
        bool b_IsAttacking;

        public bool IsDead { get; set; }

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Decreases enemy health by the damage amount, and kills it if health is below 5
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (IsDead) return;
            if(Health < 5.0)
            {
                IsDead = true;
                m_Animator.Rebind();
                m_Animator.SetTrigger("Die");
            }
        }

        /// <summary>
        /// Attack the player, ie., reduce player health
        /// </summary>
        /// <param name="player"></param>
        public IEnumerator Attack(Transform player)
        {
            if(!b_IsAttacking)
            {
                ActivationController.SwitchCrouchState(true);
                ActivationController.ForceCrouch = true;
                b_IsAttacking = true;
                //m_Animator.Rebind();
                yield return new WaitForSeconds(AttackDelay);
                m_Animator.SetTrigger("Attack");
                Debug.Log("Attack!");
                b_IsAttacking=false;
                ActivationController.ForceCrouch = false;
            }
        }

    }
}
