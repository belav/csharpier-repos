using System.Security.Cryptography.Xml;

namespace Microsoft.AspNetCore.DataProtection.XmlEncryption;

/// <summary>
/// Internal implementation details of <see cref="EncryptedXmlDecryptor"/> for unit testing.
/// </summary>
internal interface IInternalEncryptedXmlDecryptor
{
    void PerformPreDecryptionSetup(EncryptedXml encryptedXml);
}
