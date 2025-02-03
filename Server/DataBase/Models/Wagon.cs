namespace WagonService.Server.DataBase.Models
{
    public class WagonInfo
    {
        public string InventoryNumber { get; set; } = "";
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
    }
}
