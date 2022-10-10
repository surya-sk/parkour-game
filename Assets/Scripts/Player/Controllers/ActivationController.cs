using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Player.Controllers
{
    /// <summary>
    /// Decides when to activate crouched controller based on stealth state
    /// </summary>
    public class ActivationController : MonoBehaviour
    {
        public GameObject PlayerModel;
        public GameObject StealthPlayerModel;

        public Action OnCrouched;
        public Action OnUncrouched;
        public Action OnCombatMode;
        public Action OnReleaseCombatMode;
        public bool ForceCrouch { get; set; }

        private bool m_IsInCombatMode;
        private bool m_IsCrouched;

        // Start is called before the first frame update
        void Start()
        {
            if(PlayerModel == null || StealthPlayerModel == null)
            {
                Debug.LogError("Either or both player models are missing!");
                return;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (ForceCrouch)
                return;
            bool _isCrouched = Input.GetAxis("Crouch") == 1 ? !m_IsInCombatMode : false;
            SwitchCrouchState(_isCrouched);
        }

        /// <summary>
        /// Switch between crouched and non-crouched state
        /// </summary>
        /// <param name="_isCrouched"></param>
        public void SwitchCrouchState(bool _isCrouched)
        {
            if (m_IsCrouched != _isCrouched)
            {
                m_IsCrouched = _isCrouched;
                PlayerModel.SetActive(!_isCrouched);
                StealthPlayerModel.SetActive(_isCrouched);
                if (!m_IsCrouched) // means player just stopped crouching
                {
                    PlayerModel.transform.position = StealthPlayerModel.transform.position;
                    PlayerModel.transform.rotation = StealthPlayerModel.transform.rotation;
                    OnUncrouched?.Invoke();
                }
                else
                {
                    StealthPlayerModel.transform.localPosition = PlayerModel.transform.localPosition;
                    StealthPlayerModel.transform.rotation = PlayerModel.transform.rotation;
                    OnCrouched?.Invoke();
                }
            }
        }

        /// <summary>
        /// Set if player can be in combat mode.
        /// </summary>
        /// <param name="b"></param>
        public void SetCombatMode(bool b)
        {
            if(b)
            {
                OnCombatMode?.Invoke();
            }
            else
            {
                OnReleaseCombatMode?.Invoke();
            }
        }
    }

}