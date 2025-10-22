// Common/ConfigLoader.cs

using Microsoft.Extensions.Configuration;

namespace Common
{
	public static class ConfigLoader
	{
		public static IConfigurationRoot LoadSharedConfig()
		{
			string? dir = Directory.GetCurrentDirectory();
			string? configFile = null;

			while (dir != null)
			{
				var candidate = Path.Combine(dir, "appsettings.shared.json");
				if (File.Exists(candidate))
				{
					configFile = candidate;
					break;
				}
				dir = Directory.GetParent(dir)?.FullName;
			}

			if (configFile == null)
				throw new FileNotFoundException("Could not find appsettings.shared.json in any parent directory");

			return new ConfigurationBuilder()
				.AddJsonFile(configFile, optional: false, reloadOnChange: true)
				.Build();
		}

	}
}