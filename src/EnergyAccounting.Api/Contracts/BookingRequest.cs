namespace EnergyAccounting.Api.Contracts
{
    public class BookingRequest
    {
        public int TripId { get; set; }

        public string SeatId { get; set; } = string.Empty;
    }
}