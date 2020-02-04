using DOTS.Systems;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Hybrid
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public class CreateColoringSystem : MonoBehaviour
    {
        public Material DynamicMaterial;
        public Material StaticMaterial;
        
        void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var group = world.GetOrCreateSystem<SimulationSystemGroup>();
            group.AddSystemToUpdateList(CreateSystem<ColorAgentsSystem>(world, DynamicMaterial, StaticMaterial) );
        }
        
        private T CreateSystem<T>(World world, params object[] args) where T:ComponentSystemBase
        {
            var system = world.CreateSystem<T>(args);
            return system;
        }
    }
}