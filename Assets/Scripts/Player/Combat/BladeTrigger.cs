using ParkourGame.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player.Combat
{
    public class BladeTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Enemy")
            {
                var _enemy = other.GetComponent<Base>();
                _enemy.TakeDamage(20);
            }
        }
    }
}
