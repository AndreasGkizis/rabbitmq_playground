using System.Diagnostics;
using System.Text;
using Common;
using RabbitMQ.Client;

namespace SimpleProducer
{
	public class Producer
	{
		public static async Task Main()
		{
			Console.WriteLine("RabbitMQ Producer");
			var config = ConfigLoader.LoadSharedConfig();
			var host = config["RabbitMQConfig:HostName"];
			var testQ = config["RabbitMQConfig:TestQueue"];
			var numberofmessages = Convert.ToInt32(config["RabbitMQConfig:NumberOfMsgsSent"]);

			var factory = new ConnectionFactory() { HostName = host };

			var props = new BasicProperties
			{
				Persistent = true
			};

			try
			{
				using var connection = await factory.CreateConnectionAsync();
				using var channel = await connection.CreateChannelAsync();
				await channel.QueueDeclareAsync(
					queue: testQ,
					durable: true,
					exclusive: false,
					autoDelete: false,
					arguments: null);

				// 3. Core publishing loop
				string? input;
				do
				{
					Console.WriteLine($"\n--- Publishing Batch of {numberofmessages} messages... ---");
					var timer = new Stopwatch();
					timer.Start();
					await SendBatch(numberofmessages, channel, testQ, props);
					Console.WriteLine($"Time elapsed: {timer.Elapsed}");
					Console.WriteLine("-----------------------------------------------------");

					Console.WriteLine("All messages have been published.");
					Console.WriteLine("Press [ENTER] to resend, or type 'q' and press [ENTER] to quit.");
                 
					// Read user input for loop control
					input = Console.ReadLine()?.ToLowerInvariant(); 

				} while (input != "q" && input != "quit");

				Console.WriteLine("Producer shutting down.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"An error occurred: {ex.Message}");
				Console.WriteLine("Ensure RabbitMQ is running on localhost and accessible.");
				Console.ResetColor();
			}
		}

		private async static Task SendBatch(int numberofmessages, IChannel channel, string testQ, BasicProperties props)
		{
			for (int i = 1; i <= numberofmessages; i++)
			{
				string message = $"Message {i}";
				var body = Encoding.UTF8.GetBytes(message);

				await channel.BasicPublishAsync(
					exchange: "",
					routingKey: testQ,
					mandatory: false,
					basicProperties: props,
					body: body);
				Console.WriteLine($" [x] Sent '{message}'");
			}
		}
	}
}