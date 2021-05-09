using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.File
{
    public interface IFileService
    {
        Task<string> GetDataFileContentsAsync( string filePath );

        Task SetDataFileContentsAsync( string filePath, string contents );
    }
}
