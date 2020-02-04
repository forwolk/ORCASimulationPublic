using System.Collections;
using System.Collections.Generic;
using DOTS.Hybrid;
using UnityEditor;

namespace DOTS.Editor
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    [CustomEditor(typeof(NavigationAgent))]
    [CanEditMultipleObjects]
    public class AgentMovementEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var agent = (NavigationAgent) target;
            if (agent.IsStatic)
            {
                agent.Radius = EditorGUILayout.FloatField("Radius", agent.Radius);
                agent.IsStatic = EditorGUILayout.Toggle("Is Static", agent.IsStatic);
            }
            else
            {
                base.OnInspectorGUI();
            }
        }
    }
}

