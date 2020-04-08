using System.Threading.Tasks;

public class AccountsService : IAccountsService
{
    private readonly IBaseRepository<User> _userRepository;

    public AccountsService(IBaseRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateUserResponseDTO> UpdateUser(UpdateUserRequestDTO request)
    {
        var user = await _userRepository.Get(item => item.EmailAddress == request.Email);

        if(user != null)
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _userRepository.Update(user);

            return new UpdateUserResponseDTO{UserId = user.Guid, LastName = user.LastName};
        }

        return null;
    }
}