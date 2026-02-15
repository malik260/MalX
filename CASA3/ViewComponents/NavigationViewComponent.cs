using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CASA3.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IMemoryCache _cache;
        private readonly IProjectService _projectService;
        private readonly ILogger<NavigationViewComponent> _logger;

        public NavigationViewComponent(
            IMemoryCache cache,
            IProjectService projectService,
            ILogger<NavigationViewComponent> logger)
        {
            _cache = cache;
            _projectService = projectService;
            _logger = logger;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var projects = _cache.GetOrCreate("Projects", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    var result = _projectService.GetAllProjects() ?? new List<ProjectDto>();
                    _logger.LogInformation("NavigationViewComponent: Loaded {Count} projects from service", result.Count);
                    return result;
                });

                _logger.LogInformation("NavigationViewComponent: Returning {Count} projects to view", projects.Count);
                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NavigationViewComponent: Error loading projects");
                return View(new List<ProjectDto>());
            }
        }
    }
}