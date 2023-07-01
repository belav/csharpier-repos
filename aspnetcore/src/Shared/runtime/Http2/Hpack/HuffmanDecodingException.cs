using System.Runtime.Serialization;

namespace System.Net.Http.HPack
{
    // TODO: Should this be public?
    [Serializable]
    internal sealed class HuffmanDecodingException : Exception, ISerializable
    {
        public HuffmanDecodingException() { }

        public HuffmanDecodingException(string message)
            : base(message) { }

        private HuffmanDecodingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        void ISerializable.GetObjectData(
            SerializationInfo serializationInfo,
            StreamingContext streamingContext
        )
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }

        public override void GetObjectData(
            SerializationInfo serializationInfo,
            StreamingContext streamingContext
        )
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }
    }
}
