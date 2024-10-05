//
// MonoTests.System.Web.Services.Description.PortTypeCollectionTest.cs
//
// Author:
//   Erik LeBel <eriklebel@yahoo.ca>
//
// (C) 2003 Erik LeBel
//

using System;
using System.Web.Services.Description;
using NUnit.Framework;

namespace MonoTests.System.Web.Services.Description
{
    [TestFixture]
    public class PortTypeCollectionTest
    {
        PortTypeCollection ptc;

        [SetUp]
        public void InitializePortTypeCollection()
        {
            // workaround for internal constructor
            ServiceDescription desc = new ServiceDescription();
            ptc = desc.PortTypes;
        }

        [Test]
        public void TestDefaultProperties()
        {
            Assert.IsNull(ptc["hello"]);
            Assert.AreEqual(0, ptc.Count);
        }

        [Test]
        public void TestAddPortType()
        {
            const string portTypeName = "testPortType";

            PortType p = new PortType();
            p.Name = portTypeName;

            ptc.Add(p);

            Assert.AreEqual(1, ptc.Count);
            Assert.AreEqual(p, ptc[portTypeName]);
        }
    }
}
