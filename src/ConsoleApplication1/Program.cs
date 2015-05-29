using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var s =System.Reflection.Assembly.Load("Marea.Tests.Entities");
            var t=s.GetType("Marea.Packet`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]");
        }
    }
}
