using EnergyAccounting.Api.Models;

namespace EnergyAccounting.Api.Services;

public class BookingService
{
    private readonly List<Booking> bookings = new();
    private readonly object bookingsLock = new();

    public BookingResult CreateBooking(int tripId, string? seatId)
    {
        if (tripId <= 0 || string.IsNullOrWhiteSpace(seatId))
        {
            return new BookingResult(
                BookingStatus.Invalid,
                null,
                "Некоректний ID рейсу або номер місця.");
        }

        seatId = seatId.Trim();

        lock (bookingsLock)
        {
            bool seatIsOccupied = bookings.Any(b =>
                b.TripId == tripId &&
                b.SeatId.Equals(seatId, StringComparison.OrdinalIgnoreCase));

            if (seatIsOccupied)
            {
                return new BookingResult(
                    BookingStatus.Conflict,
                    null,
                    "Це місце вже заброньоване.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                SeatId = seatId,
                CreatedAt = DateTime.UtcNow
            };

            bookings.Add(booking);

            return new BookingResult(
                BookingStatus.Created,
                booking,
                "Бронювання успішно створено.");
        }
    }

    public List<Booking> GetAllBookings()
    {
        lock (bookingsLock)
        {
            return bookings.ToList();
        }
    }
}

public enum BookingStatus
{
    Created,
    Conflict,
    Invalid
}

public record BookingResult(
    BookingStatus Status,
    Booking? Booking,
    string Message);