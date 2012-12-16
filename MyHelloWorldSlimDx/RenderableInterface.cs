using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;

namespace MyHelloWorldSlimDx
{
    public interface RenderableInterface
    {
        VertexBufferBinding getVertices(); 
        void dispose();
        int getTriangleCount();
        void initBuffers(Device device);
    }
}
