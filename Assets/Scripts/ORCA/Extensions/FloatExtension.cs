using System.Runtime.CompilerServices;

namespace ORCA.Extensions
{
    /* Copyright (C) Anton Trukhan - All Rights Reserved.
     * Free for personal use and experiments.
     * Written permission of the author is required for commercial use or closed-source distribution.
     * Written by Anton Trukhan <anton.truhan@gmail.com>, 2019.
     */
    public static class FloatExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Square(this float v)
        {
            return v * v;
        }
    }
}