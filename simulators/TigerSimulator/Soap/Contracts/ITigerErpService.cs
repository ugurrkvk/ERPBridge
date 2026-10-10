using System.ServiceModel;

namespace TigerSimulator.Soap.Contracts;

[ServiceContract(Name = "TigerErpService", Namespace = "urn:tiger-simulator:erp:v1")]
public interface ITigerErpService
{
    [OperationContract(Name = "GetInventory")]
    Task<InventoryItemResponse[]> GetInventoryAsync();

    [OperationContract(Name = "CreateSalesOrder")]
    Task<CreateSalesOrderResponse> CreateSalesOrderAsync(CreateSalesOrderRequest request);
}