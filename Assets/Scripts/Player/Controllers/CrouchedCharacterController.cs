using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player.Controllers
{
    public class CrouchedCharacterController : MonoBehaviour
    {
        public float Speed;
        public Camera Camera;

        CharacterController m_Controller;
        Vector3 m_Movement;
        Animator m_Animator;
        // Start is called before the first frame update
        void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            m_Movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            if (m_Movement.magnitude >= 0.1f)
            {
                m_Animator.SetTrigger("Move");
                float _targetAngle = Mathf.Atan2(m_Movement.x, m_Movement.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, _targetAngle, 0);
                m_Movement = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
                m_Controller.Move(m_Movement * Speed * Time.deltaTime);
            }
            else
            {
                m_Animator.SetTrigger("Idle");
            }
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("Taking damage from enemy");
        }
    }
}
