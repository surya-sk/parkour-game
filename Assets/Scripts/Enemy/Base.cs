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

        public bool IsDead { get; set; }

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
                var _animator = GetComponent<Animator>();
                _animator.Rebind();
                _animator.SetTrigger("Die");
            }
        }

        /// <summary>
        /// Attack the player, ie., reduce player health
        /// </summary>
        /// <param name="player"></param>
        public void Attack(Transform player)
        {
            Debug.Log("Attack!");
        }

    }
}
