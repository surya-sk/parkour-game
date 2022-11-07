using ParkourGame.Enemy;
using ParkourGame.Player.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player.Combat
{
    public class BladeTrigger : MonoBehaviour
    {
        public ActivationController ActivationController;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Enemy")
            {
                var _player = ActivationController.StealthPlayerModel;
                var _playerController = _player.GetComponent<CrouchedCharacterController>();
                if(_playerController.IsAttacking)
                {
                    var _enemy = other.GetComponent<Base>();
                    var _assassinate = !ActivationController.InCombatMode;
                    _enemy.TakeDamage(20, _assassinate);
                }
            }
        }
    }
}
