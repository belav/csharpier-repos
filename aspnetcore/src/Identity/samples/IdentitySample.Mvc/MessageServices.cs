// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IdentitySamples;

public static class MessageServices
{
    public static Task SendEmailAsync(string email, string subject, string message)
    {
        // Plug in your email service
        return Task.FromResult(0);
    }

    public static Task SendSmsAsync(string number, string message)
    {
        // Plug in your sms service
        return Task.FromResult(0);
    }
}
