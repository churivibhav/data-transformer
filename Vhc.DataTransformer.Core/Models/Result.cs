using Vhc.DataTransformer.Core.Abstractions;

namespace Vhc.DataTransformer.Core.Models
{
    public class Result : IResult
    {
        public int RecordsAffected { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}