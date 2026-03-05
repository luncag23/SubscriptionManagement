using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.AbstractFactory
{
	// --- FAMILIA GOOGLE PAY ---
	public class GooglePayProcessor : IPaymentProcessor
	{
		public string ProcessPayment(decimal amount) => $"Plată de {amount} Lei procesată securizat prin Google Pay API.";
	}
	public class GooglePayInvoice : IInvoiceGenerator
	{
		public string GenerateInvoice() => "Factură digitală Google Pay generată și trimisă în G-Wallet.";
	}

	// --- FAMILIA PAYPAL ---
	public class PayPalProcessor : IPaymentProcessor
	{
		public string ProcessPayment(decimal amount) => $"Redirecționare către PayPal pentru suma de {amount} Lei.";
	}
	public class PayPalInvoice : IInvoiceGenerator
	{
		public string GenerateInvoice() => "Chitanță PayPal (Transaction ID: PP-12345) generată.";
	}

	// --- FAMILIA CREDIT CARD (Metoda clasică) ---
	public class CreditCardProcessor : IPaymentProcessor
	{
		public string ProcessPayment(decimal amount) => $"Tranzacție de {amount} Lei efectuată direct de pe cardul bancar.";
	}
	public class CreditCardInvoice : IInvoiceGenerator
	{
		public string GenerateInvoice() => "Factură fiscală standard generată în format PDF.";
	}
}
