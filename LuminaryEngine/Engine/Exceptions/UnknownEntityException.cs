namespace LuminaryEngine.Engine.Exceptions;

public class UnknownEntityException : Exception
{
    public UnknownEntityException() : base()
    {
    }

    public UnknownEntityException(string message) : base(message)
    {
    }

    public UnknownEntityException(string message, System.Exception inner) : base(message, inner)
    {
    }

    protected UnknownEntityException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}