﻿namespace Vhc.Optimus.Core.Abstractions
{
    public interface IResult
    {
        int RecordsAffected { get; set; }
        string Message { get; set; }
        bool Success { get; set; }
    }
}