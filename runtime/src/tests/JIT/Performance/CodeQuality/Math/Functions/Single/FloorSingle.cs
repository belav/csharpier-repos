// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Functions
{
    public static partial class MathTests
    {
        // Tests MathF.Floor(float) over 5000 iterations for the domain -1, +1

        private const float floorSingleDelta = 0.0004f;
        private const float floorSingleExpectedResult = -2498.0f;

        public static void FloorSingleTest()
        {
            var result = 0.0f; var value = -1.0f;

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                value += floorSingleDelta;
                result += MathF.Floor(value);
            }

            var diff = MathF.Abs(floorSingleExpectedResult - result);

            if (diff > singleEpsilon)
            {
                throw new Exception($"Expected Result {floorSingleExpectedResult,10:g9}; Actual Result {result,10:g9}");
            }
        }
    }
}
