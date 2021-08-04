using MyNoSqlServer.Abstractions;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExternalExchangeSettingsNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-externalexchange-settings";
        private static string GeneratePartitionKey(string exchangeName) => $"exchange:{exchangeName}";
        private static string GenerateRowKey() => "settings";
        
        public ExternalExchangeSettings Settings { get; set; }
        
        public static ExternalExchangeSettingsNoSql Create(ExternalExchangeSettings settings)
        {
            return new ExternalExchangeSettingsNoSql()
            {
                PartitionKey = GeneratePartitionKey(settings.ExchangeName),
                RowKey = GenerateRowKey(),
                Settings = settings
            };
        }
    }
}