namespace BoricaNet.Exceptions;

public class BoricaNetException : Exception
{
    public BoricaNetException() { }

    public BoricaNetException(string message) : base(message) { }

    public BoricaNetException(string message, Exception innerException)
        : base(message, innerException) { }
}