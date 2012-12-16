using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using SlimDX;

namespace MyHelloWorldSlimDxWithMMV
{
    public class MyTriangle : RenderableInterface
    {
        SlimDX.Direct3D11.VertexBufferBinding bufferBinding;
        Buffer vertexBuffer; 
        DataStream vertices;
        private int triangleCount = 3;
        Vector3 position = new Vector3(0f, 0f, 0f);
        public Vector3 getPosition()
        {
            return position;
        }

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

        public Texture2D getTexture()
        {
            return null;
        }

        public System.String getShader()
        {
            return null;
        }

        public void setPosition(Vector3 pos)
        {
        }

        bool RenderableInterface.useBlending()
        {
            return false;
        }
    }
}
