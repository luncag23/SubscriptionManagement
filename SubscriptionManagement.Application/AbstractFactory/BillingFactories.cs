using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.AbstractFactory
{
	public class GooglePayFactory : IBillingFactory
	{
		public IPaymentProcessor CreateProcessor() => new GooglePayProcessor();
		public IInvoiceGenerator CreateInvoice() => new GooglePayInvoice();
	}

	public class PayPalFactory : IBillingFactory
	{
		public IPaymentProcessor CreateProcessor() => new PayPalProcessor();
		public IInvoiceGenerator CreateInvoice() => new PayPalInvoice();
	}

	public class CreditCardFactory : IBillingFactory
	{
		public IPaymentProcessor CreateProcessor() => new CreditCardProcessor();
		public IInvoiceGenerator CreateInvoice() => new CreditCardInvoice();
	}
}
