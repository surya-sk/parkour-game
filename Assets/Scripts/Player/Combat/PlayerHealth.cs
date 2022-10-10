using ParkourGame.Player.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player.Combat
{
    public class PlayerHealth : MonoBehaviour
    {
        public GameObject PlayerRef;

        /// <summary>
        /// Decreases player health and kills player if health is too low
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            var _health = Player.GetInstance().GetHealth();
            _health -= damage;
            Player.GetInstance().SetHealth(_health);
            if(_health <= 0)
            {
                StartCoroutine(Die());
            }
            Debug.Log("Damage!");
        }

        IEnumerator Die()
        {
            var _animator = GetComponent<Animator>();
            _animator.SetTrigger("Die");
            yield return new WaitForSeconds(2f);
            Destroy(PlayerRef);
        }
    }
}

