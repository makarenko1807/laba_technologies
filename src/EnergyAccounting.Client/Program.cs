using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
namespace EnergyAccounting.Client
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static string serverUrl = "http://localhost:5100";
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            client.BaseAddress = new Uri(serverUrl);

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== СИСТЕМА БРОНЮВАННЯ КВИТКІВ ===");
                Console.WriteLine("1. Забронювати квиток");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть дію: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await BookTicket();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Невірний пункт меню.");
                        Pause();
                        break;
                }
            }
        }

        static async Task BookTicket()
        {
            Console.Clear();
            Console.WriteLine("=== БРОНЮВАННЯ КВИТКА ===");

            Console.Write("Введіть ID рейсу: ");
            string? tripInput = Console.ReadLine();

            if (!int.TryParse(tripInput, out int tripId) || tripId <= 0)
            {
                Console.WriteLine("Помилка: ID рейсу має бути додатним числом.");
                Pause();
                return;
            }

            Console.Write("Введіть номер місця: ");
            string? seatId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(seatId))
            {
                Console.WriteLine("Помилка: номер місця не може бути пустим.");
                Pause();
                return;
            }

            var request = new
            {
                TripId = tripId,
                SeatId = seatId.Trim()
            };

            try
            {
                HttpResponseMessage response =
                    await client.PostAsJsonAsync("/api/bookings", request);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var booking = await response.Content.ReadFromJsonAsync<BookingResponse>();

                    Console.WriteLine("\nБронювання успішно створено!");
                    if (booking != null)
                    {
                        Console.WriteLine($"ID бронювання: {booking.Id}");
                        Console.WriteLine($"Рейс: {booking.TripId}");
                        Console.WriteLine($"Місце: {booking.SeatId}");
                        Console.WriteLine($"Дата створення: {booking.CreatedAt}");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine("\nВідмова: це місце вже заброньоване.");
                    Console.WriteLine("Будь ласка, оберіть інше місце.");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"\nСервер повернув статус: {(int)response.StatusCode}");
                    Console.WriteLine(error);
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nНе вдалося підключитися до сервера.");
                Console.WriteLine("Переконайтеся, що сервер запущений і адреса правильна.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виникла помилка: {ex.Message}");
            }

            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
            Console.ReadLine();
        }
    }

    public class BookingResponse
    {
        public Guid Id { get; set; }
        public int TripId { get; set; }
        public string SeatId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}