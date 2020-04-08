using System.Threading.Tasks;

public interface IAccountsService
{
    Task<UpdateUserResponseDTO> UpdateUser(UpdateUserRequestDTO request);
}