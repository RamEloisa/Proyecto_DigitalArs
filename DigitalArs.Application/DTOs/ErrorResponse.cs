using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalArs.Application.DTOs
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Errors { get; set; }
        public string TraceId { get; set; } = string.Empty;
    }
}
