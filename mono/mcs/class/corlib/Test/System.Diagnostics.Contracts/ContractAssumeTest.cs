#define CONTRACTS_FULL
#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using MonoTests.System.Diagnostics.Contracts.Helpers;
using NUnit.Framework;

namespace MonoTests.System.Diagnostics.Contracts
{
    [TestFixture]
    public class ContractAssumeTest : TestContractBase
    {
        /// <summary>
        /// At runtime Contract.Assume() acts just like a Contract.Assert(), except the exact message in the assert
        /// or exception is slightly different.
        /// </summary>
        [Test]
        //[Ignore ("This causes NUnit crash on .NET 4.0")]
        public void TestAssumeMessage()
        {
            try
            {
                Contract.Assume(false);
                Assert.Fail("TestAssumeMessage() exception not thrown #1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Assumption failed.", ex.Message);
            }

            try
            {
                Contract.Assume(false, "Message");
                Assert.Fail("TestAssumeMessage() exception not thrown #1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Assumption failed.  Message", ex.Message);
            }
        }

        // Identical to Contract.Assert, so no more testing required.
    }
}
