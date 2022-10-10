using ParkourGame.Player.Combat;
using ParkourGame.Player.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Enemy
{
    public class WeaponTrigger : MonoBehaviour
    {
        public Base Enemy;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Stealth Player")
            {
                var _playerHealth = other.GetComponent<PlayerHealth>();
                _playerHealth.TakeDamage(Enemy.Damage);
            }
        }
    }
}
