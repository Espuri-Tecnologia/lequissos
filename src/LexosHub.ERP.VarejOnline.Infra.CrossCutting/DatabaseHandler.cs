using Microsoft.Extensions.Configuration;

namespace LexosHub.ERP.VarejOnline.Infra.CrossCutting
{
    public class DatabaseHandler
    {
        public static string MontarConexao(IConfiguration configuration)
        {
            return $"Server={configuration["Databases:VarejoOnline:Host"]};Database={configuration["Databases:VarejoOnline:DatabaseName"]};User Id={configuration["Databases:VarejoOnline:User"]}; Password={configuration["Databases:VarejoOnline:Password"]};TrustServerCertificate=true;";
        }
    }
}
