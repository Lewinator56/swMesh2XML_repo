using System;
using System.Collections.Generic;
using System.Text;

namespace swMesh2XML
{
    class SubMesh
    {
        public List<vertex> vertices = new List<vertex>();
        public byte shader;
        public Single[] cullingMin;
        public  Single[] cullingMax;
        public List<Triangle> triangles = new List<Triangle>();

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
                cullingMin[0] = y < cullingMin[0] ? y : cullingMin[0];
                cullingMin[0] = z < cullingMin[0] ? z : cullingMin[0];
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
                cullingMax[0] = y > cullingMax[0] ? y : cullingMax[0];
                cullingMax[0] = z > cullingMax[0] ? z : cullingMax[0];
            }
        }
        public void setShader(byte shaderID)
        {
            this.shader = shaderID;
        }
        public void addTriangle(Triangle t)
        {
            this.triangles.Add(t);
        }
    }
    
}
