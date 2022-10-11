using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player.Controllers
{
    public class CrouchedCharacterController : MonoBehaviour
    {
        public float Speed;
        public float CombatCooldown = 2f;
        public Camera Camera;
        public ActivationController ActivationController;
        public RuntimeAnimatorController CombatAnimatiorController;

        CharacterController m_Controller;
        Vector3 m_Movement;
        Animator m_Animator;
        bool b_InCombatMode;
        bool b_IsReadyToAttack = true;
        RuntimeAnimatorController m_DefaultAnimatorController;

        // Start is called before the first frame update
        void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_DefaultAnimatorController = m_Animator.runtimeAnimatorController;
            ActivationController.OnCombatMode += ActivateCombatMode;
            ActivationController.OnReleaseCombatMode += DeactivateCombatMode; 
        }


        // Update is called once per frame
        void Update()
        {
            HandleMovement();
            if(Input.GetButtonDown("Attack") && b_IsReadyToAttack)
            {
                StartCoroutine(Attack());
            }
        }

        IEnumerator Attack()
        {
            b_IsReadyToAttack = false;
            m_Animator.SetTrigger("Attack");
            if(b_InCombatMode)
            {
                yield return new WaitForSeconds(CombatCooldown);
            }
            //m_Animator.ResetTrigger("Attack");
            b_IsReadyToAttack = true;
        }

        /// <summary>
        /// Handles player movement
        /// </summary>
        private void HandleMovement()
        {
            m_Movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            if (m_Movement.magnitude >= 0.1f)
            {
                m_Animator.SetFloat("Speed", 1f);
                float _targetAngle = Mathf.Atan2(m_Movement.x, m_Movement.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, _targetAngle, 0);
                m_Movement = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
                m_Controller.Move(m_Movement * Speed * Time.deltaTime);
            }
            else
            {
                m_Animator.SetFloat("Speed", 0f);
            }
        }

        private void ActivateCombatMode()
        {
            b_InCombatMode = true;
            m_Animator.runtimeAnimatorController = CombatAnimatiorController;
        }

        private void DeactivateCombatMode()
        {
            b_InCombatMode = false;
            m_Animator.runtimeAnimatorController = m_DefaultAnimatorController;
        }
    }
}
