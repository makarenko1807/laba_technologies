namespace EnergyAccounting.Api.Models
{
    public class Booking
    {
        public Guid Id { get; set; }

        public int TripId { get; set; }

        public string SeatId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}