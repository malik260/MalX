
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IAccountService
    {
        Task<HeplerResponseVM> ChangePasswordService(string details);
        Task CreateSuperAdmin();
    }
}
