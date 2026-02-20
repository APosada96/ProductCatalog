using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Infrastructure.Idempotency
{
    public sealed class ProcessedRequest
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
}
