using ORCA.Components;
using ORCA.DOTS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace DOTS.Systems
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [DisableAutoCreation]
    public class ORCASystemJobified : JobComponentSystem
    {
        EntityQuery agents;
        float simulatorTimeStep;
        float maxDistance;
        public ORCASystemJobified(float simulatorTimeStep, float maxDistance)
        {
            this.simulatorTimeStep = simulatorTimeStep;
            this.maxDistance = maxDistance;
        }
        
        protected override void OnCreate()
        {
            agents = GetEntityQuery(typeof(Translation), typeof(NavigationData));
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var entities = agents.ToEntityArray(Allocator.TempJob);
            var translations = agents.ToComponentDataArray<Translation>(Allocator.TempJob);
            var navigation = agents.ToComponentDataArray<NavigationData>(Allocator.TempJob);
            
            var job = new LCASystemJob()
            {    
                Entities = entities,
                Translations = translations,
                Agents = navigation,
                SimulatorTimeStep = simulatorTimeStep,
                MaxDistance = maxDistance
            };

            JobHandle jobHandle = job.Schedule(agents, inputDependencies);
            jobHandle.Complete();
            
            entities.Dispose();
            translations.Dispose();
            navigation.Dispose();
            
            return jobHandle;
        }
    }

    [BurstCompile]
    public struct LCASystemJob : IJobForEachWithEntity<Translation, NavigationData>
    {
        [ReadOnly]
        public NativeArray<Entity> Entities;
        
        [ReadOnly]
        public NativeArray<Translation> Translations;
        
        [ReadOnly]
        public NativeArray<NavigationData> Agents;

        public float SimulatorTimeStep;
        public float MaxDistance;
        
        public void Execute( Entity entity, int index, ref Translation tr, ref NavigationData agent)
        {
            ORCAVelocityCalculationDOTS.ComputeNewVelocity(ref agent, tr, entity, Agents, Translations, Entities, SimulatorTimeStep, MaxDistance);
        }
    }
}