using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.AbstractFactory
{
	public interface IBillingFactory
	{
		IPaymentProcessor CreateProcessor();
		IInvoiceGenerator CreateInvoice();
	}

	public interface IPaymentProcessor
	{
		string ProcessPayment(decimal amount);
	}

	public interface IInvoiceGenerator
	{
		string GenerateInvoice();
	}
}
