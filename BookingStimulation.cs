using Dapper;

namespace Concurrent_Booking
{
    public class BookingStimulation
    {
        // list of current users trying to book the same seat concurrently
        private List<string> users = new List<string> { "Sheldon", "Jack", "Mahito", "Drack", "Neza" };
        private string userChoice;

        DbHelper dbHelper;

        public BookingStimulation()
        {
            dbHelper = new DbHelper();
        }
        public async Task Start()
        {
            try
            {
                do
                {
                    // reset the seat which every user will try to book, so that we can simulate the concurrent booking scenario
                    await ResetSeatsAsync();

                    
                    // concurrent Task to simulate the concurrent booking scenario
                    List<Task> bookingTask = new List<Task>();

                    // using AsParallel to simulate the concurrent booking scenario,
                    // where multiple users are trying to book the same seat at the same time
                    users.AsParallel().ForAll(user =>
                    {
                        bookingTask.Add(BookSeat(user));
                    });

                    // wait for all booking tasks to complete
                    await Task.WhenAll(bookingTask);

                    Console.WriteLine("press 1 and Enter to stimulate again?");

                    userChoice = Console.ReadLine();
                }
                while (userChoice == "1");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in booking stimulation: {ex.Message}");
            }            
        }

        private async Task BookSeat(string user)
        {
            string bookingQuery = "UPDATE seats SET username = @userName WHERE id = 1;";
            string selectSeatQuery = "SELECT id, username FROM seats WHERE ID = 1 and username is null FOR UPDATE;";
            try
            {
                using (var con = await dbHelper.getOpenConnectionAsync())
                {
                    using (var transaction = await con.BeginTransactionAsync())
                    {
                        var seatDeetails = await con.QueryFirstOrDefaultAsync<Seats>(selectSeatQuery, transaction);

                        if (seatDeetails == null)
                        {
                            Console.WriteLine($"Failed to book seat for {user}");
                            return ;
                        }

                        await con.ExecuteAsync(bookingQuery, new { userName = user }, transaction);
                        Console.WriteLine($"Seat booked successfully for {user}");

                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error booking seat for {user}: {ex.Message}");
            }
        }       

        private async Task ResetSeatsAsync()
        {
            string resetSeatsQuery = "UPDATE seats SET username = NULL;";

            using (var con = await dbHelper.getOpenConnectionAsync())
            {
                await con.ExecuteAsync(resetSeatsQuery);                
                Console.WriteLine("\n---------------------Seats reset successfully.---------------------");
                Console.WriteLine($"{string.Join(", ", users)} are Trying to book the same seat numbered 1.\n");                
            }
        }
    }
}
