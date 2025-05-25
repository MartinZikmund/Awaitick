namespace Awaitick.Core.Services.Mail;

public interface IMailService
{
	Task ComposeMailAsync(string subject, string addressTo);
}
