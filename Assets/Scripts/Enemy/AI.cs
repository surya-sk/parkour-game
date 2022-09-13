using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Enemy
{
    public class AI : MonoBehaviour
    {
        private Animator m_Animator;

        // Start is called before the first frame update
        void Start()
        {
            m_Animator = GetComponent<Animator>();
            if(m_Animator == null)
            {
                Debug.LogError($"Enemy {gameObject.name} animator not found!!");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
