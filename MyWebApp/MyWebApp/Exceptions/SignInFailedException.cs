﻿namespace MyWebApp.Exceptions;

public class SignInFailedException : Exception
{
    public SignInFailedException(string message) : base(message) {}
}