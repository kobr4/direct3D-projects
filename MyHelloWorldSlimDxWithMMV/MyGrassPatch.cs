using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;

namespace MyHelloWorldSlimDxWithMMV
{
    class MyGrassPatch : RenderableInterface
    {
        SlimDX.Direct3D11.VertexBufferBinding bufferBinding;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        DataStream vertices;
        public Texture2D grassTexture;
        private static String shader = "grass.fx";
        private static int length = 2;
        private static int width = 2;
        private int triangleCount = 0;
        int nbSegment = 3;
        int bladeBySquare = 4;
        float bladeHalfWidth = 0.08f;
        private HeightMap heightMap;
        Vector3 position = new Vector3(0f, 0f, 0f);
        public Vector3 getPosition()
        {
            return position;
        }
        public void setPosition(Vector3 pos)
        {
            position = pos;
        }


        public static Vector3 Matrix33Xvector(Matrix matrix3x3, Vector3 vector)
        {
            Vector3 product = new Vector3();
            product.X = matrix3x3.M11 * vector.X + matrix3x3.M12 * vector.Y+ matrix3x3.M13 * vector.Z;
            product.Y = matrix3x3.M21 * vector.X + matrix3x3.M22 * vector.Y+ matrix3x3.M23 * vector.Z;
            product.Z = matrix3x3.M31 * vector.X + matrix3x3.M32 * vector.Y+ matrix3x3.M33 * vector.Z;
            return product;
        }

        private void addBlade(float x, float y, float z, float bladeHeight, float angle)
        {
            //float height = 0.5f;
            Matrix m = Matrix.RotationY(angle);
            //Matrix m = Matrix.Identity;
            Vector3 translate = new Vector3(x, y, z);
            for (int i = 0; i < nbSegment; i++)
            {
                if (i == nbSegment - 1)
                {
                    
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f + bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f, i * bladeHeight / nbSegment + bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));

                    //Other side
                    
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f, i * bladeHeight / nbSegment + bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(0f, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                     
                    
                }
                else
                {
                    
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));

                    //Other side
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)i / (float)nbSegment));

                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)i / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(bladeHalfWidth, i * bladeHeight / nbSegment + bladeHeight / nbSegment + 0.0f, 0f)));
                    vertices.Write(new Vector2(1.0f, (float)(i + 1) / (float)nbSegment));
                    vertices.Write(translate + Matrix33Xvector(m,new Vector3(-bladeHalfWidth, i * bladeHeight / nbSegment + bladeHeight / nbSegment, 0f)));
                    vertices.Write(new Vector2(0.0f, (float)(i + 1) / (float)nbSegment));
                    
                }
                 
            }

        }

        public MyGrassPatch(HeightMap heightMap, int size)
        {
            MyGrassPatch.width = size;
            MyGrassPatch.length = size;
            this.heightMap = heightMap;
            System.Random rSeed = new Random();
            
            //triangleCount = length * width * bladeBySquare * bladeBySquare * nbSegment * 2 * 4 * 2; 
            triangleCount = length * width * bladeBySquare * bladeBySquare * nbSegment * 4; 

            vertices = new DataStream((12+8) * triangleCount * 3, true, true);
            for (int i = 0; i < width;i++)
                for (int j = 0; j < length; j++)
                {
                    for (int xb = 0;xb < bladeBySquare;xb++)
                        for (int yb = 0; yb < bladeBySquare; yb++)
                        {
                            float x = i + xb * 1f / bladeBySquare + ((float)rSeed.NextDouble()-0.5f) * 0.2f;
                            float z = j + yb * 1f / bladeBySquare + ((float)rSeed.NextDouble() - 0.5f) * 0.2f;
                            //float x = i + xb * 1f / bladeBySquare;
                            //float z = j + yb * 1f / bladeBySquare;
                            addBlade(x, heightMap.getHeight(x, z), z, 1.0f + (float)rSeed.NextDouble() * 0.2f, (float)(rSeed.NextDouble()*Math.PI));
                        }
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

        public Texture2D getTexture()
        {
            return this.grassTexture;
        }

        void RenderableInterface.initBuffers(SlimDX.Direct3D11.Device device)
        {
            vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, (12 + 8)* triangleCount * 3 , ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            bufferBinding = new VertexBufferBinding(vertexBuffer, 20, 0);
            grassTexture = Texture2D.FromFile(device,"grass2.bmp");

        }

        public System.String getShader()
        {
            return MyGrassPatch.shader;
        }

        bool RenderableInterface.useBlending()
        {
            return false;
        }

    }
}
