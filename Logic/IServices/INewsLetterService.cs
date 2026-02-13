using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface INewsLetterService
    {
        List<NewsLetterVM> GetAllNewsLetterService();
        Task<HeplerResponseVM> CreateNewsLetterservice(NewsletterDto model, string coverImgUrl, string documentUrl);
        Task<NewsLetterVM> GetNewsLetterIdMain(string id);
        Task<HeplerResponseVM> GetNewsLetterByIdService(string id);
        Task<HeplerResponseVM> UpdateNewsLetterService(NewsletterUpdateDto model, string? coverImgUrl, string? documentUrl);
        Task<HeplerResponseVM> DeleteNewsLetterByIdService(string id);
    }
}
