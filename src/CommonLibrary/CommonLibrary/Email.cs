using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace CommonLibrary;

public class Email
{
	public string Subject { get; set; }

	public string Message { get; set; }

	public DateTime CreateTime { get; set; } = DateTime.Now;

	public DateTime SendTime { get; set; }

	public MailAddress FromEmail { get; set; }

	public List<MailAddress> ToMailboxes { get; set; } = new List<MailAddress>();

	public bool IsSend { get; set; }

	public Email(string mainAddress)
	{
		FromEmail = new MailAddress("betforks365@yandex.ru");
		ToMailboxes.Add(new MailAddress(mainAddress));
	}
}
