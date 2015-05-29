using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;

namespace Napster
{
    [ServiceDefinition("This service represents an altimeter")]
    public interface IClient
    {
        String DownloadFile(String filename);
    }
}
