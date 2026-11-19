//
// System.ResolveEventArgs Test Cases
//
// Author: Nick Drochak <ndrochak@gol.com>
//

using System;
using NUnit.Framework;

namespace MonoTests.System
{
    [TestFixture]
    public class ResolveEventArgsTest
    {
        public ResolveEventArgsTest() { }

        [Test]
        public void TestTheWholeThing()
        {
            ResolveEventArgs REA = new ResolveEventArgs("REA_Name");
            Assert.AreEqual(REA.Name, "REA_Name", "Name property not correct");
        }
    }
}
