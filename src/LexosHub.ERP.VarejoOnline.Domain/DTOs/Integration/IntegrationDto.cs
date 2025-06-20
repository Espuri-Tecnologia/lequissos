namespace LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration
{
    public class IntegrationDto
    {
        public int Id { get; set; }
        public int HubIntegrationId { get; set; }
        public int TenantId { get; set; }
        public string? HubKey { get; set; }
        public string? Url { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? LastOrderSyncDate { get; set; }
        public bool HasValidVersion { get; set; }

        /// <summary>
        /// Token de autenticação da integração. Este valor não deve ser retornado
        /// pelas APIs.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// RefreshToken utilizado para obter novo Token. Este valor não deve ser
        /// retornado pelas APIs.
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
