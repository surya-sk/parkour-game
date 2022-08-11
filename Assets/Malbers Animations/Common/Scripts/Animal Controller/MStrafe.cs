using UnityEngine;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.Controller
{
    /// <summary>  Make the Calculation for Strafing with Camera or a Target </summary>
    public class MStrafe : MonoBehaviour
    {
        public enum StrafeType { Camera, Target, }

        public StrafeType SType = StrafeType.Camera;
        public BoolReference active;
        public BoolReference Rotate;
        public BoolReference Normalize;
        public BoolReference UpdateAnimator = new BoolReference(true);
        public FloatReference SmoothValue = new FloatReference(15f);
        //public Vector3Reference Gravity = new Vector3Reference(new Vector3(0, -1, 0));

        #region Camera Stuff
        /// <summary>Camera Side to use on Strafing</summary>
        private float strafeAngle;
        private float Side;
        ///  /// <summary>Main Camera </summary>
        public Transform MainCamera;
        #endregion

        private Animator Anim;
        private MAnimal animal;

        public bool Active { set => active.Value = value; get => active.Value; }

        public Transform Target;
        protected Vector3 Direction;
        public bool LeftSide { get; set; }

        public string m_StrafeAngle = "StrafeAngle";
        private int hash_StrafeAngle;

        void OnEnable()
        {
            Anim = gameObject.GetComponent<Animator>();                     //Catche the MainCamera
            animal = gameObject.GetComponent<MAnimal>();                     //Catche the MainCamera
            MainCamera = MTools.FindMainCamera().transform;
            hash_StrafeAngle = Animator.StringToHash(m_StrafeAngle);

            animal.OnStrafe += SetActive;
        }

        private void OnDisable()
        {
            animal.OnStrafe -= SetActive;
        }

        void SetActive(bool valu) => Active = valu;

        public virtual void ToogleActive() => Active ^= true;

        #region Strafing
        /// <summary>Calculate the Strafe Angle using the Camera or a Target</summary>
        protected virtual void LookDirection(float DeltaValue)
        {
            Side = 0;
            Direction = transform.forward;

            if (Target && SType == StrafeType.Target)         //Check if we have Target First
            {
                Direction = (Target.position - transform.position);
            }
            else if (MainCamera && SType == StrafeType.Camera)   //if we do not have Target then use the Main Camera
            {
                Direction = MainCamera.forward;
            }


            Direction = Vector3.ProjectOnPlane(Direction, animal.UpVector).normalized;
            Side = Vector3.Dot(Direction, transform.right);                             //Get the Camera Side Float
            LeftSide = Side > 0;

            var ForwardNormalized = Vector3.ProjectOnPlane(transform.forward, animal.UpVector).normalized;
            float NewHorizontalAngle = (Vector3.Angle(Direction, ForwardNormalized) * (LeftSide ? 1 : -1));     //Get the Normalized value for the look direction

            strafeAngle = Mathf.Lerp(strafeAngle, NewHorizontalAngle, DeltaValue); //Smooth Swap between 1 and -1

            if (Normalize) strafeAngle /= 180;

            if (UpdateAnimator) Anim.SetFloat(hash_StrafeAngle, strafeAngle);
        }

        #endregion


        // Update is called once per frame
        void OnAnimatorMove()
        {
            float DeltaTime = Anim.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
            float DeltaValue = SmoothValue <= 0 ? 1 : (DeltaTime * SmoothValue);

            if (Active)
            {
                LookDirection(DeltaValue);
               
                if (Rotate && animal.MovementDetected)
                {
                    var DesiredRot = transform.rotation * Quaternion.Euler(0, strafeAngle, 0);
                    transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRot, DeltaValue);
                }
            }
            else
            {
                if (UpdateAnimator) Anim.SetFloat(hash_StrafeAngle, strafeAngle);
                strafeAngle = Mathf.Lerp(strafeAngle, 0, DeltaValue); //Smooth Reset
            }

            Debug.DrawRay(transform.position, Direction, Color.blue);
        }
    }
}