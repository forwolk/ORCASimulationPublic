using System;
using DOTS.Components;
using ORCA.Components;
using ORCA.Extensions;
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
    public partial class MoveToDestinationSystem : JobComponentSystem
    {
        EntityQuery agents;
        
        protected override void OnCreate()
        {
            agents = GetEntityQuery(typeof(Translation), typeof(NavigationData));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var navigation = agents.ToComponentDataArray<NavigationData>(Allocator.TempJob);
            var translations = agents.ToComponentDataArray<Translation>(Allocator.TempJob);
            var entities = agents.ToEntityArray(Allocator.TempJob);
            var moveForwardRotationJob = new MoveToDestinationJob {Agents = navigation, Translations = translations, Entities = entities};

            var jobHandle = moveForwardRotationJob.Schedule(this, inputDeps);
            jobHandle.Complete();
            
            navigation.Dispose();
            translations.Dispose();
            entities.Dispose();
            
            return jobHandle;
        }
        
        [BurstCompile]
        struct MoveToDestinationJob : IJobForEachWithEntity<NavigationData, DestinationData, Translation>
        {
            [ReadOnly]
            public NativeArray<NavigationData> Agents;
            
            [ReadOnly]
            public NativeArray<Translation> Translations;
            
            [ReadOnly]
            public NativeArray<Entity> Entities;
            
            public void Execute(Entity entity, int index, ref NavigationData navigationData, [ReadOnly] ref DestinationData destinationData, [ReadOnly] ref Translation translation)
            {
                var currentDistanceSq = (translation.Value.xy - destinationData.Destination).LengthSquared();
                
                var distVector = destinationData.Destination - translation.Value.xy;
                navigationData.PreferredVelocity = math.normalize(distVector) * navigationData.MaxSpeed;

                var distanceDiff = currentDistanceSq - destinationData.RequiredDistance.Square();
                navigationData.IsStatic = distanceDiff <= 0.01f;
                FindClosestAgent(Entities, Agents, Translations,
                    entity, ref navigationData, ref destinationData, ref translation, false,
                    out var closestAgent, out var closestAgentPosition, out var distanceSquared);

                //If collision is happening with another object we can not stop it
                var combinedRadius = (navigationData.Radius + closestAgent.Radius).Square();
                if (distanceSquared < combinedRadius)
                {
                    navigationData.IsStatic = false;
                }
            }

        }
    }
}