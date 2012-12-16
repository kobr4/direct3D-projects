using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using SlimDX;

namespace MyHelloWorldSlimDxWithMMV
{
    public interface RenderableInterface
    {
        VertexBufferBinding getVertices(); 
        void dispose();
        int getTriangleCount();
        void initBuffers(Device device);
        Texture2D getTexture();
        String getShader();
        Vector3 getPosition();
        void setPosition(Vector3 pos);
        bool useBlending();
    }
}
