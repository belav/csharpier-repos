// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Functions
{
    public static partial class MathTests
    {
        // Tests MathF.Tan(float) over 5000 iterations for the domain -PI/2, +PI/2

        private const float tanSingleDelta = 0.0004f;
        private const float tanSingleExpectedResult = 1.66717815f;

        public static void TanSingleTest()
        {
            var result = 0.0f; var value = -1.0f;

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                value += tanSingleDelta;
                result += MathF.Tan(value);
            }

            var diff = MathF.Abs(tanSingleExpectedResult - result);

            if (diff > singleEpsilon)
            {
                throw new Exception($"Expected Result {tanSingleExpectedResult,10:g9}; Actual Result {result,10:g9}");
            }
        }
    }
}
