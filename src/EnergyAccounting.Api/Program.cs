using EnergyAccounting.Api.Models;
using EnergyAccounting.Api.Contracts;
using EnergyAccounting.Api.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
var bookingService = new BookingService();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapPost("/api/bookings", (BookingRequest request) =>
{
    var result = bookingService.CreateBooking(
        request.TripId,
        request.SeatId);

    return result.Status switch
    {
        BookingStatus.Created => Results.Created(
            $"/api/bookings/{result.Booking!.Id}",
            result.Booking),

        BookingStatus.Conflict => Results.Conflict(new
        {
            message = result.Message
        }),

        BookingStatus.Invalid => Results.BadRequest(new
        {
            message = result.Message
        }),

        _ => Results.BadRequest()
    };
});
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
