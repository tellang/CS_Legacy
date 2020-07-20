using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Triangular{
    interface IPosition{
        public int xPos { get; set; }
        public int yPos { get; set; }
    }
    interface IDictionary{
        public int sort { get; set; }
        public int level { get; set; }
        public int index { get; set; }
    }
    interface IInsideSize{
        public int MyProperty { get; set; }

    }   
     interface IOutsideSize{
        public int outWidth { get; set; }
        public int outHeight { get; set; }
    }
}