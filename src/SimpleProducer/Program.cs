using System.Text;
using RabbitMQ.Client;

namespace SimpleProducer
{
    public class Producer
    {
        private const string QueueName = "simple_queue";

        public static async Task Main()
        {
            Console.WriteLine("RabbitMQ Producer");

            var factory = new ConnectionFactory() { HostName = "localhost" };

            var props = new BasicProperties
            {
                Persistent = true
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                for (int i = 1; i <= 100; i++)
                {
                    string message = $"Message {i}";
                    var body = Encoding.UTF8.GetBytes(message);

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: QueueName,
                        mandatory: false,
                        basicProperties: props,
                        body: body);
                    Console.WriteLine($" [x] Sent '{message}'");
                    Thread.Sleep(100);
                }

                Console.WriteLine("\nAll 100 messages have been published.");
                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Ensure RabbitMQ is running on localhost and accessible.");
                Console.ResetColor();
            }
        }
    }
}
