using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IPartnerService
    {
        List<PartnerVM> GetAllPartnersService();
        Task<HeplerResponseVM> CreatePartnerService(PartnerDto model, string logoUrl);
        Task<PartnerVM> GetPartnerByIdMain(string id);
        Task<HeplerResponseVM> GetPartnerByIdService(string id);
        Task<HeplerResponseVM> UpdatePartnerService(PartnerUpdateDto model, string? logoUrl);
        Task<HeplerResponseVM> DeletePartnerByIdService(string id);
        Task<HeplerResponseVM> TogglePartnerStatusService(string id);
    }
}
