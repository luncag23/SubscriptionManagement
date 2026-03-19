using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Adapters
{
	// 1. TARGET: Interfața pe care aplicația noastră o folosește (Modernă & Async)
	public interface ICurrencyConverter
	{
		Task<decimal> ConvertToEur(decimal amountRon);
		string GetCurrencySymbol();
	}

	// 2. MODEL: Pentru maparea răspunsului JSON de la Frankfurter API
	// Exemplu JSON: { "amount": 1.0, "base": "RON", "date": "2024-03-11", "rates": { "EUR": 0.2012 } }
	public class FrankfurterResponse
	{
		public decimal amount { get; set; }
		public string @base { get; set; }
		public Dictionary<string, decimal> rates { get; set; }
	}

	// 3. ADAPTER: Adaptăm un API extern (Web REST Service) la interfața noastră locală
	public class CurrencyAdapter : ICurrencyConverter
	{
		private readonly HttpClient _httpClient;
		private const string ApiUrl = "https://api.frankfurter.app/latest?from=RON&to=EUR";

		public CurrencyAdapter(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<decimal> ConvertToEur(decimal amountRon)
		{
			try
			{
				// Facem apelul real către API-ul de curs valutar (Banca Centrală Europeană)
				var response = await _httpClient.GetFromJsonAsync<FrankfurterResponse>(ApiUrl);

				if (response != null && response.rates.ContainsKey("EUR"))
				{
					decimal rate = response.rates["EUR"];
					return amountRon * rate;
				}
			}
			catch (Exception ex)
			{
				// Logica de Fallback: Dacă API-ul nu răspunde, folosim un curs aproximativ (0.20) 
				// pentru ca procesul de checkout să nu se blocheze.
				Console.WriteLine($"[ADAPTER ERROR] Nu am putut lua cursul real: {ex.Message}");
				return amountRon * 0.201m;
			}

			return amountRon * 0.201m;
		}

		public string GetCurrencySymbol()
		{
			return "€";
		}
	}
}