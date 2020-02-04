using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Components
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public struct DestinationData : IComponentData
    {
        public float2 Destination;
        public float RequiredDistance;
    }
}