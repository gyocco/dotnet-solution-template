namespace DemoProject.Services.Shared.Exceptions;

public class ValidationException : Exception
{
  public ValidationException(string message) : base(message)
  {
  }
}
