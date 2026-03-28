using System;
using System.Collections.Generic;
using System.Text;

namespace DataContract.Enums
{
	public enum SubscriptionStatus
	{
		Active,
		Expired,
		Canceled,
		Trial,
		CanceledPending   // 4 - Utilizatorul a apăsat Cancel, dar mai are zile plătite
	}
}
