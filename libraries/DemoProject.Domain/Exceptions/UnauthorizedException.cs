using System;

namespace DemoProject.Domain.Exceptions;

public class UnauthorizedException : Exception
{
  public UnauthorizedException(string message) : base(message)
  {
  }
}
