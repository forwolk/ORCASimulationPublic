using ORCA.Components;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [DisableAutoCreation]
    public class ColorAgentsSystem : ComponentSystem
    {
        public Material DynamicMaterial;
        public Material StaticMaterial;
        
        public ColorAgentsSystem(Material dynamicMaterial, Material staticMaterial)
        {
            DynamicMaterial = dynamicMaterial;
            StaticMaterial = staticMaterial;
        }
        
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref Translation translation, ref NavigationData navigationData) =>
            {
                var material = navigationData.IsStatic ? StaticMaterial : DynamicMaterial;
                var data = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                data.material = material;
                EntityManager.SetSharedComponentData(entity, data);
            });
        }
    }
}