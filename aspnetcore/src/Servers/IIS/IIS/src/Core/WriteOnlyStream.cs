// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Server.IIS.Core
{
    /// <summary>
    /// A <see cref="Stream"/> which only allows for writes.
    /// </summary>
    [Obsolete("The WriteOnlyStream is obsolete and will be removed in a future release.")] // Remove after .NET 6.
    public abstract class WriteOnlyStream : Stream
    {
        ///<inheritdoc/>
        public override bool CanRead => false;

        ///<inheritdoc/>
        public override bool CanWrite => true;

        ///<inheritdoc/>
        public override int ReadTimeout
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        ///<inheritdoc/>
        public override bool CanSeek => false;

        ///<inheritdoc/>
        public override long Length => throw new NotSupportedException();

        ///<inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        ///<inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
