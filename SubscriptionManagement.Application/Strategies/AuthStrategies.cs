using DAL.Abstract;
using BusinessLogic.Security;

namespace BusinessLogic.Strategies
{
	// 1. Strategia Locală (Email + Parolă)
	public class LocalAuthStrategy : IAuthStrategy
	{
		private readonly ISubscriptionRepository _repository;
		public LocalAuthStrategy(ISubscriptionRepository repository) => _repository = repository;

		public async Task<AuthResult> AuthenticateAsync(string email, string password)
		{
			var user = await _repository.GetUserByEmailAsync(email);
			if (user != null && PasswordHasher.VerifyPassword(user.Password, password))
			{
				return new AuthResult { Success = true, Email = user.Email, FullName = user.FullName };
			}
			return new AuthResult { Success = false, ErrorMessage = "Email sau parolă incorectă." };
		}
	}

	// 2. Strategia Google (Simulată)
	public class GoogleAuthStrategy : IAuthStrategy
	{
		public async Task<AuthResult> AuthenticateAsync(string token, string unused)
		{
			// Aici în realitate s-ar apela API-ul Google pentru a valida token-ul
			await Task.Delay(500); // Simulare apel extern
			return new AuthResult { Success = true, Email = "google-user@gmail.com", FullName = "Google User Test" };
		}
	}

	
}