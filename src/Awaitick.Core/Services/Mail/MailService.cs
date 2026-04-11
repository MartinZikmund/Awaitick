using Awaitick.Core.Services.Mail;
using Windows.System;

namespace Awaitick.Core.Services;

public class MailService : IMailService
{
	public async Task ComposeMailAsync(string subject, string addressTo) => await Launcher.LaunchUriAsync(new Uri($"mailto:{addressTo}?subject={subject}"));
}
