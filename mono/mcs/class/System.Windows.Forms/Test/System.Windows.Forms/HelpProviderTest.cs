using System;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.System.Windows.Forms
{
    [TestFixture]
    public class HelpProviderTest : TestHelper
    {
        [Test]
        public void HelpProviderPropertyTag()
        {
            HelpProvider md = new HelpProvider();
            object s = "MyString";

            Assert.AreEqual(null, md.Tag, "A1");

            md.Tag = s;
            Assert.AreSame(s, md.Tag, "A2");
        }
    }
}
