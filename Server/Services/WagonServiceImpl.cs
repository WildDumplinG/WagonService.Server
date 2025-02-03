using Grpc.Core;
using Microsoft.Extensions.Options;
using Npgsql;
using Dapper;
using System.Data;
using DBModels = WagonService.Server.DataBase.Models;

namespace WagonService.Server.Services
{
    public class WagonServiceImpl(IOptions<WagonServiceImpl.WagonServiceImplSettings> options) : WagonService.WagonServiceBase
    {
        #region Settings

        private readonly IOptions<WagonServiceImplSettings> _settings = options;

        #endregion
        public class WagonServiceImplSettings
        { 
            public string Connection { get; set; } = "";
        }

        public override async Task<WagonResponse> GetWagons(WagonRequest request, ServerCallContext context)
        {
            var startTime = DateTime.Parse(request.StartTime);
            var endTime = DateTime.Parse(request.EndTime);

            using (IDbConnection dbConnection = new NpgsqlConnection(_settings.Value.Connection))
            {
                dbConnection.Open();

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
                        ""EventArrival"" ea ON ee.""IdPath"" = ea.""IdPath"" 
                    LEFT JOIN 
                        ""EventDeparture"" ed ON ee.""IdPath"" = ed.""IdPath""
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
    }
}
