namespace AuthDemo.Dtos
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public bool IsSuccess { get; set; }



        public static LoginResponseDto LoginSuccess(string accessToken, string refreshToken, DateTime accessTokenExpiration)
            => new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                IsSuccess = true
            };

        public static LoginResponseDto LoginFailure() => new LoginResponseDto
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                AccessTokenExpiration = DateTime.MinValue,
                IsSuccess = false
            };
    }
}
