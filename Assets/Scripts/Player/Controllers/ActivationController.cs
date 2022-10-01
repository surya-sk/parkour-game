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
            PlayerModel.SetActive(!_isCrouched);
            StealthPlayerModel.SetActive(_isCrouched);
        }
    }

}