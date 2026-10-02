using System;
using System.Net;
using System.Net.Mail;

namespace CommonLibrary;

public static class EmailBL
{
	private static int TotalSendEmails = 0;

	private static object thisLock { get; set; } = new object();

	public static string SendEmail(string subject, string message, string mailAddress)
	{
		try
		{
			if (mailAddress == "")
			{
				return null;
			}
			Email email = new Email(mailAddress);
			email.Subject = subject;
			email.Message = message;
			if (TotalSendEmails > 200)
			{
				return null;
			}
			SmtpClient smtpClient = new SmtpClient("mail.guppy.bet", 587);
			smtpClient.Credentials = new NetworkCredential("support@guppy.bet", "Sto9cm19by50");
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = email.FromEmail;
			foreach (MailAddress mailAdress in email.ToMailboxes)
			{
				mailMessage.To.Add(mailAdress);
			}
			mailMessage.Subject = email.Subject;
			mailMessage.Body = email.Message;
			lock (thisLock)
			{
				smtpClient.Send(mailMessage);
				email.IsSend = true;
			}
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
