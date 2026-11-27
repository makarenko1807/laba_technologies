using EnergyAccounting.Api.Services;

namespace EnergyAccounting.Tests;

public class BookingTests
{
    [Fact]
    public void CreateBooking_FreeSeat_ReturnsCreated()
    {
        // Arrange
        var service = new BookingService();

        // Act
        var result = service.CreateBooking(1, "12");

        // Assert
        Assert.Equal(BookingStatus.Created, result.Status);
        Assert.NotNull(result.Booking);
        Assert.Equal(1, result.Booking.TripId);
        Assert.Equal("12", result.Booking.SeatId);
        Assert.Single(service.GetAllBookings());
    }

    [Fact]
    public void CreateBooking_SameSeat_ReturnsConflict()
    {
        // Arrange
        var service = new BookingService();
        service.CreateBooking(1, "12");

        int bookingsBefore = service.GetAllBookings().Count;

        // Act
        var result = service.CreateBooking(1, "12");

        // Assert
        Assert.Equal(BookingStatus.Conflict, result.Status);
        Assert.Null(result.Booking);
        Assert.Equal(bookingsBefore, service.GetAllBookings().Count);
    }

    [Fact]
    public void CreateBooking_SameSeatDifferentTrip_ReturnsCreated()
    {
        // Arrange
        var service = new BookingService();
        service.CreateBooking(1, "12");

        // Act
        var result = service.CreateBooking(2, "12");

        // Assert
        Assert.Equal(BookingStatus.Created, result.Status);
        Assert.Equal(2, result.Booking!.TripId);
        Assert.Equal(2, service.GetAllBookings().Count);
    }

    [Fact]
    public void CreateBooking_InvalidTripId_ReturnsInvalid()
    {
        // Arrange
        var service = new BookingService();

        // Act
        var result = service.CreateBooking(0, "12");

        // Assert
        Assert.Equal(BookingStatus.Invalid, result.Status);
        Assert.Null(result.Booking);
        Assert.Empty(service.GetAllBookings());
    }

    [Fact]
    public void CreateBooking_EmptySeatId_ReturnsInvalid()
    {
        // Arrange
        var service = new BookingService();

        // Act
        var result = service.CreateBooking(1, "");

        // Assert
        Assert.Equal(BookingStatus.Invalid, result.Status);
        Assert.Null(result.Booking);
        Assert.Empty(service.GetAllBookings());
    }
}