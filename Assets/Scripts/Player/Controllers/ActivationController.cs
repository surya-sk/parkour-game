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

        private bool m_IsCrouched;

        // Start is called before the first frame update
        void Start()
        {
            if(PlayerModel == null || StealthPlayerModel == null)
            {
                Debug.LogError("Either or both player models are null!");
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
                }
                else 
                {
                    StealthPlayerModel.transform.position = PlayerModel.transform.position;
                    StealthPlayerModel.transform.rotation = PlayerModel.transform.rotation;
                }
            }
        }
    }

}