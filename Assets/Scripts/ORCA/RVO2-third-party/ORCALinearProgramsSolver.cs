using ORCA.DS;
using ORCA.Extensions;
using Unity.Collections;
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
    
    public static class ORCALinearProgramsSolver
    {
        internal const float EPSILON = 0.00001f;
        
        public static bool linearProgram1(NativeList<Line> lines, int lineNo, float radius, float2 optVelocity, bool directionOpt, ref float2 result)
        {
            float dotProduct = math.dot(lines[lineNo].point, lines[lineNo].direction);
            float discriminant = dotProduct.Square() + radius.Square() - lines[lineNo].point.LengthSquared();

            if (discriminant < 0.0f)
            {
                /* Max speed circle fully invalidates line lineNo. */
                return false;
            }

            float sqrtDiscriminant = math.sqrt(discriminant);
            float tLeft = -dotProduct - sqrtDiscriminant;
            float tRight = -dotProduct + sqrtDiscriminant;

            for (int i = 0; i < lineNo; ++i)
            {
                float denominator = lines[lineNo].direction.CrossScalar( lines[i].direction );
                float numerator = lines[i].direction.CrossScalar( lines[lineNo].point - lines[i].point );

                if (math.abs(denominator) <= EPSILON)
                {
                    /* Lines lineNo and i are (almost) parallel. */
                    if (numerator < 0.0f)
                    {
                        return false;
                    }

                    continue;
                }

                float t = numerator / denominator;

                if (denominator >= 0.0f)
                {
                    /* Line i bounds line lineNo on the right. */
                    tRight = math.min(tRight, t);
                }
                else
                {
                    /* Line i bounds line lineNo on the left. */
                    tLeft = math.max(tLeft, t);
                }

                if (tLeft > tRight)
                {
                    return false;
                }
            }

            if (directionOpt)
            {
                /* Optimize direction. */
                var dot = math.dot(optVelocity, lines[lineNo].direction);
                if (dot > 0.0f)
                {
                    /* Take right extreme. */
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                }
                else
                {
                    /* Take left extreme. */
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
            }
            else
            {
                /* Optimize closest point. */
                float t = math.dot(lines[lineNo].direction, (optVelocity - lines[lineNo].point));

                if (t < tLeft)
                {
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
                else if (t > tRight)
                {
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                }
                else
                {
                    result = lines[lineNo].point + t * lines[lineNo].direction;
                }
            }

            return true;
        }
        
        public static int linearProgram2(NativeList<Line> lines, float radius, float2 optVelocity, bool directionOpt, ref float2 result)
        {
            if (directionOpt)
            {
                /*
                 * Optimize direction. Note that the optimization velocity is of
                 * unit length in this case.
                 */
                result = optVelocity * radius;
            }
            else if ( optVelocity.LengthSquared() > radius.Square() )
            {
                /* Optimize closest point and outside circle. */
                result = math.normalize(optVelocity) * radius;
            }
            else
            {
                /* Optimize closest point and inside circle. */
                result = optVelocity;
            }

            for (int i = 0; i < lines.Length; ++i)
            {
                if ( lines[i].direction.CrossScalar( lines[i].point - result ) > 0.0f)
                {
                    /* Result does not satisfy constraint i. Compute new optimal result. */
                    float2 tempResult = result;
                    if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
                    {
                        result = tempResult;

                        return i;
                    }
                }
            }

            return lines.Length;
        }
        
        public static void linearProgram3(NativeList<Line> lines, int beginLine, float radius, ref float2 result)
        {
            float distance = 0.0f;

            for (int i = beginLine; i < lines.Length; ++i)
            {
                if ( lines[i].direction.CrossScalar( lines[i].point - result ) > distance)
                {
                    /* Result does not satisfy constraint of line i. */
                    NativeList<Line> projLines = new NativeList<Line>(1, Allocator.Temp);

                    for (int j = 0; j < i; ++j)
                    {
                        Line line;

                        float determinant = lines[i].direction.CrossScalar( lines[j].direction );

                        if (math.abs(determinant) <= EPSILON)
                        {
                            /* Line i and line j are parallel. */
                            var dot = math.dot(lines[i].direction, lines[j].direction);
                            if ( dot > 0.0f)
                            {
                                /* Line i and line j point in the same direction. */
                                continue;
                            }
                            else
                            {
                                /* Line i and line j point in opposite direction. */
                                line.point = 0.5f * (lines[i].point + lines[j].point);
                            }
                        }
                        else
                        {
                            line.point = lines[i].point + ( lines[j].direction.CrossScalar( lines[i].point - lines[j].point ) / determinant) * lines[i].direction;
                        }

                        line.direction = math.normalize(lines[j].direction - lines[i].direction);
                        projLines.Add(line);
                    }

                    float2 tempResult = result;
                    if (linearProgram2(projLines, radius, new float2(-lines[i].direction.y, lines[i].direction.x), true, ref result) < projLines.Length)
                    {
                        /*
                         * This should in principle not happen. The result is by
                         * definition already in the feasible region of this
                         * linear program. If it fails, it is due to small
                         * floating point error, and the current result is kept.
                         */
                        result = tempResult;
                    }

                    projLines.Dispose();
                    distance = lines[i].direction.CrossScalar( lines[i].point - result );
                }
            }
        }

    }
}