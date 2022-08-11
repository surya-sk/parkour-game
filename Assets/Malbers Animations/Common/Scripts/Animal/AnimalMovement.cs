using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations.Utilities;

namespace MalbersAnimations
{
    //Animal Logic
    public partial class Animal
    {

        void Awake()
        {
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Int, Hash.Type))
            {
                Anim.SetInteger("Type", animalTypeID);                         //Adjust the layer for the curret animal Type this is of offseting the bones to another pose
            }
            WaterLayer = LayerMask.GetMask("Water");
        }

        void Start()
        {
            SetStart();
        }

        void OnEnable()
        {
            if (Animals == null) Animals = new List<Animal>();

            Animals.Add(this);          //Save the the Animal on the current List
        }

        void OnDisable()
        {
            Animals.Remove(this);   //Remove all this animal from the Overall AnimalList
        }

        ///// <summary>
        ///// Rays cast was very time consuming when you have 100 animals so lest try not raycast all at the same time
        ///// </summary>
        private void PerformanceSettings()
        {
            //PerformanceID = UnityEngine.Random.Range(100000, 999999);       //Used for the animals Raycasting Stuff
        }

        protected virtual void SetStart()
        {
            _transform = this.transform;                                        //Set Reference for the transform
            AnimalMesh = GetComponentInChildren<Renderer>();

            if (_RigidBody) _RigidBody.isKinematic = false;                     //Some People set it as Kinematic and falling stop working (Just to make sure)

            pivots = GetComponentsInChildren<Pivots>().ToList();                //Pivots are Strategically Transform objects use to cast rays used to calculate the terrain inclination 
            scaleFactor = _transform.localScale.y;                              //TOTALLY SCALABE animal
            MovementReleased = true;

            if (pivots != null)
            {
                pivot_Hip = pivots.Find(p => p.name.ToUpper().Contains("HIP"));
                pivot_Chest = pivots.Find(p => p.name.ToUpper().Contains("CHEST"));
                pivot_Water = pivots.Find(p => p.name.ToUpper().Contains("WATER"));
            }

            switch (StartSpeed)                                     //Set Start Speed
            {
                case Ground.walk: Speed1 = true;
                    break;
                case Ground.trot: Speed2 = true;
                    break;
                case Ground.run: Speed3 = true;
                    break;
                default:
                    break;
            }


            //if (_RigidBody)                                                     // Kind of fixing the problems with the rigidbody and the animator
            //{
            //    switch (Anim.updateMode)
            //    {
            //        case AnimatorUpdateMode.Normal:
            //            _RigidBody.interpolation = RigidbodyInterpolation.Extrapolate;
            //            break;
            //        case AnimatorUpdateMode.AnimatePhysics:
            //            _RigidBody.interpolation = RigidbodyInterpolation.None;
            //            break;
            //        default:
            //            break;
            //    }
            //}


            Attack_Triggers = GetComponentsInChildren<AttackTrigger>(true).ToList();        //Save all Attack Triggers.

            OptionalAnimatorParameters();                                                   //Enable Optional Animator Parameters on the Animator Controller;

            Start_Flying();
        }

        /// <summary>
        /// Setting the animal that flies when start
        /// </summary>
        protected virtual void Start_Flying()
        {
            if (hasFly && StartFlying && canFly)
            {
                stand = false;
                Fly = true;
                Anim.Play("Fly", 0);
                IsInAir = true;
                if (_RigidBody) _RigidBody.useGravity = false;
            }
        }

        /// <summary>
        ///  //Enable Optional Animator Parameters on the Animator Controller;
        /// </summary>
        protected void OptionalAnimatorParameters()
        {
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Swim)) hasSwim = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Dodge)) hasDodge = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Fly)) hasFly = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Attack2)) hasAttack2 = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Stunned)) hasStun = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Underwater)) hasUnderwater = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Float, Hash.UpDown)) hasUpDown = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Float, Hash.Slope)) hasSlope = true;
        }

        /// <summary>
        /// Link all Parameters to the animator
        /// </summary>
        public virtual void LinkingAnimator()
        {
            if (!Death)
            {
                Anim.SetFloat(Hash.Vertical, forwardMovement);
                Anim.SetFloat(Hash.Horizontal, direction);
                Anim.SetBool(Hash.Stand, stand);
                Anim.SetBool(Hash.Shift, shift);
                Anim.SetBool(Hash._Jump, jump);
                Anim.SetBool(Hash.Attack1, attack1);
                Anim.SetBool(Hash.Damaged, damaged);
                Anim.SetBool(Hash.Action, action);
                Anim.SetInteger(Hash.IDAction, actionID);
                Anim.SetInteger(Hash.IDInt, IDInt);                //The problem is that is always zero if you change it externally;


                //Optional Animator Parameters
                if (hasSlope) Anim.SetFloat(Hash.Slope, slope);
                if (hasStun) Anim.SetBool(Hash.Stunned, stun);
                if (hasAttack2) Anim.SetBool(Hash.Attack2, attack2);
                if (hasUpDown) Anim.SetFloat(Hash.UpDown, movementAxis.y);
                if (hasDodge) Anim.SetBool(Hash.Dodge, dodge);
                if (hasFly && canFly) Anim.SetBool(Hash.Fly, Fly);
                if (hasSwim && canSwim) Anim.SetBool(Hash.Swim, swim);
                if (hasUnderwater && CanGoUnderWater) Anim.SetBool(Hash.Underwater, underwater);
            }

            Anim.SetBool(Hash.Fall, fall);  //Update  fall either if is death or not

            OnSyncAnimator.Invoke(); //Ready to Sync all the parameters with external scripts ... (Riding System)
        }

        /// <summary>
        /// Gets the movement from the Input Script or AI
        /// </summary>
        /// <param name="move">Direction VEctor</param>
        /// <param name="active">Active = true means that is taking the direction Move</param>
        public virtual void Move(Vector3 move, bool active = true)
        {
            MovementReleased = move.x == 0 && move.z == 0;

            if (active)
            {
                // convert the world relative moveInput vector into a local-relative
                // turn amount and forward amount required to head in the desired
                // direction.
                if (move.magnitude > 1f) move.Normalize();
                move = transform.InverseTransformDirection(move);

                if (!Fly && !underwater)
                {
                    move = Vector3.ProjectOnPlane(move, /*-Physics.gravity*/ SurfaceNormal).normalized;       //If is not underwater and not flying remove the Y axis and added to the other ones
                }

                turnAmount = Mathf.Atan2(move.x, move.z);                   //Convert it to relative

                forwardAmount = move.z;

                if (forwardAmount > 0) forwardAmount = 1;               //It will remove slowing Down when rotating
                if (forwardAmount < 0) forwardAmount = -1;               //It will remove slowing Down when rotating


                movementAxis = new Vector3(turnAmount, movementAxis.y, Mathf.Abs(forwardAmount));

                if (!jump && !down)                   //Up & Down movement while flying or swiming;
                {
                    if (Fly || underwater)
                    {
                        float Ylimit = move.y;

                        Ylimit = Ylimit > 0 ? Ylimit * YAxisPositiveMultiplier : Ylimit * YAxisNegativeMultiplier;
                        movementAxis.y = Mathf.Lerp(movementAxis.y, Ylimit, Time.deltaTime);
                    }
                }

                if (!stand && !RealAnimatorState(Hash.Action) && !RealAnimatorState(Hash.Tag_Sleep)) //if is not stand and is not making an action
                {
                    DeltaRotation *= Quaternion.Euler(0, movementAxis.x * Time.deltaTime * TurnMultiplier, 0);
                    DeltaPosition += DeltaPositionFrom_RotateAround(AnimalMesh.bounds.center, UpVector, movementAxis.x * Time.deltaTime * TurnMultiplier);
                    //_transform.RotateAround(animalMesh.bounds.center, UpVector, movementAxis.x * Time.deltaTime * TurnMultiplier); //Add Extra rotation when is not standing
                }


                if (RealAnimatorState(Hash.Action)) movementAxis = Vector3.zero;    // No movement when is making an action
                return;
            }
            else
            {
                movementAxis = new Vector3(move.x, movementAxis.y, move.z);         //Do not convert to Direction Input Mode (Camera or AI)
            }
        }


        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Add more Rotations to the current Turn Animations 
        /// </summary>
        protected virtual void AdditionalTurn(float time)
        {
            float Turn = 0;

            if (GroundSpeed == 1) Turn = walkSpeed.rotation;                                    //If we are walking set the turn ammount to walkSpeed Rotation
            if (GroundSpeed == 2) Turn = trotSpeed.rotation;                                    //"" "" """ troting     """    ""  ""   '"  trotSpeed Rotation
            if (GroundSpeed == 3) Turn = runSpeed.rotation;                                     //"" "" """ running   "  """    ""  ""   '" RunSpeed Rotation

            if (CurrentAnimState.tagHash != Hash.Locomotion) Turn = 0;                      //Set the turn to 0 if the current animation is not locomotion

           // if (Fly) Turn = flySpeed.rotation;
            if (swim) Turn = swimSpeed.rotation;
            //if (underwater) Turn = underWaterSpeed.rotation;

            float clampDirection = Mathf.Clamp(direction, -1, 1) * (movementAxis.z >= 0 ? 1 : -1);  //Add +Rotation when going Forward and -Rotation when going backwards


            DeltaRotation *= Quaternion.Euler(0, Turn * 2 * clampDirection * time, 0);

            if (Fly || swim || stun || RealAnimatorState(Hash.Action)) return;      //Skip the code below if is in any of this states


            if (IsJumping() || fall)                                                //More Rotation when jumping and falling... in air rotation
            {
                float amount = airRotation * direction * time * (movementAxis.z >= 0 ? 1 : -1);

                DeltaRotation *= Quaternion.Euler(0, amount, 0);
                DeltaPosition += DeltaPositionFrom_RotateAround(AnimalMesh.bounds.center, _transform.up, amount);
            }
        }

        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Add more Speed to the current Move animations
        /// </summary>
        protected virtual void AdditionalSpeed(float time)
        {
            float amount = 0;                               //Additional Speed Amount
            float Anim_LerpSpeed = 1;                       //Smoothness Multiplier of the Animator Speed

            if (groundSpeed == 1)
            {
                amount = walkSpeed.position;
                CurrentAnimatorSpeed = walkSpeed.animator;
                Anim_LerpSpeed = walkSpeed.lerpAnimator;
            }
            if (groundSpeed == 2 || (groundSpeed == 1 && Shift))
            {
                amount = trotSpeed.position;
                CurrentAnimatorSpeed = trotSpeed.animator;
                Anim_LerpSpeed = trotSpeed.lerpAnimator;
            }

            if (groundSpeed == 3 || (groundSpeed == 2 && Shift))
            {
                amount = runSpeed.position;
                CurrentAnimatorSpeed = runSpeed.animator;
                Anim_LerpSpeed = runSpeed.lerpAnimator;
            }

            if (CurrentAnimState.tagHash != Hash.Locomotion || MovementAxis.z == 0) //Reset to the Default if is not on the Locomotion State
            {
                CurrentAnimatorSpeed = 1;
            }

            if (forwardMovement < 0) amount = walkSpeed.position;

            amount = amount * ScaleFactor * Mathf.Clamp(Mathf.Abs(forwardMovement), -1, 1);

            if (hasSwim && swim)                                                   //Change values to Swim
            {
                amount = swimSpeed.position;
                CurrentAnimatorSpeed = swimSpeed.animator;
                Anim_LerpSpeed = swimSpeed.lerpAnimator;
            }

            if (hasFly && fly)                                                    //Change values to Fly
            {
                //amount = flySpeed.position;
                CurrentAnimatorSpeed = flySpeed.animator;
                Anim_LerpSpeed = flySpeed.lerpAnimator;
            }

            Vector3 forward = transform.forward * forwardMovement;
            //forward += transform.up * movementAxis.y;

            if (forward.magnitude > 1) forward.Normalize();

            DeltaPosition += forward * amount / 5f * time;

            Anim.speed = Mathf.Lerp(Anim.speed, CurrentAnimatorSpeed * animatorSpeed, time * Anim_LerpSpeed);               //Changue the velocity of the animator
        }

        /// <summary>
        /// Updates the MovementAxis.Y.
        /// </summary>
        /// <param name="smoothness">Smoothness</param>
        public virtual void YAxisMovement(float smoothness, float time)
        {
            if (jump)
            {
                movementAxis.y = Mathf.Lerp(movementAxis.y, LockUp ? 0 : 1, time * smoothness);
            }
            else if (down)
            {
                movementAxis.y = Mathf.Lerp(movementAxis.y, -1, time * smoothness);
            }
            else
            {
                movementAxis.y = Mathf.Lerp(movementAxis.y, 0, time * smoothness);
            }


            if (Mathf.Abs(movementAxis.y) < 0.001f) movementAxis.y = 0;         //Remove extra float values...

        }

        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Updates Movement on Platforms
        /// </summary>
        private void UpdatePlatformMovement()
        {
            if (platform == null) return;

            if (RealAnimatorState(Hash.Tag_Jump)) { platform = null; return; }         //Do not calculate if you are jumping

            var DeltaPlatformPos = platform.position - platform_Pos;
            DeltaPlatformPos.y = 0;                                                     //the Y is handled by the Fix Position

            DeltaPosition += DeltaPlatformPos;                          // Keep the same relative position.

            //var target =_transform.position + platform.position - platform_Pos; //// Keep the same relative position.
            //transform.position = target;

            platform_Pos = platform.position;

            var deltaAngle = platform.eulerAngles.y - platform_formAngle;       //The diference between the this and the last frame

            if (deltaAngle == 0) return;            // no rotation founded.. Skip the code below

            DeltaRotation *= Quaternion.Euler(0, deltaAngle, 0);
            DeltaPosition += DeltaPositionFrom_RotateAround(platform.position, Vector3.up, deltaAngle);             //Move Position to the relative rotation pivot of the platform..

            //if (platform.eulerAngles.y != platform_formAngle) 
            //transform.RotateAround(platform.position, Vector3.up, deltaAngle);
            platform_formAngle = platform.eulerAngles.y;
        }

        virtual protected Vector3 DeltaPositionFrom_RotateAround(Vector3 point, Vector3 axis, float deltaAngle)
        {
            Vector3 pos = transform.position;
            Vector3 direction = pos - point;
            Quaternion rotation = Quaternion.AngleAxis(deltaAngle, axis);
            direction = rotation * direction;

            pos = point + direction - pos;
            pos.y = 0;                                                      //the Y is handled by the Fix Position

            return pos;
        }

        /// <summary>
        /// Mayor Raycasting stuff to align and calculate the ground from the animal ****IMPORTANT***
        /// </summary>
        protected void RayCasting()
        {
            UpVector = -Physics.gravity;
            scaleFactor = _transform.localScale.y;                      //Keep Updating the Scale Every Frame
            _Height = height * scaleFactor;                             //multiply the Height by the scale

            backray = frontray = false;

            hit_Chest = new RaycastHit();                               //Clean the Raycasts every time 
            hit_Hip = new RaycastHit();                                 //Clean the Raycasts every time 

            hit_Chest.distance = hit_Hip.distance = _Height;            //Reset the Distances to the Heigth of the animal

            if (Pivot_Hip != null) //Ray From the Hip to the ground
            {
                if (Physics.Raycast(Pivot_Hip.GetPivot, -transform.up, out hit_Hip, scaleFactor * Pivot_Hip.multiplier, GroundLayer))
                {
                    if (debug) Debug.DrawRay(hit_Hip.point, hit_Hip.normal * 0.2f, Color.blue);

                    backray = true;

                    if (platform == null && !RealAnimatorState(Hash.Tag_Jump))               //Platforming logic
                    {
                        platform = hit_Hip.transform;
                        platform_Pos = platform.position;
                        platform_formAngle = platform.eulerAngles.y;
                    }
                }
                else { platform = null; }
            }
           


            //Ray From Chest to the ground ***Use the pivot for calculate the ray... but the origin position to calculate the distance
            if (Physics.Raycast(Chest_Pivot_Point, -transform.up, out hit_Chest, Chest_Pivot_Multiplier, GroundLayer))
            {
                if (debug) Debug.DrawRay(hit_Chest.point, hit_Chest.normal * 0.2f, Color.red);

                if (hit_Chest.normal.y > 0.7)  // if the hip ray if in Big Angle ignore it
                    frontray = true;

                if ((platform == null || platform != hit_Chest.transform) && !RealAnimatorState(Hash.Tag_Jump))      //Platforming
                {
                    platform = hit_Chest.transform;
                    platform_Pos = platform.position;
                    platform_formAngle = platform.eulerAngles.y;
                }

            }
            //else { platform = null; }


            if (!frontray && Stand) //Hack if is there's no ground beneath the animal and is on the Stand Sate;
            {
                fall = true;

                if (pivot_Hip && backray) fall = false;
            }
        }


        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Terrain Aligment Logic
        /// </summary>
        protected virtual void FixPosition(float time)
        {
            slope = 0;

            //───────────────────────────────────────────────CHECK FOR ANIMATIONS THAT WILL SKIP THE REST OF THE METHOD ───────────────────────────────────────────────────────────────────────────────────
            if (swim || fly || underwater) return;
            //────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


            //───────────────────────────────────────────────Terrain Adjusment───────────────────────────────────────────────────────────────────────────────────

            //Calculate the Align vector of the terrain
            if (pivot_Hip)
            {
                if (Pivot_Chest)
                {
                    Vector3 direction = (hit_Chest.point - hit_Hip.point).normalized;
                    Vector3 Side = Vector3.Cross(UpVector, direction).normalized;
                    surfaceNormal = Vector3.Cross(direction, Side).normalized;
                }
                else
                {
                    surfaceNormal = hit_Hip.normal;
                }

                float AngleSlope = Vector3.Angle(surfaceNormal, UpVector);                              //Calculate the Angle of the Terrain

                var forward = transform.forward;
                forward.y = 0;

                float SlopeDirection = 1;

                if (pivot_Chest && pivot_Hip)
                {
                    SlopeDirection = pivot_Chest.Y > pivot_Hip.Y ? 1 : -1;
                }

                slope = AngleSlope / maxAngleSlope * (SlopeDirection <= 0 ? 1 : -1);         //Normalize the AngleSlop by the MAX Angle Slope and make it positive(HighHill) or negative(DownHill)
                

                Quaternion finalRot = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;  //Calculate the orientation to Terrain 
                Quaternion deltaFixRotation = Quaternion.Inverse(transform.rotation) * finalRot;

                if (isInAir || IsJumping() || !backray)                                     // If the character is falling, jumping or swimming smoothly aling with the Up Vector
                {
                    if (slope < 0 || (fall && !IsJumping()))                                //Don't Align when is UpHill falling or jumping
                    {
                        finalRot = Quaternion.FromToRotation(transform.up, UpVector) * transform.rotation;

                        deltaFixRotation = Quaternion.Inverse(transform.rotation) * finalRot;
                        deltaFixRotation = Quaternion.Slerp(DeltaRotation, DeltaRotation * deltaFixRotation, time * AlingToGround/2);
                        DeltaRotation *= deltaFixRotation; 

                       // _transform.rotation = Quaternion.Slerp(transform.rotation, finalRot, time * 5f);
                    }
                }
                else
                {
                    if (!swim && slope < 1)
                    {
                        if (!Stand)
                        {
                            deltaFixRotation =  Quaternion.Slerp(DeltaRotation, DeltaRotation * deltaFixRotation, time * AlingToGround);
                        }

                        DeltaRotation *= deltaFixRotation;

                        //_transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, time * 10f);                 //Align with Terrain!!! *** Important (USING TRANSFORM)
                    }
                    else { }
                }
            }

            float distance = hit_Hip.distance;
            if (!backray) distance = hit_Chest.distance;         //if is landing on the front feets


            float realsnap = SnapToGround;                  //Change in the inspector the  adjusting speed for the terrain 
            float diference = _Height - distance;

            //───────────────────────────────────────────────Snap To Terrain  -HIGHER───────────────────────────────────────────────────────────────────────────────────
            if (distance > _Height)
            {
                if (!isInAir && !swim)
                {
                    if (CurrentAnimState.tagHash == Hash.Locomotion || Stand)                            //If is in locomotion state
                    {
                        DeltaPosition += new Vector3(0, diference, 0);
                        //_transform.position = transform.position + new Vector3(0, diference, 0); //For Going DowHill otherWhise the aninal will float while going downHill
                    }
                    else
                    {
                        DeltaPosition += new Vector3(0, diference * time * realsnap, 0);
                        //_transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, diference, 0), time * realsnap);
                    }
                }
            }
            //───────────────────────────────────────────────Snap To Terrain  -LOWER───────────────────────────────────────────────────────────────────────────────────
            else
            {
                if (!fall && !IsInAir)
                {
                    if (diference < 0.1f || Stand)
                    {
                        DeltaPosition += new Vector3(0, diference, 0);
                        // _transform.position = transform.position + new Vector3(0, diference, 0); //for platforming
                    }
                    else
                    {
                        DeltaPosition += new Vector3(0, diference * time * realsnap, 0);
                        //_transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, diference, 0), time * realsnap);
                    }
                }

                if (_RigidBody)
                {
                    if (RealAnimatorState(Hash.Action) || IsJumping()) return;                         //if we are on the Action/Jump State or we are transition it to it skip

                    if (RealAnimatorState(Hash.Locomotion)                       //Restore the Contraints if is in any of these states
                    || RealAnimatorState(Hash.Tag_Idle)
                    || RealAnimatorState(Hash.Tag_JumpEnd))
                    {
                        _RigidBody.constraints = Still_Constraints;                 //Lock Y axis
                    }
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Fall Logic
        /// </summary>
        protected virtual void Falling()
        {
            float Multiplier = Chest_Pivot_Multiplier;
            if (debug) Debug.DrawRay(fall_Point, -transform.up * Multiplier, Color.magenta);

            //Don't Calcultate Fall Ray if the animal on any of these states
            if (CurrentAnimState.tagHash == Hash.Tag_Idle) return;
            if (CurrentAnimState.tagHash == Hash.Tag_Sleep) return;
            if (CurrentAnimState.tagHash == Hash.Action) return;
            if (CurrentAnimState.tagHash == Hash.Swim) return;


            RaycastHit hitpos;

            if (CurrentAnimState.tagHash == Hash.Tag_Jump || CurrentAnimState.tagHash == Hash.Fall) //If the animal is falling or jumping add the fall multiplier
            {
                Multiplier *= FallRayMultiplier;
            }

            //Set the Fall Ray a bit farther from the front feet.

            fall_Point = Chest_Pivot_Point;

            if (!Fly)
                fall_Point += (_transform.forward.normalized * (Shift ? GroundSpeed + 1 : GroundSpeed) * FallRayDistance * ScaleFactor); //Calculate ahead the falling ray

           

            if (Physics.Raycast(fall_Point, -_transform.up, out hitpos, Multiplier, GroundLayer))
            {
                fall = false;

                if (Fly && land)
                {
                    SetFly(false);                                          //Reset Fly when the animal is near the ground
                    IsInAir = false;
                    groundSpeed = LastGroundSpeed;                          //Restore the GroundSpeed to the original
                }

                if (RealAnimatorState(Hash.Tag_SwimJump)) Swim = false;     //in case is jumping from the water to the ground ***Important

                if (Vector3.Angle(hitpos.normal, UpVector) > maxAngleSlope && IsJumping()) fall = true; //if the ground but is to Sloppy
            }
            else
            { fall = true; }
        }

        /// <summary>
        /// Swim Logic
        /// </summary>
        protected virtual void Swimming(float time)
        {
            if (!canSwim) return;                               //If it cannot swim do nothing
            if (CanGoUnderWater && underwater) return;          //if we are underwater this behavior does not need to be calcultate **Important**
            if (Stand) return;                                  //Skip if where doing nothing
            if (!hasSwim) return;                               //If doesnt have swimm animation don't do the swimming calculations
            if (!pivot_Water) return;                           //If there's no water Pivot do nothing

            Vector3 Up = _transform.up;
            RaycastHit WaterHitCenter;

            //Front RayWater Cast
            if (Physics.Raycast(pivot_Water.transform.position, -Up, out WaterHitCenter, scaleFactor * pivot_Water.multiplier, WaterLayer))
            {
                waterlevel = WaterHitCenter.point.y;                //Get the water level when find water

                if (!isInWater) isInWater = true;                   //Has found a water layer.. so Set isInWater to true
            }
            else
            {
                if (isInWater && !RealAnimatorState(Hash.Tag_SwimJump))
                {
                    isInWater = false;
                }
            }

            if (isInWater)                                                  //if we hit water
            {
                if  ((hit_Chest.distance < (_Height * 0.8f) && movementAxis.z > 0 && hit_Chest.transform != null)    //Exit the water walking forward  and it has found land
                    || (hit_Hip.distance < (_Height * 0.8f) && movementAxis.z < 0 && hit_Hip.transform != null))     //Exit the water walking backward and it has found land
                {
                    if (CurrentAnimState.tagHash != Hash.Tag_Recover)       //Don't come out of the water if you are playing entering water
                    {
                        Swim = false;
                        return;
                    }
                }
                if (!swim)
                {
                    if (Pivot_Chest.Y <= waterlevel) //Enter the water if the water is above chest level
                    {
                        Swim = true;
                        OnSwim.Invoke();
                        if (_RigidBody) _RigidBody.constraints = Still_Constraints;
                    }
                }
            }

            if (swim)
            {
                fall = isInAir = fly = false;  // Reset all other states

                float angleWater = Vector3.Angle(Up, WaterHitCenter.normal);  //Calculates the angle of the water

                Quaternion finalRot = Quaternion.FromToRotation(Up, WaterHitCenter.normal) * transform.rotation;        //Calculate the rotation forward for the water

                //Slerp rotate until is Aling with the Water (USING TRANSFORM)
                //if (angleWater > 0.5f)
                //    _transform.rotation = Quaternion.Slerp(_transform.rotation, finalRot, time * 10);
                //else
                //    _transform.rotation = finalRot;

                //Slerp rotate until is Aling with the Water (USING DELTA ROTATION)

                Quaternion deltaFixRotation = Quaternion.Inverse(transform.rotation) * finalRot;

                if (angleWater > 0.5f)
                {
                    deltaFixRotation = Quaternion.Slerp(DeltaRotation, DeltaRotation * deltaFixRotation, time * 10f);
                }

                DeltaRotation *= deltaFixRotation;

                if (CurrentAnimState.tagHash != Hash.Tag_SwimJump)      //if is not Swim Jumping then aling with the water
                {
                    //Smoothy Aling position with the Water
                    deltaPosition.y =((waterlevel - _Height + waterLine)  - _transform.position.y) * time * 5f;  

                    //Vector3 NewPos = new Vector3(_transform.position.x, waterlevel - _Height + waterLine, _transform.position.z);
                    //transform.position = Vector3.Lerp(_transform.position, NewPos, time * 5f);
                }
              

                //-------------------Go UnderWater---------------
                if (CanGoUnderWater && down && !IsJumping() && !RealAnimatorState(Hash.Tag_SwimJump))
                {
                    underwater = true;
                }
            }
        }

        // Check for a behind Cliff so it will stop going backwards.
        protected virtual bool IsFallingBackwards(float ammount = 0.5f)
        {
            RaycastHit BackRay = new RaycastHit();
            var point = Pivot_Hip ? Pivot_Hip.transform.position : _transform.position + new Vector3(0, _Height, 0);
            var multiplier = Pivot_Hip ? Pivot_Hip.multiplier * FallRayMultiplier : FallRayMultiplier;

            Vector3 FallingVectorBack = point + _transform.forward * -1 * ammount;

            if (debug) Debug.DrawRay(FallingVectorBack, -_transform.up * multiplier * scaleFactor, Color.white);                 //Draw a White Ray

            if (Physics.Raycast(FallingVectorBack, -_transform.up, out BackRay, scaleFactor * multiplier, GroundLayer))
            {
                if (BackRay.normal.y < 0.6)  // if the back ray if in Big Angle don't walk 
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (!swim && movementAxis.z < 0) return true; //Check if the animal is moving backwards
            }
            return false;
        }

        /// <summary>
        /// Movement Trot Walk Run (Velocity changes)
        /// </summary>
        protected void MovementSystem(float s1 = 1, float s2 = 2, float s3 = 3)
        {
            float smooth = 2;
            float directionSmooth = 8;

            if (groundSpeed == 1)   
            {
                smooth = walkSpeed.lerpPosition;       
                directionSmooth = walkSpeed.lerpRotation;
            }
            if (groundSpeed == 2)
            {
                smooth = trotSpeed.lerpPosition;
                directionSmooth = trotSpeed.lerpRotation;
            }
            if (groundSpeed >= 3)
            {
                smooth = runSpeed.lerpPosition;
                directionSmooth = runSpeed.lerpRotation;
            }

            float maxspeed = groundSpeed;           //Do not override the groundSpeed

            if (swim)
            {
                maxspeed = 1;
                directionSmooth = swimSpeed.lerpRotation;
                smooth = swimSpeed.lerpPosition;
            }

            if (underwater)
            {
                maxspeed = 1;
                directionSmooth = underWaterSpeed.lerpRotation;
                smooth = underWaterSpeed.lerpPosition;
            }

            if (shift) maxspeed++;                                                  //Increase the Speed with Shift pressed

            if (!Fly && !Swim && !IsJumping() )                                                      //Dont check for slopes when swimming or flying
            {
                if (SlowSlopes && slope >= 0.5 && maxspeed > 1) maxspeed--;         //SlowDown When going UpHill

                if (slope >= 1)                                                     //Prevent to go uphill
                {
                    maxspeed = 0;
                    smooth = 10;
                }
            }

            if (Fly)
            {
                YAxisMovement(upDownSmoothness, Time.deltaTime);                //Controls the Fly Movement Up and Down
                smooth = flySpeed.lerpPosition;
                directionSmooth = flySpeed.lerpRotation;
            }

            if (movementAxis.z < 0)                             //if is walking backwards check for a cliff
            {
                if (!swim && !Fly && IsFallingBackwards())
                {
                    maxspeed = 0;
                    smooth = 10;
                }
            }
           

            forwardMovement = Mathf.Lerp(forwardMovement, movementAxis.z * maxspeed, Time.deltaTime * smooth);                          //smoothly transitions bettwen velocities
            direction = Mathf.Lerp(direction, movementAxis.x * (shift ? 2 : 1), Time.deltaTime * directionSmooth);  //smoothly transitions bettwen directions

            if (Mathf.Abs(direction)>0.1f || (Mathf.Abs(forwardMovement) > 0.2f))   //Check if the Character is Standing
                stand = false;
            else stand = true;

            if (!MovementReleased) stand = false;
           

            if (jump || damaged || stun || fall || swim || fly || isInAir || (tired >= GotoSleep && GotoSleep != 0)) stand = false; //Stand False when doing some action

            if (tired >= GotoSleep) tired = 0;          //Reset Time Out

            if (!stand) tired = 0;                      //Reset Tired if is moving;

            if (!swim && !fly) movementAxis.y = 0;      //Reset Movement in Y if is not swimming or flying
        }

        void FixedUpdate()                              
        {
            if (CanGoUnderWater && underwater) return;                          //Dont calculate the methods below  if we are swimming

            if (anim.updateMode == AnimatorUpdateMode.AnimatePhysics)
            {
                RayCasting();
                FixPosition(Time.fixedDeltaTime);
                AdditionalSpeed(Time.fixedDeltaTime);                           //Apply Speed movement Turn movement
                AdditionalTurn(Time.fixedDeltaTime);                            //Apply Additional Turn movement
                Falling();
                Swimming(Time.fixedDeltaTime);                                  //Calculate more acurately the Swiming if the animal is falling
                UpdatePlatformMovement();


                ApplyDeltaTransform();
            }
        }

        void Update()
        {
            currentAnimState = Anim.GetCurrentAnimatorStateInfo(0);             //Store the current Animation State
            nextAnimState = Anim.GetNextAnimatorStateInfo(0);                   //Store the Next    Animation State
            if (anim.updateMode != AnimatorUpdateMode.AnimatePhysics)
            {
                RayCasting();
                FixPosition(Time.deltaTime);
                AdditionalTurn(Time.deltaTime);                                 //Apply Additional Turn movement ON UPDATE
                AdditionalSpeed(Time.deltaTime);                                //Apply Speed movement Turn movement ON UPDATE
                Falling();
                Swimming(Time.deltaTime);                                //Calculate Swimming Logic when not falling on Update
                UpdatePlatformMovement();


                ApplyDeltaTransform();
            }
        }


        private void ApplyDeltaTransform()
        {
            transform.position += DeltaPosition;
            if (anim.updateMode != AnimatorUpdateMode.AnimatePhysics) transform.rotation = anim.rootRotation;
            transform.rotation *= DeltaRotation;

            DeltaPosition = Vector3.zero;
            DeltaRotation = Quaternion.identity;
        }

        void LateUpdate()
        {
            MovementSystem(movementS1, movementS2, movementS3);
            LinkingAnimator();              //Set all Animator Parameters
        }


#if UNITY_EDITOR
        /// <summary>
        /// DebugOptions
        /// </summary>
        void OnDrawGizmos()
        {
            if (debug && Application.isPlaying)
            {
                pivots = GetComponentsInChildren<Pivots>().ToList();

                Gizmos.color = Color.magenta;
                float sc = transform.localScale.y;

                if (backray)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(hit_Hip.point, 0.05f * sc);
                }
                if (frontray)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(hit_Chest.point, 0.05f * sc);
                }
            }
        }
#endif
    }
}