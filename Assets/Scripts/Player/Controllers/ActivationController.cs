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
            bool _isCrouched = Input.GetAxis("Crouch") == 1;
            if(m_IsCrouched != _isCrouched)
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
    }

}