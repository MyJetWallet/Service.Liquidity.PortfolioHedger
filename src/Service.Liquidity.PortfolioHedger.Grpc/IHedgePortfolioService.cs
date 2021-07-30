using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IHedgePortfolioService
    {
        [OperationContract]
        Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request);
    }

    [DataContract]
    public class ExecuteManualConvertRequest
    {
        [DataMember(Order = 1)] public string FromAsset { get; set; }
        [DataMember(Order = 2)] public decimal FromAssetVolume { get; set; }
        [DataMember(Order = 3)] public string ToAsset { get; set; }
    }

    [DataContract]
    public class ExecuteManualConvertResponse
    {
        [DataMember(Order = 1)] public decimal ExecutedFromValue { get; set; }
        [DataMember(Order = 2)] public decimal ExecutedToValue { get; set; }
    }
}