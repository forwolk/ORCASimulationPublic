using DOTS.Components;
using ORCA.Components;
using ORCA.Extensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public partial class MoveToDestinationSystem
    {
        private const int SENSOR_ARC_ANGLE = 85;

        private static void FindClosestAgent(
            NativeArray<Entity> Entities,
            NativeArray<NavigationData> Agents,
            NativeArray<Translation> Translations,
            Entity entity, ref NavigationData navigationData, 
            [ReadOnly] ref DestinationData destinationData, [ReadOnly] ref Translation translation, 
            bool mustBeStatic,
            out NavigationData closestAgent, out float2 closestAgentPosition, out float distanceSquared)
        {
            closestAgent = default;
            distanceSquared = default;
            closestAgentPosition = default;
            
            float maxDistance = int.MaxValue;
            for (var i = 0; i < Agents.Length; ++i)
            {
                if (Entities[i] == entity)
                {
                    continue;
                }

                if (mustBeStatic)
                {
                    var relativePositionVector = (Translations[i].Value - translation.Value).xy;
                    var angle = Vector2.Angle(navigationData.Velocity, relativePositionVector);
                    var agentLiesInFrontOfTheObject = math.abs(angle) < SENSOR_ARC_ANGLE;
                    var agentSeemsToBeStatic = Agents[i].IsStatic && agentLiesInFrontOfTheObject;
                    if (!agentSeemsToBeStatic)
                    {
                        continue;
                    }
                }

                distanceSquared = (Translations[i].Value - translation.Value).xy.LengthSquared();
                if (distanceSquared < maxDistance)
                {
                    closestAgent = Agents[i];
                    closestAgentPosition = Translations[i].Value.xy;
                    maxDistance = distanceSquared;
                }
            }
        }
    }
}