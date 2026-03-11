using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace BusinessLogic.Builder
{
	public class UserProfileBuilder : IUserProfileBuilder
	{
		private UserProfile _profile;

		public UserProfileBuilder() => Reset();

		public void Reset()
		{
			_profile = new UserProfile
			{
				Id = Guid.NewGuid(),
				// Inițializăm lista pentru a evita NullReferenceException
				SocialLinks = new List<string>()
			};
		}

		// IMPLEMENTARE NOUĂ:
		public void SetUser(Guid userId)
		{
			_profile.UserId = userId;
		}

		public void SetBasicInfo(string name, string bio)
		{
			_profile.DisplayName = name;
			_profile.Bio = bio;
		}

		public void SetPreferences(string theme, bool notifications)
		{
			_profile.Theme = theme;
			_profile.EmailNotifications = notifications;
		}

		public void AddSocialMedia(string platform, string url)
		{
			_profile.SocialLinks.Add($"{platform}: {url}");
		}

		public UserProfile GetProfile()
		{
			UserProfile result = _profile;
			Reset();
			return result;
		}
	}
}
