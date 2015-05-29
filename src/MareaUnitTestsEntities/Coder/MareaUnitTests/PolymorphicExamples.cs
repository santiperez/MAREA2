using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MareaUnitTests
{
    [Serializable]
    public class PA
    {
        public PA() { }
        public int x;
        public char y;
        public QA z;
    }

    [Serializable]
    public class QA
    {
        public QA() { }
        public char a;
        public char b;
    }

    [Serializable]
    public class PB
    {
        public PB() { }
        public int x;
        public char y;
        public QB z;
    }

    [Serializable]
    public struct QB
    {
        public char a;
        public char b;
    }

    [Serializable]
    public class QC:QA
    {
        public char c;
    }

}
