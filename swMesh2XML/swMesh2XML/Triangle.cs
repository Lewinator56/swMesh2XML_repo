using System;
using System.Collections.Generic;
using System.Text;

namespace swMesh2XML
{
    class Triangle
    {
        public byte v1;
        public byte v2;
        public byte v3;

        public Triangle(byte v1, byte v2, byte v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }
}
