//------------------------------------------------------------------------------
// <copyright file="TextOutput.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Res = System.Xml.Utils.Res;
    using System.Xml.XPath;

    internal class TextOutput : SequentialOutput
    {
        private TextWriter writer;

        internal TextOutput(Processor processor, Stream stream)
            : base(processor)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.encoding = processor.Output.Encoding;
            this.writer = new StreamWriter(stream, this.encoding);
        }

        internal TextOutput(Processor processor, TextWriter writer)
            : base(processor)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            this.encoding = writer.Encoding;
            this.writer = writer;
        }

        internal override void Write(char outputChar)
        {
            this.writer.Write(outputChar);
        }

        internal override void Write(string outputText)
        {
            this.writer.Write(outputText);
        }

        internal override void Close()
        {
            this.writer.Flush();
            this.writer = null;
        }
    }
}
