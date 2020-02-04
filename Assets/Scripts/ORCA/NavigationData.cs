using System;
using Unity.Entities;
using Unity.Mathematics;

namespace ORCA.Components
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [Serializable]
    public struct NavigationData : IComponentData
    {
        public float2 PreferredVelocity;
        [NonSerialized]
        public float2 Velocity;
        public float Radius;
        public float MaxSpeed;
        public float TimeHorizon;
        public bool IsStatic;
    }
}