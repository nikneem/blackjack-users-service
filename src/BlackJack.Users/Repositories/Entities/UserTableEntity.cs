﻿using Azure;
using Azure.Data.Tables;

namespace BlackJack.Users.Repositories.Entities;

public class UserTableEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}