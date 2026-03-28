using Microsoft.AspNetCore.Http;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusinessLogic.Proxy
{
	public class AdminProxy : IAdminService
	{
		private readonly AdminService _realService;
		private readonly IHttpContextAccessor _httpContext;

		public AdminProxy(AdminService realService, IHttpContextAccessor httpContext)
		{
			_realService = realService;
			_httpContext = httpContext;
		}

		// Metodă privată de verificare a permisiunilor
		private bool IsAuthorized()
		{
			var user = _httpContext.HttpContext?.User;
			return user != null && user.IsInRole("Admin");
		}

		public async Task<Guid> CreateApp(string name, decimal price, string description, string imageUrl)
		{
			if (!IsAuthorized())
				throw new UnauthorizedAccessException("Proxy: Acces refuzat! Nu aveți drepturi de Administrator pentru a crea aplicații.");

			return await _realService.CreateApp(name, price, description, imageUrl);
		}

		public async Task<Guid> CreateBundle(string name, string description)
		{
			if (!IsAuthorized())
				throw new UnauthorizedAccessException("Proxy: Acces refuzat! Nu aveți drepturi de Administrator pentru a crea pachete.");

			return await _realService.CreateBundle(name, description);
		}

		public async Task UpdateApp(CreativeApp app, List<Guid> selectedIds)
		{
			if (!IsAuthorized())
				throw new UnauthorizedAccessException("Proxy: Modificarea resurselor este restricționată.");

			await _realService.UpdateApp(app, selectedIds);
		}
	}
}