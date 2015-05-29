using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;

namespace DHT
{
    [ServiceDefinition("This service is a DHT Node")]
    public interface IDHT
    {
        String GetValue(String key);
        void SetValue(String key, String value);
        void Login(String ip);

        void Logout(String ip);

        void SetPredecessor(String ip);
        void SetSucessor(String ip);

        void SetValues(Dictionary<String, String> values);
    }
}
