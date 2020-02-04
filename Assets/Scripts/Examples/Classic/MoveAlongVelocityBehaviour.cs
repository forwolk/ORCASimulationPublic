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
    public class MoveAlongVelocityBehaviour : MonoBehaviour
    {
        private NavigationBehaviour agent;

        void Start()
        {
            agent = GetComponent<NavigationBehaviour>();
        }

        void Update()
        {
            var offset = agent.Data.Velocity * Time.deltaTime;
            transform.position += new Vector3(offset.x, offset.y, transform.position.z);
        }
    }
}