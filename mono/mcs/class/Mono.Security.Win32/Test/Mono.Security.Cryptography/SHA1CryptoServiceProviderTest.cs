//
// SHA1CryptoServiceProviderTest.cs - NUnit Test Cases for SHA1 (FIPS186)
//
// Author:
//	Sebastien Pouliot (spouliot@motus.com)
//
// (C) 2003 Motus Technologies Inc. (http://www.motus.com)
//

using System;
using Mono.Security.Cryptography;
using NUnit.Framework;

namespace MonoTests.Security.Cryptography
{
    [TestFixture]
    public class SHA1CryptoServiceProviderTest : SHA1Test
    {
        [SetUp]
        public void Setup()
        {
            hash = new SHA1CryptoServiceProvider();
        }

        // this will run ALL tests defined in SHA1Test.cs with the SHA1CryptoServiceProvider implementation
    }
}
