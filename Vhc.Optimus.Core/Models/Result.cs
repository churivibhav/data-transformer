using Vhc.Optimus.Core.Abstractions;

namespace Vhc.Optimus.Core.Models
{
    public class Result : IResult
    {
        public int RecordsAffected { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}