public class JwtSettingsDTO
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }

    public int TokenLifeTime { get; set; }
    public int RefreshTokenLifeTime { get; set; }
}