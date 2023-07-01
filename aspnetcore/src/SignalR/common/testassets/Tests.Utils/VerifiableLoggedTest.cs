using System;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;

namespace Microsoft.AspNetCore.SignalR.Tests
{
    public class VerifiableLoggedTest : LoggedTest
    {
        public VerifiableLoggedTest()
        {
            // Ensures this isn't null in case the logged test framework
            // doesn't initialize it correctly.
            LoggerFactory = NullLoggerFactory.Instance;
        }

        public virtual IDisposable StartVerifiableLog(
            Func<WriteContext, bool> expectedErrorsFilter = null
        )
        {
            return CreateScope(expectedErrorsFilter);
        }

        private VerifyNoErrorsScope CreateScope(
            Func<WriteContext, bool> expectedErrorsFilter = null
        )
        {
            return new VerifyNoErrorsScope(
                LoggerFactory,
                wrappedDisposable: null,
                expectedErrorsFilter
            );
        }
    }
}
