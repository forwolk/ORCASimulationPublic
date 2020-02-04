using ORCA.Components;
using UnityEngine;

namespace ORCA.Classic
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public class NavigationBehaviour : MonoBehaviour
    {
        public const int MaxDistance = 10;
        
        public NavigationData Data;
        void Update()
        {
            if (Data.IsStatic)
            {
                return;
            }
            //TODO: For production the recommendation is to use some cache structure (like grid or AABB tree) to get the neighbouring agents quickly
            var neighbourAgents = FindObjectsOfType<NavigationBehaviour>();
            ORCAVelocity.Compute(this, neighbourAgents, 1, MaxDistance );
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying && !Data.IsStatic)
            {
                Gizmos.color = Color.green;

                var endPos = transform.position + (Vector3) (Vector2) Data.PreferredVelocity;

                Gizmos.DrawLine(transform.position, endPos);
            }
        }
    }
}