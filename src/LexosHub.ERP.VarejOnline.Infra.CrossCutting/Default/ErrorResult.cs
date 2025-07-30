

using FluentValidation.Results;

namespace LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default
{
    public class ErrorResult
    {
        public string? Message { get; set; }
        public List<ValidationFailure>? Details { get; set; }

        protected ErrorResult() { }

        public ErrorResult(string message)
        {
            Message = message;
        }

        public ErrorResult(string message, List<ValidationFailure> details)
        {
            Message = message;
            Details = details;
        }
    }
}
