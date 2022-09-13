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

    }
}
