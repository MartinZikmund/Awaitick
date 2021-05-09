using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using EventCountdowns.Core.Services.Mail;

namespace EventCountdowns.Core.Services
{
    public class MailService : IMailService
    {
        public async Task ComposeMailAsync( string subject, string addressTo )
        {
            await Launcher.LaunchUriAsync( new Uri( $"mailto:{addressTo}?subject={subject}" ) );
        }
    }
}
