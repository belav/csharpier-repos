// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Try catch error case
using System;
using Xunit;

namespace hello_trythrowcatch_basics_cs
{
    public class Class1
    {
        private static TestUtil.TestLog testLog;

        static Class1()
        {
            // Create test writer object to hold expected output
            System.IO.StringWriter expectedOut = new System.IO.StringWriter();

            // Write expected output to string writer object
            expectedOut.WriteLine("In try");
            expectedOut.WriteLine("In catch");

            // Create and initialize test log object
            testLog = new TestUtil.TestLog(expectedOut);
        }

        public static void inTry()
        {
            Console.WriteLine("In try");
            throw new Exception();
        }

        public static void inCatch()
        {
            Console.WriteLine("In catch");
        }

        public static void inFinally() { }

        [Fact]
        public static int TestEntryPoint()
        {
            //Start recording
            testLog.StartRecording();

            try
            {
                inTry();
            }
            catch (Exception)
            {
                inCatch();
            }

            // stop recoding
            testLog.StopRecording();

            return testLog.VerifyOutput();
        }
    }
}
