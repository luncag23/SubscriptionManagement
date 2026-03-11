using System;
using System.Text;

namespace BusinessLogic.Singleton
{
	// "sealed" previne moștenirea clasei, ceea ce este recomandat pentru Singleton
	public sealed class LicenseGenerator
	{
		// Instanța unică este păstrată într-un câmp static
		// Lazy<T> pentru a asigura "Thread Safety" (siguranță la acces simultan)
		// și "Lazy Initialization" (crearea obiectului doar când e prima dată nevoie de el)
		private static readonly Lazy<LicenseGenerator> _instance =
			new Lazy<LicenseGenerator>(() => new LicenseGenerator());

		// Metoda publică de acces la instanță (Punctul Global de Acces)
		public static LicenseGenerator Instance => _instance.Value;

		// 3. CONSTRUCTOR PRIVAT - Nimeni din exterior nu poate face "new LicenseGenerator()"
		private LicenseGenerator()
		{
			// Aici pot fi inițializate logici complexe o singură dată
		}

		// Metoda de business: Generarea unei chei unice de tip Adobe (ex: ADBE-XXXX-XXXX)
		public string GenerateKey()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new Random();

			string Part1 = GenerateRandomString(4, random, chars);
			string Part2 = GenerateRandomString(4, random, chars);

			return $"ADBE-{Part1}-{Part2}";
		}

		private string GenerateRandomString(int length, Random random, string chars)
		{
			var result = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				result.Append(chars[random.Next(chars.Length)]);
			}
			return result.ToString();
		}
	}
}