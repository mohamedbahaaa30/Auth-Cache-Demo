namespace AuthDemo.Dtos
{
    public class RegisterResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }

        // Helper methods for easy creation
        public static RegisterResultDto SuccessResult(string message = "Operation successful", string? userId = null)
            => new RegisterResultDto { Success = true, Message = message, UserId = userId };

        public static RegisterResultDto FailureResult(string message)
            => new RegisterResultDto { Success = false, Message = message };
    }
}
