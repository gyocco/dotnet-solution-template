namespace DemoProject.Services.Shared.Exceptions;

public class UnexpectedErrorException : Exception
{
  public UnexpectedErrorException(string message) : base(message)
  {
  }
}
