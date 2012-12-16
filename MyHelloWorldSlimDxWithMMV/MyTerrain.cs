using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;

namespace MyHelloWorldSlimDxWithMMV
{
    class MyTerrain : RenderableInterface
    {
        String shader = "terrain.fx";
        SlimDX.Direct3D11.VertexBufferBinding bufferBinding;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        DataStream vertices;
        private static int length = 64;
        private static int width = 64;
        private static int triangleCount = MyTerrain.length * MyTerrain.width* 2;
        private HeightMap heightMap;
        
        /*
        private float getHeight(int i,int j,float max)
        {
            if (i == width)
                i = 0;
            if (j == length)
                j = 0;
            int imap = i * mapSize / width;
            int jmap = j * mapSize / length;
            ushort value = heightMap[jmap * mapSize + imap];

            return (float)value * max / (float)0xffff;      
        }
        */

        Vector3 position = new Vector3(0f, 0f, 0f);
        public Vector3 getPosition()
        {
            return position;
        }
        public void setPosition(Vector3 pos)
        {
            position = pos;
        }

        public MyTerrain(HeightMap heightMap,int size)
        {
            MyTerrain.width = size;
            MyTerrain.length = size;
            this.heightMap = heightMap;


            vertices = new DataStream((12+8) * triangleCount * 3, true, true);
            for (int i = 0; i < width;i++)
                for (int j = 0; j < length; j++)
                {
                    float height;
                    height = heightMap.getHeight((float)i, (float)j);
                    vertices.Write(new Vector3(0.0f + i, height, 0.0f + j));
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    height = heightMap.getHeight((float)i, (float)j+1f);
                    vertices.Write(new Vector3(0.0f + i, height, 1.0f + j));
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    height = heightMap.getHeight((float)i+1, (float)j);
                    vertices.Write(new Vector3(1.0f + i, height, 0.0f + j));
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    height = heightMap.getHeight((float)i, (float)j + 1);
                    vertices.Write(new Vector3(0.0f + i, height, 1.0f + j));
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    height = heightMap.getHeight((float)i + 1, (float)j + 1);
                    vertices.Write(new Vector3(1.0f + i, height, 1.0f + j));
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    height = heightMap.getHeight((float)i + 1, (float)j);
                    vertices.Write(new Vector3(1.0f + i, height, 0.0f + j));
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    
                }
            vertices.Position = 0;
        }


        SlimDX.Direct3D11.VertexBufferBinding RenderableInterface.getVertices()
        {
            return bufferBinding;
        }

        void RenderableInterface.dispose()
        {
            vertices.Close();
            vertexBuffer.Dispose();
        }

        int RenderableInterface.getTriangleCount()
        {
            return triangleCount;
        }

        void RenderableInterface.initBuffers(SlimDX.Direct3D11.Device device)
        {
            vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, (12 + 8) * triangleCount * 3, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            bufferBinding = new VertexBufferBinding(vertexBuffer, 20, 0);
        }

        public Texture2D getTexture()
        {
            return null;
        }

        public String getShader()
        {
            return this.shader;
        }

        bool RenderableInterface.useBlending()
        {
            return false;
        }
    }
}
