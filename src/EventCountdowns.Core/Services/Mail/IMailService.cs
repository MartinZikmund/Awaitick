using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.Mail;

public interface IMailService
{
	Task ComposeMailAsync(string subject, string addressTo);
}
