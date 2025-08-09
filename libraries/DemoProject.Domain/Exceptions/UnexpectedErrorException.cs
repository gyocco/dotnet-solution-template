using System;

namespace DemoProject.Domain.Exceptions;

public class UnexpectedErrorException : Exception
{
  public UnexpectedErrorException(string message) : base(message)
  {
  }
}
