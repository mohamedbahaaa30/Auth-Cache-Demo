namespace AuthDemo.Dtos
{
    public class RefreshTokenResponseDto
    {
        public string AccessTokne { get; set; }
        public string ResreshToken { get; set; }
        public bool IsSuccess {  get; set; }
        public DateTime AccessTokenExpiration { get; set; }

        public static RefreshTokenResponseDto Success(string accessTokne, string resreshToken)
            => new RefreshTokenResponseDto { AccessTokne = accessTokne, ResreshToken = resreshToken, IsSuccess = true };

        public static RefreshTokenResponseDto Failure()
            => new RefreshTokenResponseDto { AccessTokne = string.Empty, ResreshToken = string.Empty, IsSuccess = false };
    }
    
}
