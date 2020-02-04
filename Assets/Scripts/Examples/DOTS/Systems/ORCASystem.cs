using ORCA.Components;
using ORCA.DOTS;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.Systems
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [DisableAutoCreation]
    public class ORCASystem : ComponentSystem
    {
        private EntityQuery query;
        float simulatorTimeStep;
        float maxDistance;
        
        public ORCASystem(float simulatorTimeStep, float maxDistance)
        {
            this.simulatorTimeStep = simulatorTimeStep;
            this.maxDistance = maxDistance;
        }
        
        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(Translation), typeof(NavigationData));
        }

        protected override void OnUpdate()
        {
            var agentData = query.ToComponentDataArray<NavigationData>(Allocator.TempJob);
            var translationData = query.ToComponentDataArray<Translation>(Allocator.TempJob);
            var entityArray = query.ToEntityArray(Allocator.TempJob);
            Entities.ForEach((Entity entity, ref Translation translation, ref NavigationData data) =>
            {
                ORCAVelocityCalculationDOTS.ComputeNewVelocity(ref data, translation, entity, agentData, translationData, entityArray, 
                    simulatorTimeStep, maxDistance );
                
                agentData.Dispose();
                translationData.Dispose();
                entityArray.Dispose();
            });
        }
    }
}