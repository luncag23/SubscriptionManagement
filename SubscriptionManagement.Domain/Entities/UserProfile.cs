using System;
using System.Collections.Generic;
using System.Text;
// Builder

namespace Domain.Entities
{
	public class UserProfile
	{
		// Aceasta este linia care îți lipsește:
		public Guid Id { get; set; }

		public Guid UserId { get; set; } // Foreign Key către User

		public string DisplayName { get; set; }
		public string Bio { get; set; }
		public string Theme { get; set; } // "Light" sau "Dark"
		public bool EmailNotifications { get; set; }

		// Pentru a evita erori în baza de date cu List<string>, 
		// o definim ca listă, dar trebuie inițializată
		public List<string> SocialLinks { get; set; } = new List<string>();

		// Referința de navigare către User
		public User User { get; set; }
	}
}
