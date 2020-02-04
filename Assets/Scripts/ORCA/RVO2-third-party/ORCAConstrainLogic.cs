using ORCA.Components;
using ORCA.DS;
using ORCA.Extensions;
using Unity.Mathematics;

namespace ORCA
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

    public static class ORCAConstrainLogic
    {
        public static Line CalculateConstrain(NavigationData agent, NavigationData other, 
            float2 otherPosition, float2 agentPosition, float invTimeHorizon, float simulatorTimeStep)
        {
           var otherVelocity = !other.IsStatic ? other.Velocity : float2.zero;

                float2 relativePosition = otherPosition - agentPosition;
                float2 relativeVelocity = agent.Velocity - otherVelocity;
                float distSq = relativePosition.LengthSquared();
                float combinedRadius = agent.Radius + other.Radius;
                float combinedRadiusSq = combinedRadius.Square();

                Line line;
                float2 u;

                if (distSq > combinedRadiusSq)
                {
                    /* No collision. */
                    float2 w = relativeVelocity - invTimeHorizon * relativePosition;

                    /* Vector from cutoff center to relative velocity. */
                    float wLengthSq = w.LengthSquared();
                    float dotProduct1 = math.dot(w, relativePosition);

                    if (dotProduct1 < 0.0f && dotProduct1.Square() > combinedRadiusSq * wLengthSq)
                    {
                        /* Project on cut-off circle. */
                        float wLength = math.sqrt(wLengthSq);
                        float2 unitW = w / wLength;

                        line.direction = new float2(unitW.y, -unitW.x);
                        u = (combinedRadius * invTimeHorizon - wLength) * unitW;
                    }
                    else
                    {
                        /* Project on legs. */
                        float leg = math.sqrt(distSq - combinedRadiusSq);

                        if ( relativePosition.CrossScalar( w ) > 0.0f)
                        {
                            /* Project on left leg. */
                            line.direction = new float2(relativePosition.x * leg - relativePosition.y * combinedRadius, relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
                        }
                        else
                        {
                            /* Project on right leg. */
                            line.direction = -new float2(relativePosition.x * leg + relativePosition.y * combinedRadius, -relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
                        }

                        float dotProduct2 = math.dot(relativeVelocity, line.direction);
                        u = dotProduct2 * line.direction - relativeVelocity;
                    }
                }
                else
                {
                    /* Collision. Project on cut-off circle of time timeStep. */
                    float invTimeStep = 1.0f / simulatorTimeStep;

                    /* Vector from cutoff center to relative velocity. */
                    float2 w = relativeVelocity - invTimeStep * relativePosition;

                    float wLength = math.length(w);
                    float2 unitW = w / wLength;

                    line.direction = new float2(unitW.y, -unitW.x);
                    u = (combinedRadius * invTimeStep - wLength) * unitW;
                }
                
                var avoidanceFactor = !other.IsStatic ? 0.5f : 1f;

                line.point = agent.Velocity + avoidanceFactor * u;

                return line;
        }
    }
}