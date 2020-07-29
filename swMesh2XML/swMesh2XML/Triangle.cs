using System;
using System.Collections.Generic;
using System.Text;

namespace swMesh2XML
{
    class Triangle
    {
        public UInt16 v1;
        public UInt16 v2;
        public UInt16 v3;

        public Triangle(UInt16 v1, UInt16 v2, UInt16 v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }
}
