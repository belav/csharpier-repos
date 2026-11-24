// <OWNER>Microsoft</OWNER>
namespace System.CodeDom
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    [ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), Serializable]
    public class CodeDefaultValueExpression : CodeExpression
    {
        private CodeTypeReference type;

        public CodeDefaultValueExpression() { }

        public CodeDefaultValueExpression(CodeTypeReference type)
        {
            this.type = type;
        }

        public CodeTypeReference Type
        {
            get
            {
                if (type == null)
                {
                    type = new CodeTypeReference("");
                }
                return type;
            }
            set { type = value; }
        }
    }
}
