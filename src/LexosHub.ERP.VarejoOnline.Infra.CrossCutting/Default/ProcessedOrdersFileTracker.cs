using System;
using System.Collections.Generic;
using System.IO;

namespace LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default
{
    /// <summary>
    /// Tracks processed orders in a plain text file. Each line contains a processed order number.
    /// </summary>
    public class ProcessedOrdersFileTracker : IProcessedOrdersTracker
    {
        private readonly string _filePath;
        private readonly HashSet<string> _orders;

        public ProcessedOrdersFileTracker(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _orders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (File.Exists(_filePath))
            {
                foreach (var line in File.ReadAllLines(_filePath))
                {
                    var order = line.Trim();
                    if (!string.IsNullOrEmpty(order))
                        _orders.Add(order);
                }
            }
        }

        public bool IsProcessed(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(orderNumber));

            return _orders.Contains(orderNumber.Trim());
        }

        public void MarkProcessed(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(orderNumber));

            orderNumber = orderNumber.Trim();
            if (_orders.Add(orderNumber))
            {
                File.AppendAllLines(_filePath, new[] { orderNumber });
            }
        }
    }
}
