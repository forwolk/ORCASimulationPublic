using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace ORCA.Extensions
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public static class Float2Extension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSquared(this float2 vec)
        {
            return math.dot(vec, vec);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CrossScalar(this float2 vector1, float2 vector2)
        {
            return vector1.x * vector2.y - vector1.y * vector2.x;
        }
    }
}