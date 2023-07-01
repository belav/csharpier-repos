using System.Runtime.Serialization;

namespace System.Net.Http.QPack
{
    [Serializable]
    internal sealed class QPackEncodingException : Exception
    {
        public QPackEncodingException(string message)
            : base(message) { }

        public QPackEncodingException(string message, Exception innerException)
            : base(message, innerException) { }

        private QPackEncodingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
