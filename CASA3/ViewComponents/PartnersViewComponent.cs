using Core.ViewModels;
using Logic.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CASA3.ViewComponents
{
    public class PartnersViewComponent : ViewComponent
    {
        private readonly IMemoryCache _cache;
        private readonly IPartnerService _partnerService;
        private readonly ILogger<PartnersViewComponent> _logger;

        public PartnersViewComponent(
            IMemoryCache cache,
            IPartnerService partnerService,
            ILogger<PartnersViewComponent> logger)
        {
            _cache = cache;
            _partnerService = partnerService;
            _logger = logger;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var partners = _cache.GetOrCreate("Partners", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    var allPartners = _partnerService.GetAllPartnersService();
                    var activePartners = allPartners?.Where(p => p.IsActive).ToList() ?? new List<PartnerVM>();

                    _logger.LogInformation("PartnersViewComponent: Loaded {Total} total partners, {Active} active",
                        allPartners?.Count ?? 0, activePartners.Count);

                    return activePartners;
                });

                _logger.LogInformation("PartnersViewComponent: Returning {Count} partners to view", partners.Count);
                return View(partners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PartnersViewComponent: Error loading partners");
                return View(new List<PartnerVM>());
            }
        }
    }
}