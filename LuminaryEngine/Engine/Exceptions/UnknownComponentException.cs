namespace LuminaryEngine.Engine.Exceptions;

public class UnknownComponentException : Exception
{
    public UnknownComponentException() : base()
    {
    }

    public UnknownComponentException(string message) : base(message)
    {
    }

    public UnknownComponentException(string message, System.Exception inner) : base(message, inner)
    {
    }

    protected UnknownComponentException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}