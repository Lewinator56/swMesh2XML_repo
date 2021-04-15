using System;
using System.Collections.Generic;
using System.Text;

namespace swMesh2XML
{
    class SubMesh
    {
        public List<vertex> vertices = new List<vertex>();
        public UInt16 shader;
        public Single[] cullingMin;
        public  Single[] cullingMax;
        public List<Triangle> triangles = new List<Triangle>();
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public SubMesh()
        {

        }

        public void addVertex(vertex v)
        {
            this.vertices.Add(v);
        }
        public void setCullingMin(Single x, Single y, Single z)
        {
            if (cullingMin == null)
            {
                cullingMin = new Single[] { x, y, z };
            }
            else
            {
                cullingMin[0] = x < cullingMin[0] ? x : cullingMin[0];
                cullingMin[1] = y < cullingMin[1] ? y : cullingMin[1];
                cullingMin[2] = z < cullingMin[2] ? z : cullingMin[2];
            }
        }
        public void setCullingMax(Single x, Single y, Single z)
        {
            if (cullingMax == null)
            {
                cullingMax = new Single[] { x, y, z };
            }
            else
            {
                cullingMax[0] = x > cullingMax[0] ? x : cullingMax[0];
                cullingMax[1] = y > cullingMax[1] ? y : cullingMax[1];
                cullingMax[2] = z > cullingMax[2] ? z : cullingMax[2];
            }
        }
        public void setShader(UInt16 shaderID)
        {
            this.shader = shaderID;
        }
        public void addTriangle(Triangle t)
        {
            this.triangles.Add(t);
        }

    }
    
}
