using System.Threading.Tasks;

public interface IAuthService
{
    Task<SignUpResponseDTO> SignUp(SignUpRequestDTO request);
    Task<SignInResponseDTO> SignIn(SignInRequestDTO request);

    Task<NewAccessTokenDTO> GetNewAccessToken(string refreshTokenValue);
}