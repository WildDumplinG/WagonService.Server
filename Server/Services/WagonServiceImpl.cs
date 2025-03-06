using Grpc.Core;
using Microsoft.Extensions.Options;
using Npgsql;
using Dapper;
using System.Data;
using DBModels = WagonService.Server.DataBase.Models;
using Server.Services.Settings;

namespace WagonService.Server.Services
{
    public class WagonServiceImpl(IOptions<WagonServiceImplSettings> options, ILogger<WagonServiceImpl> logger) : WagonService.WagonServiceBase
    {
        #region Settings

        private readonly IOptions<WagonServiceImplSettings> _settings = options;

        #endregion

        public override async Task<WagonResponse> GetWagons(WagonRequest request, ServerCallContext context)
        {
            if (!IsValidTimeFormat(request.StartTime, out var startTime) || !IsValidTimeFormat(request.EndTime, out var endTime))
            {
                var ex = "Неверный формат времени.";
                logger.LogWarning(ex, "WagonServer");
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex));
            }

            if (startTime >= endTime)
            {
                var ex = "Start time должно быть меньше End time.";
                logger.LogWarning(ex, "WagonServer");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Start time должно быть меньше End time."));
            }

            using (IDbConnection dbConnection = new NpgsqlConnection(_settings.Value.Connection))
            {
                try
                {
                    dbConnection.Open();
                }
                catch (Exception ex)
                { 
                }
                

                string sqlQuery = @"
                    SELECT 
                        e.""Number"" AS InventoryNumber, 
                        ea.""Time"" AS ArrivalTime, 
                        ed.""Time"" AS DepartureTime
                    FROM 
                        ""Epc"" e
                    LEFT JOIN 
                        ""EpcEvent"" ee ON e.""Id"" = ee.""IdEpc"" AND ee.""Type"" = 0
                    LEFT JOIN 
                        ""EventArrival"" ea ON ee.""IdPath"" = ea.""IdPath"" AND ee.""Time"" = ea.""Time"" 
                    LEFT JOIN 
                        ""EventDeparture"" ed ON ee.""IdPath"" = ed.""IdPath"" AND ee.""Time"" = ed.""Time""
                    WHERE 
                        ed.""Time"" BETWEEN @StartDate AND @EndDate
                    AND 
                        e.""Type"" = 1";

                var parameters = new { StartDate = startTime, EndDate = endTime };
                var wagonInfos = await dbConnection.QueryAsync<DBModels.WagonInfo>(sqlQuery, parameters);

                var response = new WagonResponse();

                response.Wagons.AddRange(wagonInfos.Select(wagon => new Wagon
                {
                    InventoryNumber = wagon.InventoryNumber,
                    ArrivalTime = wagon.ArrivalTime.ToString("o"),
                    DepartureTime = wagon.DepartureTime.ToString("o")
                }));

                return response;
            }
        }

        private bool IsValidTimeFormat(string time, out DateTime dateTime)
        {
            return DateTime.TryParse(time, out dateTime);
        }
    }
}
