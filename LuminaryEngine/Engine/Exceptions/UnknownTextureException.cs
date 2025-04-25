namespace LuminaryEngine.Engine.Exceptions;

public class UnknownTextureException : Exception
{
    public UnknownTextureException() : base()
    {
    }

    public UnknownTextureException(string message) : base(message)
    {
    }

    public UnknownTextureException(string message, System.Exception inner) : base(message, inner)
    {
    }

    protected UnknownTextureException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}