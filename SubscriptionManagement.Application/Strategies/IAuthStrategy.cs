using DataContract.DTOs;
using System.Threading.Tasks;

namespace BusinessLogic.Strategies
{
	// Rezultat comun pentru orice tip de login
	public class AuthResult
	{
		public bool Success { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public string ErrorMessage { get; set; }
	}

	// Interfața Strategy
	public interface IAuthStrategy
	{
		Task<AuthResult> AuthenticateAsync(string identifier, string secret);
	}
}