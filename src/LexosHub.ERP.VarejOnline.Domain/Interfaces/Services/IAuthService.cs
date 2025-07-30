using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
  public interface IAuthService
  {
    Task<Response<IntegrationDto>> EnableTokenIntegrationAsync(string code);
    Task<string> GetAuthUrl();
  }
}
