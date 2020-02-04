using ORCA.Classic;
using UnityEngine;

namespace Classic
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [RequireComponent(typeof(NavigationBehaviour))]
    public class MoveToDestinationBehaviour : MonoBehaviour
    {
        private NavigationBehaviour agent;
        public Transform DestinationTransform;
        
        void Start()
        {
            agent = GetComponent<NavigationBehaviour>();
        }
        
        void Update()
        {
            agent.Data.PreferredVelocity = (Vector2) (DestinationTransform.position - transform.position) * agent.Data.MaxSpeed;
        }
    }
}