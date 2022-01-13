namespace GatewayService.DTOs
{
#nullable disable
    public class ErrorResponse
    {
        /// <summary>
        /// Error object
        /// </summary>
        public Error Error { get; set; }
    }

    public class Error
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }
    }
}
