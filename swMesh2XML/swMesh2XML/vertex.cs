using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace swMesh2XML
{
    
    class vertex
    {
        public Single px;
        public Single py;
        public Single pz;
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public Single nx;
        public Single ny;
        public Single nz;

        public vertex(Single px, Single py, Single pz, byte r, byte g, byte b, byte a, Single nx, Single ny, Single nz)
        {
            this.px = px;
            this.py = py;
            this.pz = pz;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
        }

        public vertex(Single px, Single py, Single pz, byte r, byte g, byte b, byte a)
        {
            this.px = px;
            this.py = py;
            this.pz = pz;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public void setNormals(Single nx, Single ny, Single nz)
        {
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
        }
    }
    
}
