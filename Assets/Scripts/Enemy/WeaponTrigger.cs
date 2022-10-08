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
            if(other.gameObject.tag == "Player")
            {
                //var _playerController = other.GetComponent<CrouchedCharacterController>();
                //_playerController.TakeDamage(Enemy.Damage);
            }
        }
    }
}
