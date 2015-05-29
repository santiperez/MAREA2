using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marea
{
    [Serializable]
    public class None
    {
        public None() { }

		public static None Instance {
			get { return null; }
		}
    }
}
