using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.Rating;

public interface IAppRatingService
{
	Task AskUserForRatingAsync();
}
