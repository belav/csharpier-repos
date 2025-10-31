//------------------------------------------------------------------------------
// <copyright file="StringOutput.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Text;
    using System.Xml;
    using Res = System.Xml.Utils.Res;

    internal class StringOutput : SequentialOutput
    {
        private StringBuilder builder;
        private string result;

        internal string Result
        {
            get { return this.result; }
        }

        internal StringOutput(Processor processor)
            : base(processor)
        {
            this.builder = new StringBuilder();
        }

        internal override void Write(char outputChar)
        {
            this.builder.Append(outputChar);

#if DEBUG
            this.result = this.builder.ToString();
#endif
        }

        internal override void Write(string outputText)
        {
            this.builder.Append(outputText);

#if DEBUG
            this.result = this.builder.ToString();
#endif
        }

        internal override void Close()
        {
            this.result = this.builder.ToString();
        }
    }
}
