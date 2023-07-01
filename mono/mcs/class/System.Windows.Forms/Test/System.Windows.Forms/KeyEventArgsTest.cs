//
//
// Author:
//   Rolf Bjarne Kvinge  (RKvinge@novell.com)
//
// (C) 2007 Novell, Inc. (http://www.novell.com)
//

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;

namespace MonoTests.System.Windows.Forms
{
    [TestFixture]
    public class KeyEventArgsTest : TestHelper
    {
        [Test]
        public void SuppressKeyPressTest()
        {
            KeyEventArgs kea = new KeyEventArgs(Keys.L);

            Assert.IsFalse(kea.SuppressKeyPress, "#01");
            Assert.IsFalse(kea.Handled, "#02");

            kea.SuppressKeyPress = true;

            Assert.IsTrue(kea.SuppressKeyPress, "#03");
            Assert.IsTrue(kea.Handled, "#04");

            kea.SuppressKeyPress = false;

            Assert.IsFalse(kea.SuppressKeyPress, "#05");
            Assert.IsFalse(kea.Handled, "#06");
        }
    }
}
