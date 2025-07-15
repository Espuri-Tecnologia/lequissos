using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
    public interface IProdutoService
    {
        Task ProcessWebhookAsync(ProdutoWebhookDto produto, CancellationToken cancellationToken = default);
    }
}
