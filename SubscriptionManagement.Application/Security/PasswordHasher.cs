using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Security
{
	public static class PasswordHasher
	{
		private static readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

		// Transformă parola în hash (ex: "admin123" -> "AQAAAAIAAYagAAAAE...")
		public static string HashPassword(string password)
		{
			return _hasher.HashPassword(null, password);
		}

		// Verifică dacă parola introdusă se potrivește cu hash-ul din DB
		public static bool VerifyPassword(string hashedPassword, string providedPassword)
		{
			var result = _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
			return result == PasswordVerificationResult.Success;
		}
	}
}