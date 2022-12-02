namespace Autoregistrar.Exceptions;

public class NullConfigurationEntryException : Exception
{
    public NullConfigurationEntryException() { }

    public NullConfigurationEntryException(string message) : base(message) { }

    public NullConfigurationEntryException(string message, Exception inner) : base(message, inner)
    { }
}