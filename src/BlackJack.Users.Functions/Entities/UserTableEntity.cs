using Azure;
using Azure.Data.Tables;

namespace BlackJack.Users.Functions.Entities
{
    public class UserTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = null!;
        public string RowKey { get; set; } = null!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
