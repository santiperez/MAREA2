using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;

namespace Napster
{
    [ServiceDefinition("This service is a Napster Server")]
    public interface IServer
    {
        void IHaveFile(String fileName, String ip);

        String[] IWantFile(String fileName);
    }
}
