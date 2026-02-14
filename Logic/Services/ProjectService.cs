using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class ProjectService : IProjectService
    {
        private readonly EFContext _context;
        private readonly ILoggerManager _log;

        public ProjectService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public async Task<HeplerResponseVM> CreateProjectAsync(ProjectCreateDto dto)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                {
                    response.Message = "Project name is required.";
                    return response;
                }

                var project = new Project
                {
                    Name = dto.Name.Trim(),
                    Url = string.IsNullOrWhiteSpace(dto.Url) ? null : dto.Url.Trim(),
                    Slug = string.IsNullOrWhiteSpace(dto.Slug) ? null : dto.Slug.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    HeroImageUrl = string.IsNullOrWhiteSpace(dto.HeroImageUrl) ? null : dto.HeroImageUrl.Trim(),
                    BrochurePdfUrl = string.IsNullOrWhiteSpace(dto.BrochurePdfUrl) ? null : dto.BrochurePdfUrl.Trim(),
                    Year = dto.Year,
                    IsFeatured = dto.IsFeatured,
                    Category = dto.Category
                };

                await _context.Projects.AddAsync(project).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Project added successfully.";
                response.Data = MapToDto(project);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while adding the project.";
                return response;
            }
        }

        public List<ProjectDto> GetAllProjects()
        {
            try
            {
                return _context.Projects
                    .Where(p => !p.IsDeleted)
                    .OrderBy(p => p.Name)
                    .ToList()
                    .Select(MapToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<ProjectDto>();
            }
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(string id)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
                    .ConfigureAwait(false);
                return project == null ? null : MapToDto(project);
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> UpdateProjectAsync(string id, ProjectCreateDto dto)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id) || dto == null)
                {
                    response.Message = "Invalid parameters.";
                    return response;
                }

                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
                    .ConfigureAwait(false);
                if (project == null)
                {
                    response.Message = "Project not found.";
                    return response;
                }

                project.Name = dto.Name.Trim();
                project.Url = string.IsNullOrWhiteSpace(dto.Url) ? null : dto.Url.Trim();
                project.Slug = string.IsNullOrWhiteSpace(dto.Slug) ? null : dto.Slug.Trim();
                project.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
                project.HeroImageUrl = string.IsNullOrWhiteSpace(dto.HeroImageUrl) ? null : dto.HeroImageUrl.Trim();
                project.BrochurePdfUrl = string.IsNullOrWhiteSpace(dto.BrochurePdfUrl) ? null : dto.BrochurePdfUrl.Trim();
                project.Year = dto.Year;
                project.IsFeatured = dto.IsFeatured;
                project.Category = dto.Category;
                project.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Project updated successfully.";
                response.Data = MapToDto(project);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while updating the project.";
                return response;
            }
        }

        public async Task<ProjectDetailsDto?> GetProjectDetailsAsync(string id)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.BuildingDesigns)
                    .FirstOrDefaultAsync(p => (p.Id == id || p.Slug == id) && !p.IsDeleted)
                    .ConfigureAwait(false);
                if (project == null) return null;

                var designs = project.BuildingDesigns
                    .Where(b => !b.IsDeleted)
                    .OrderBy(b => b.Name)
                    .Select(MapBuildingDesignToDto)
                    .ToList();

                return new ProjectDetailsDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    HeroImageUrl = project.HeroImageUrl,
                    Description = project.Description,
                    BrochurePdfUrl = project.BrochurePdfUrl,
                    BuildingDesigns = designs
                };
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> DeleteProjectAsync(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid parameter.";
                    return response;
                }

                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
                    .ConfigureAwait(false);
                if (project == null)
                {
                    response.Message = "Project not found.";
                    return response;
                }

                project.IsDeleted = true;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Project removed successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while removing the project.";
                return response;
            }
        }

        public async Task<HeplerResponseVM> CreateBuildingDesignAsync(string projectId, BuildingDesignCreateDto dto)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(projectId) || dto == null || string.IsNullOrWhiteSpace(dto.Name))
                {
                    response.Message = "Project and unit name are required.";
                    return response;
                }
                var exists = await _context.Projects.AnyAsync(p => p.Id == projectId && !p.IsDeleted).ConfigureAwait(false);
                if (!exists)
                {
                    response.Message = "Project not found.";
                    return response;
                }
                var design = new BuildingDesign
                {
                    ProjectId = projectId,
                    Name = dto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                    ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim(),
                    BrochurePdfUrl = string.IsNullOrWhiteSpace(dto.BrochurePdfUrl) ? null : dto.BrochurePdfUrl.Trim(),
                    FloorPlanPdfUrl = string.IsNullOrWhiteSpace(dto.FloorPlanPdfUrl) ? null : dto.FloorPlanPdfUrl.Trim()
                };
                await _context.BuildingDesigns.AddAsync(design).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Unit added successfully.";
                response.Data = MapBuildingDesignToDto(design);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while adding the unit.";
                return response;
            }
        }

        public async Task<BuildingDesignDto?> GetBuildingDesignByIdAsync(string id)
        {
            try
            {
                var design = await _context.BuildingDesigns
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
                    .ConfigureAwait(false);
                return design == null ? null : MapBuildingDesignToDto(design);
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public List<BuildingDesignDto> GetBuildingDesignsByProjectId(string projectId)
        {
            try
            {
                return _context.BuildingDesigns
                    .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                    .OrderBy(b => b.Name)
                    .ToList()
                    .Select(MapBuildingDesignToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<BuildingDesignDto>();
            }
        }

        public async Task<HeplerResponseVM> UpdateBuildingDesignAsync(string id, BuildingDesignCreateDto dto)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id) || dto == null)
                {
                    response.Message = "Invalid parameters.";
                    return response;
                }
                var design = await _context.BuildingDesigns
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
                    .ConfigureAwait(false);
                if (design == null)
                {
                    response.Message = "Unit not found.";
                    return response;
                }
                design.Name = dto.Name.Trim();
                design.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
                design.Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim();
                design.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim();
                design.BrochurePdfUrl = string.IsNullOrWhiteSpace(dto.BrochurePdfUrl) ? null : dto.BrochurePdfUrl.Trim();
                design.FloorPlanPdfUrl = string.IsNullOrWhiteSpace(dto.FloorPlanPdfUrl) ? null : dto.FloorPlanPdfUrl.Trim();
                design.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Unit updated successfully.";
                response.Data = MapBuildingDesignToDto(design);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while updating the unit.";
                return response;
            }
        }

        public async Task<HeplerResponseVM> DeleteBuildingDesignAsync(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid parameter.";
                    return response;
                }
                var design = await _context.BuildingDesigns
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
                    .ConfigureAwait(false);
                if (design == null)
                {
                    response.Message = "Unit not found.";
                    return response;
                }
                design.IsDeleted = true;
                design.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Unit removed successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while removing the unit.";
                return response;
            }
        }

        private static ProjectDto MapToDto(Project p)
        {
            return new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Url = p.Url,
                Slug = p.Slug,
                Description = p.Description,
                HeroImageUrl = p.HeroImageUrl,
                BrochurePdfUrl = p.BrochurePdfUrl,
                Year = p.Year,
                IsFeatured = p.IsFeatured,
                Category = p.Category
            };
        }

        private static BuildingDesignDto MapBuildingDesignToDto(BuildingDesign b)
        {
            return new BuildingDesignDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Location = b.Location,
                ImageUrl = b.ImageUrl,
                BrochurePdfUrl = b.BrochurePdfUrl,
                FloorPlanPdfUrl = b.FloorPlanPdfUrl
            };
        }
    }
}
