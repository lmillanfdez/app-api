using System.Threading.Tasks;

public interface IAccountsService
{
    string CreateToken(User user);
}