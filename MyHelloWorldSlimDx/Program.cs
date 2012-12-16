using System.Windows.Forms;

namespace MyHelloWorldSlimDx
{
    static class Program
    {
        static void Main()
        {
            /*
            // create test vertex data, making sure to rewind the stream afterward
            var vertices = new DataStream(12 * 3, true, true);
            vertices.Write(new Vector3(0.0f, 0.5f, 0.5f));
            vertices.Write(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f));
            vertices.Position = 0;

            // create the vertex layout and buffer
            
            
            var vertexBuffer = new Buffer(device, vertices, 12 * 3, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            // clean up all resources
            // anything we missed will show up in the debug output
            vertices.Close();
            vertexBuffer.Dispose();
             */
            Renderer renderer = new Renderer();
            renderer.init();
            
            MyTriangle mytriangle = new MyTriangle();
            renderer.addRenderable(mytriangle);

            renderer.start();
            renderer.dispose();
        }
    }
}
