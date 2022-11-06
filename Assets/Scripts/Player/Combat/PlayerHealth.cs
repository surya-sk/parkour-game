using ParkourGame.Player.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ParkourGame.Player.Combat
{
    public class PlayerHealth : MonoBehaviour
    {
        public GameObject PlayerRef;
        [HideInInspector]
        public bool IsDead { get => b_IsDead; }
        public Action OnStunned;

        private bool b_IsDead;


        /// <summary>
        /// Decreases player health and kills player if health is too low
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            var _health = Player.GetInstance().GetHealth();
            _health -= damage;
            if (b_IsDead)
                return;
            Stun();
            Player.GetInstance().SetHealth(_health);
            if(_health <= 0)
            {
                StartCoroutine(Die());
            }
        }

        /// <summary>
        /// Kill the player
        /// </summary>
        /// <returns></returns>
        IEnumerator Die()
        {
            b_IsDead = true;
            var _animator = GetComponent<Animator>();
            _animator.SetTrigger("Die1");
            yield return new WaitForSeconds(2f);
            Destroy(PlayerRef);
        }

        private void Stun()
        {
            if(Random.Range(0,1) == 0)
            {
                OnStunned?.Invoke();
            }
        }
    }
}

