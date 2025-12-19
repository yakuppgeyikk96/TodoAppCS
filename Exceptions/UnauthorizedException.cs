namespace FirstWebApi.Exceptions;

public class UnauthorizedException(string message) : Exception(message)
{
}
