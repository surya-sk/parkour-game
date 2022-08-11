using System;
using UnityEngine;
namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Set Target")]
    public class SetTargetTask : MTask
    {
        public LookDecision LookDecision;

        public override void StartTask(MAnimalBrain brain, int index)
        {
            LookDecision.Decide(brain, index);
        }
    }
}
