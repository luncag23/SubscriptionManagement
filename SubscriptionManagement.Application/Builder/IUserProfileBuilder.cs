using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace BusinessLogic.Builder
{
	public interface IUserProfileBuilder
	{
		void Reset();
		// ADAUGĂ ACEASTĂ METODĂ:
		void SetUser(Guid userId);
		void SetBasicInfo(string name, string bio);
		void SetPreferences(string theme, bool notifications);
		// Sugestie: dacă nu vrei tabele extra în DB, amână SocialMedia sau folosește un string lung
		void AddSocialMedia(string platform, string url);
		UserProfile GetProfile();
	}
}
