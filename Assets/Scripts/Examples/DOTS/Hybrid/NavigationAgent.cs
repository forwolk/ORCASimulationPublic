using ORCA.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Hybrid
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public class NavigationAgent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Vector2 PreferredVelocity;
        public float Radius;
        public float MaxSpeed;
        public float TimeHorizon;
        public bool IsStatic;
        private void OnDrawGizmos()
        {
            if (IsStatic)
            {
                return;
            }
            
            Gizmos.color = Color.green;
            
            var endPos = transform.position + (Vector3) PreferredVelocity;

            Gizmos.DrawLine(transform.position, endPos);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var movementData = new NavigationData
            {
                PreferredVelocity = new float2(PreferredVelocity.x, PreferredVelocity.y),
                Radius = Radius / 2,
                MaxSpeed = MaxSpeed,
                TimeHorizon = TimeHorizon,
                IsStatic = IsStatic,
            };
            dstManager.AddComponentData(entity, movementData);
        }
    }
}