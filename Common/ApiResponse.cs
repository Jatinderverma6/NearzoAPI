namespace NearzoAPI.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
            => new()
            { Success = true, Data = data };

        public static ApiResponse<T> FailResponse(string errorMessage)
            => new()
            { Success = false, ErrorMessage = errorMessage };
    }

}
