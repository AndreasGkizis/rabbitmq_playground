using System.Text;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SimpleConsumer
{
	public class Consumer
	{
		public static async Task Main()
		{
			Console.WriteLine("RabbitMQ Consumer");
			var config = ConfigLoader.LoadSharedConfig();
			var host = config["RabbitMQConfig:HostName"];
			var testQ = config["RabbitMQConfig:TestQueue"];

			var factory = new ConnectionFactory() { HostName = host };

			try
			{
				using var connection = await factory.CreateConnectionAsync();
				using var channel = await connection.CreateChannelAsync();

				// 1. Declare the Queue (ensures it exists)
				await channel.QueueDeclareAsync(
					queue: testQ,
					durable: true,
					exclusive: false,
					autoDelete: false,
					arguments: null);

				// 2. Create Async Consumer
				var consumer = new AsyncEventingBasicConsumer(channel);
				consumer.ReceivedAsync += async (model, ea) =>
				{
					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);

					// Simulate processing
					Console.WriteLine($" [x] Processing '{message}'");

					// Acknowledge the message
					await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
				};

				// 3. Start Consuming
				await channel.BasicConsumeAsync(
					queue: testQ,
					autoAck: false,
					consumer: consumer);

				Console.WriteLine("Waiting for messages. Press [enter] to exit.");
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