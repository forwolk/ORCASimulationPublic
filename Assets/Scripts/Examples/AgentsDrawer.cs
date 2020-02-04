using ORCA.Classic;
using ORCA.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Example
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public class AgentsDrawer : MonoBehaviour
    {
        private EntityQuery query;
        
        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if ( !world.IsCreated )
            {
                return;
            }
            var entityManager = world.EntityManager;
            query = entityManager.CreateEntityQuery(typeof(Translation), typeof(NavigationData));
        }

        private void OnDrawGizmos()
        {
            if (query == null)
            {
                return;
            }

            DrawECSAgents();
            DrawClassicAgents();
        }

        private void DrawClassicAgents()
        {
            var agents = FindObjectsOfType<NavigationBehaviour>();
            
            Gizmos.color = Color.cyan;
            
            for ( var i = 0 ; i < agents.Length; ++i)
            {
                var agent = agents[i];
                Gizmos.DrawWireSphere(agent.transform.position, agent.Data.Radius);
                
                if (agent.Data.IsStatic)
                {
                    continue;
                }
                var endPosition = agent.transform.position + (Vector3) (Vector2)agent.Data.Velocity;
                Gizmos.DrawLine(agent.transform.position, endPosition);
            }
        }

        private void DrawECSAgents()
        {
            var agentData = query.ToComponentDataArray<NavigationData>(Allocator.TempJob);
            var translationData = query.ToComponentDataArray<Translation>(Allocator.TempJob);

            Gizmos.color = Color.cyan;
            
            for ( var i = 0 ; i < agentData.Length; ++i)
            {
                var agent = agentData[i];
                var translation = translationData[i];
                Gizmos.DrawWireSphere(new Vector3(translation.Value.x, translation.Value.y), agent.Radius);

                if (agent.IsStatic)
                {
                    continue;
                }
                var endPosition = new Vector3(translation.Value.x, translation.Value.y) + (Vector3) (Vector2)agent.Velocity;
                Gizmos.DrawLine(new Vector3(translation.Value.x, translation.Value.y), endPosition);
            }
            
            agentData.Dispose();
            translationData.Dispose();
        }
    }
}