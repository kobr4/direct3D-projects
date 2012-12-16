using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using SlimDX;

namespace MyHelloWorldSlimDx
{
    class MyTriangle : RenderableInterface
    {
        SlimDX.Direct3D11.VertexBufferBinding bufferBinding;
        Buffer vertexBuffer; 
        DataStream vertices;
        private int triangleCount = 3;
        public MyTriangle()
        {
            vertices = new DataStream(12 * 3, true, true);
            vertices.Write(new Vector3(0.0f, 0.5f, 0.5f));
            vertices.Write(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f));
            vertices.Position = 0;
        }

        public void initBuffers(Device device)
        {
            vertexBuffer = new Buffer(device, vertices, 12 * 3, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            bufferBinding = new VertexBufferBinding(vertexBuffer, 12, 0);
        }

        public SlimDX.Direct3D11.VertexBufferBinding getVertices()
        {
            return bufferBinding;
        }

        public void dispose()
        {
            vertices.Close();
            vertexBuffer.Dispose();
        }

        public int getTriangleCount()
        {
            return triangleCount;
        }
    }
}
