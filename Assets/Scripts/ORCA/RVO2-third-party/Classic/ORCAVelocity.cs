using ORCA.Components;
using ORCA.DS;
using ORCA.Extensions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace ORCA.Classic
{
#region -Copyright
    /*
    * Agent.cs
    * RVO2 Library C#
    *
    * Copyright 2008 University of North Carolina at Chapel Hill
    *
    * Licensed under the Apache License, Version 2.0 (the "License");
    * you may not use this file except in compliance with the License.
    * You may obtain a copy of the License at
    *
    *     http://www.apache.org/licenses/LICENSE-2.0
    *
    * Unless required by applicable law or agreed to in writing, software
    * distributed under the License is distributed on an "AS IS" BASIS,
    * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    * See the License for the specific language governing permissions and
    * limitations under the License.
    *
    * Please send all bug reports to <geom@cs.unc.edu>.
    *
    * The authors may be contacted via:
    *
    * Jur van den Berg, Stephen J. Guy, Jamie Snape, Ming C. Lin, Dinesh Manocha
    * Dept. of Computer Science
    * 201 S. Columbia St.
    * Frederick P. Brooks, Jr. Computer Science Bldg.
    * Chapel Hill, N.C. 27599-3175
    * United States of America
    *
    * <http://gamma.cs.unc.edu/RVO2/>
    */
    
    
    /* Modifications Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
#endregion

    public static class ORCAVelocity
    {
        public static void Compute(NavigationBehaviour navigation, NavigationBehaviour[] agentNeighbors, float simulatorTimeStep, float maxDistance)
        {
            NativeList<Line> orcaLines = new NativeList<Line>(1, Allocator.Temp);

            float invTimeHorizon = 1.0f / navigation.Data.TimeHorizon;
            float maxDistanceSq = maxDistance * maxDistance;

            Vector2 agentPosition = navigation.transform.position;

            /* Create agent ORCA lines. */
            for (int i = 0; i < agentNeighbors.Length; ++i)
            {
                NavigationData other = agentNeighbors[i].Data;
                Vector2 otherPosition = agentNeighbors[i].transform.position;

                if (navigation == agentNeighbors[i])
                {
                    continue;
                }

                var distanceSqr = (agentPosition - otherPosition).sqrMagnitude;
                if (distanceSqr > maxDistanceSq)
                {
                    continue;
                }

                var line = ORCAConstrainLogic.CalculateConstrain(navigation.Data, other, otherPosition, agentPosition, invTimeHorizon, simulatorTimeStep);
                orcaLines.Add(line);
            }

            int lineFail = ORCALinearProgramsSolver.linearProgram2(orcaLines, navigation.Data.MaxSpeed, navigation.Data.PreferredVelocity, false, ref navigation.Data.Velocity);

            if (lineFail < orcaLines.Length)
            {
                ORCALinearProgramsSolver.linearProgram3(orcaLines, lineFail, navigation.Data.MaxSpeed, ref navigation.Data.Velocity);
            }

            orcaLines.Dispose();
        }
    }
}