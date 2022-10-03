using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourGame.Environment
{
    /// <summary>
    /// Obstacles that prevent clear vision for enemies, such as grass and smoke
    /// </summary>
    public class VisionObstacle : MonoBehaviour
    {
        public bool Solid = true;
        [Range(0f, 1f)]
        public float Strength = 0f;
        [HideInInspector]
        public float ActualStrength { get { return 1f - Strength; } } // Strength makes sense in UI but not in physics
        public float CutoffPoint = 1f;
    }
}
