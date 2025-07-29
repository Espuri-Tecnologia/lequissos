namespace LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default
{
    public interface IProcessedOrdersTracker
    {
        bool IsProcessed(string orderNumber);
        void MarkProcessed(string orderNumber);
    }
}
