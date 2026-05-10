//
// MonoTests.System.Web.Services.Discovery.ContractReferenceTest.cs
//
// Author:
//   Tim Coleman (tim@timcoleman.com)
//   Dave Bettin (dave@opendotnet.com)
//
// Copyright (C) Tim Coleman, 2002
// Copyright (C) Dave Bettin, 2003
//

using System;
using System.Web.Services.Discovery;
using NUnit.Framework;

namespace MonoTests.System.Web.Services.Discovery
{
    [TestFixture]
    public class ContractReferenceTest
    {
        [Test]
        public void TestConstructors()
        {
            ContractReference contractReference;
        }

        [Test]
        public void TestConstants()
        {
            Assert.AreEqual("http://schemas.xmlsoap.org/disco/scl/", ContractReference.Namespace);
        }
    }
}
