//
// CodeMemberFieldTest.cs - NUnit Test Cases for System.CodeDom.CodeMemberField
//
// Authors:
//   Gert Driesen (drieseng@users.sourceforge.net)
//
// (C) 2005 Novell
//

using System.CodeDom;
using NUnit.Framework;

namespace MonoTests.System.CodeDom
{
    [TestFixture]
    public class CodeMemberPropertyTest
    {
        [Test]
        public void DefaultType()
        {
            CodeMemberProperty property = new CodeMemberProperty();
            Assert.IsNotNull(property.Type, "#1");
            Assert.AreEqual(
                new CodeTypeReference(string.Empty).BaseType,
                property.Type.BaseType,
                "#2"
            );
        }
    }
}
