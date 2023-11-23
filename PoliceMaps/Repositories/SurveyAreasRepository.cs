using Microsoft.EntityFrameworkCore;
using PoliceMaps.Contexts;
using PoliceMaps.Entities;
using PoliceMaps.Helpers;
using PoliceMaps.Models.DTOs;
using PoliceMaps.Repositories.Generic;

namespace PoliceMaps.Repositories
{
    public interface ISurveyAreasRepository : IGenericRepository<SurveyArea>
    {
    }

    public class SurveyAreasRepository : GenericRepository<SurveyArea, MapsDbContext>, ISurveyAreasRepository
    {
        public SurveyAreasRepository(MapsDbContext context) :
            base(context)
        {
        }
    }
}
