using ORCA.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Systems
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [DisableAutoCreation]
    public class MoveAlongVelocitySystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var moveForwardRotationJob = new MoveForwardRotation
            {
                dt = Time.DeltaTime
            };

            return moveForwardRotationJob.Schedule(this, inputDeps);
        }
        
        [BurstCompile]
        struct MoveForwardRotation : IJobForEach<Translation, NavigationData>
        {
            public float dt;

            public void Execute(ref Translation pos, [ReadOnly] ref NavigationData navigationData)
            {
                if (navigationData.IsStatic)
                {
                    return;
                }
                pos.Value = new float3(pos.Value.xy + dt * navigationData.Velocity, pos.Value.z);
            }
        }
    }
}