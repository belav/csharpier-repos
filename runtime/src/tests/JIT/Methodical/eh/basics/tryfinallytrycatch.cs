// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Try/finally error case
using System;
using Xunit;

namespace hello_tryfinallytrycatch_basics_cs
{
    public class Class1
    {
        private static TestUtil.TestLog testLog;

        static Class1()
        {
            // Create test writer object to hold expected output
            System.IO.StringWriter expectedOut = new System.IO.StringWriter();

            // Write expected output to string writer object
            expectedOut.WriteLine("in Try catch");
            expectedOut.WriteLine("in Try finally");
            expectedOut.WriteLine("in Finally");
            expectedOut.WriteLine("Caught an exception");
            expectedOut.WriteLine("in Catch");

            // Create and initialize test log object
            testLog = new TestUtil.TestLog(expectedOut);
        }

        public static void inTry1()
        {
            Console.WriteLine("in Try catch");
        }

        public static void inTry2()
        {
            Console.WriteLine("in Try finally");
        }

        public static void inCatch()
        {
            Console.WriteLine("in Catch");
        }

        public static void inFinally()
        {
            Console.WriteLine("in Finally");
        }

        [Fact]
        public static int TestEntryPoint()
        {
            //Start recording
            testLog.StartRecording();

            try
            {
                inTry1();
                try
                {
                    inTry2();
                    throw new Exception();
                }
                finally
                {
                    inFinally();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Caught an exception");
                inCatch();
            }

            // stop recoding
            testLog.StopRecording();

            return testLog.VerifyOutput();
        }
    }
}
