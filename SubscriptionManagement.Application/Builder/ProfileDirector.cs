using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Builder
{
	public class ProfileDirector
	{
		private readonly IUserProfileBuilder _builder;

		public ProfileDirector(IUserProfileBuilder builder) => _builder = builder;

		// Rețeta 1: Profil Minimal - Acum are nevoie de userId
		public void BuildMinimalProfile(Guid userId, string name)
		{
			_builder.Reset();
			_builder.SetUser(userId); // Legătura cu tabela Users
			_builder.SetPreferences("Light", true);
			_builder.SetBasicInfo(name, "Adobe User proaspăt înscris.");
		}

		// Rețeta 2: Profil Profesional - Pentru cei care aleg abonament Premium
		public void BuildFullProfessionalProfile(Guid userId, string name, string bio)
		{
			_builder.Reset();
			_builder.SetUser(userId);
			_builder.SetBasicInfo(name, bio);
			_builder.SetPreferences("Dark", true);
			_builder.AddSocialMedia("Portfolio", "myportfolio.com/" + name);
		}
	}
}
