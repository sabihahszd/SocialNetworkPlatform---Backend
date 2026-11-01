using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ISiteSettingsRepository : IBaseRepository<SiteSettings>
    {
        Task<SiteSettings?> GetCurrentAsync();

    }
}
