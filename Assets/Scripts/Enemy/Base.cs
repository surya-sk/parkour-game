using ParkourGame.Player.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public float StunTime = 2f;
        public ActivationController ActivationController;

        public Action OnDeath;
        [HideInInspector]
        public bool IsAttacking { get => b_IsAttacking; }

        Animator m_Animator;
        bool b_IsAttacking;
        bool b_CanAttack = true;

        public bool IsDead { get; set; }

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Decreases enemy health by the damage amount, and kills it if health is below 5
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage, bool assassination = false)
        {
            Health -= damage;
            StartCoroutine(Stun(1));
            if (IsDead) return;
            if(Health < 5.0 || assassination)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

        /// <summary>
        /// Attack the player, ie., reduce player health
        /// </summary>
        /// <param name="player"></param>
        public IEnumerator Attack()
        {
            if(!b_IsAttacking && b_CanAttack)
            {
                ActivationController.SwitchCrouchState(true);
                ActivationController.ForceCrouch = true;
                b_IsAttacking = true;
                yield return new WaitForSeconds(AttackDelay);
                m_Animator.SetTrigger("Attack");
                b_IsAttacking = false;
                ActivationController.ForceCrouch = false;
            }
        }

        /// <summary>
        /// Stuns the enemy
        /// </summary>
        /// <param name="stunTimeMultiplier"></param>
        /// <returns></returns>
        public IEnumerator Stun(float stunTimeMultiplier)
        {
            b_CanAttack = false;
            m_Animator.SetTrigger("Stun");
            yield return new WaitForSeconds(Random.Range(StunTime-1f, StunTime * stunTimeMultiplier));
            b_CanAttack = true;
        }

    }
}
