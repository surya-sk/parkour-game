using ParkourGame.Player.Combat;
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
        [HideInInspector]
        public bool IsAttacking { get => b_IsAttacking; }

        CharacterController m_Controller;
        Vector3 m_Movement;
        Animator m_Animator;
        bool b_InCombatMode;
        bool b_IsReadyToAttack = true;
        bool b_IsAttacking = false;
        RuntimeAnimatorController m_DefaultAnimatorController;
        PlayerHealth m_PlayerHealth;

        // Start is called before the first frame update
        void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            m_PlayerHealth = GetComponent<PlayerHealth>();
            m_Animator = GetComponent<Animator>();
            m_DefaultAnimatorController = m_Animator.runtimeAnimatorController;
            ActivationController.OnCombatMode += ActivateCombatMode;
            ActivationController.OnReleaseCombatMode += DeactivateCombatMode; 
        }


        // Update is called once per frame
        void Update()
        {
            if (m_PlayerHealth.IsDead)
                return;
            HandleMovement();
            bool _attack = Input.GetButtonDown("Attack");
            bool _kick = Input.GetButtonDown("Kick");
            if ((_attack || _kick) && b_IsReadyToAttack)
            {
                StartCoroutine(Attack(_kick));
            }
        }

        /// <summary>
        /// Swing the blade
        /// </summary>
        /// <returns></returns>
        IEnumerator Attack(bool kick)
        {
            b_IsReadyToAttack = false;
            string _triggerToSet = "";
            if(kick)
            {
                _triggerToSet = Random.Range(0, 1) == 0 ? "Kick1" : "Kick2";
            }
            else
            {
                b_IsAttacking = true;
                _triggerToSet = Random.Range(0, 1) == 0 ? "Attack1" : "Attack2";
            }
            m_Animator.Rebind();
            m_Animator.SetTrigger(_triggerToSet);
            if(b_InCombatMode)
            {
                yield return new WaitForSeconds(CombatCooldown);
            }
            b_IsReadyToAttack = true;
            yield return new WaitForSeconds(0.5f);
            b_IsAttacking = false;
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
