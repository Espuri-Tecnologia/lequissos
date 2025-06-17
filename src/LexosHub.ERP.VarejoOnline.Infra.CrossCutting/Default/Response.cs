namespace LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default
{
    public class Response<TResponse>
    {
        public TResponse? Result { get; set; }
        public ErrorResult? Error { get; set; }
        public bool IsSuccess
        {
            get
            {
                int result;
                if (Error is { Details: not null })
                {
                    var errors = Error.Details;
                    result = (errors is { Count: 0 } ? 1 : 0);
                }

                if (!string.IsNullOrEmpty(Error?.Message))
                {
                    result = 0;
                }

                else
                {
                    result = 1;
                }

                return (byte)result != 0;
            }

        }

        public Response() { }

        public Response(TResponse result)
        {
            Result = result;
        }

        public static implicit operator Response<TResponse>(TResponse instance)
        {
            return new Response<TResponse>(instance);
        }

        public static implicit operator Response<TResponse?>(ErrorResult error)
        {
            return new Response<TResponse?>(default)
            {
                Error = error
            };
        }
    }
}
