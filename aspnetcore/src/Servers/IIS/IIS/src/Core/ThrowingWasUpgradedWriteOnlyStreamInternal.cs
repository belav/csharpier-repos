// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Server.IIS.Core
{
    /// <summary>
    /// A <see cref="Stream"/> which throws on calls to write after the stream was upgraded
    /// </summary>
    /// <remarks>
    /// Users should not need to instantiate this class.
    /// </remarks>
    internal class ThrowingWasUpgradedWriteOnlyStreamInternal : WriteOnlyStreamInternal
    {
        ///<inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
            => throw new InvalidOperationException(CoreStrings.ResponseStreamWasUpgraded);

        ///<inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new InvalidOperationException(CoreStrings.ResponseStreamWasUpgraded);

        ///<inheritdoc/>
        public override void Flush()
            => throw new InvalidOperationException(CoreStrings.ResponseStreamWasUpgraded);

        ///<inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        ///<inheritdoc/>
        public override void SetLength(long value)
            => throw new NotSupportedException();
    }
}
