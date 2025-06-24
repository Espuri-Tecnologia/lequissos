using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
  public interface IAuthService
  {
    Task<Response<IntegrationDto>> EnableTokenIntegrationAsync(string code);
    Task<string> GetAuthUrl();
  }
}
