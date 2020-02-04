using DOTS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Hybrid
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public class Destination : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform DestinationTransform;
        public float RequiredDistance;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var pos = DestinationTransform.transform.position;
            dstManager.AddComponentData( entity, new DestinationData
            {
                Destination = new float2(pos.x, pos.y), 
                RequiredDistance = RequiredDistance
            });
        }
    }
}