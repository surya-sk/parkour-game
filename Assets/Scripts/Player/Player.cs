using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player
{
    /// <summary>
    /// A singleton to manage player stats, like health because it can be multiple Player models
    /// </summary>
    public class Player
    {
        private static Player m_Instance = new Player();
        private float m_Health = 100f;

        private Player() { }

        public static Player GetInstance()
        {
            return m_Instance;
        }

        public float GetHealth()
        {
            return m_Health;
        }

        public void SetHealth(float health)
        {
            m_Health = health;
        }
    }
}
