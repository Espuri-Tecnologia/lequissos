using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Integration
{
    public interface IIntegrationRepository
    {
        Task<IntegrationDto> AddAsync(IntegrationDto integration);
        Task<IntegrationDto> GetByKeyAsync(string key);
        Task<IntegrationDto> GetByIdAsync(int id);
        Task<IEnumerable<IntegrationDto>> GetActiveIntegrations();
        Task<bool> DeleteAsync(string key);
        Task<IntegrationDto> UpdateAsync(IntegrationDto integration);
        Task<IntegrationDto> GetNextToFirstSync();
        Task<IntegrationDto> GetByIdWithLastOrderDateAsync(int id);
    Task<IntegrationDto> GetByDocument(string cnpj);
  }
}
