using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;

namespace MyHelloWorldSlimDxWithMMV
{
    class SkyDome : RenderableInterface
    {
        SlimDX.Direct3D11.VertexBufferBinding bufferBinding;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        DataStream vertices;
        String shader = "skydome.fx";
        private static readonly int nbSlices = 59;
        private static readonly int nbStairs = 10;
        private float radius = 50.0f;
        private float baseratio = 2.0f;
        private static readonly int triangleCount = nbSlices * nbStairs * 2;
        private Vector3 position = new Vector3(0f, -20f, 0f);
        private Texture2D skyDomeTexture;

        public SkyDome()
        {
            vertices = new DataStream((12+8) * triangleCount * 3 , true, true);
            for (int i = 0; i < nbStairs; i++)
                for (int j = 0; j < nbSlices; j++)
                {
                    double alpha = (double)Math.PI / (nbStairs+1) * i * 0.5f;
                    double beta = (double)Math.PI*2 / nbSlices * j;
                    double alpha1 = (double)Math.PI / (nbStairs+1) * (i + 1) * 0.5f;
                    double beta1 = (double)Math.PI*2 / nbSlices * (j + 1);

                   
                    Vector3 a = new Vector3(baseratio * radius * (float)Math.Sin(alpha) * (float)Math.Cos(beta), radius * (float)Math.Cos(alpha), baseratio * radius * (float)Math.Sin(alpha) * (float)Math.Sin(beta));
                    Vector3 b = new Vector3(baseratio * radius * (float)Math.Sin(alpha) * (float)Math.Cos(beta1), radius * (float)Math.Cos(alpha), baseratio * radius * (float)Math.Sin(alpha) * (float)Math.Sin(beta1));
                    Vector3 c = new Vector3(baseratio * radius * (float)Math.Sin(alpha1) * (float)Math.Cos(beta1), radius * (float)Math.Cos(alpha1), baseratio * radius * (float)Math.Sin(alpha1) * (float)Math.Sin(beta1));
                    Vector3 d = new Vector3(baseratio * radius * (float)Math.Sin(alpha1) * (float)Math.Cos(beta), radius * (float)Math.Cos(alpha1), baseratio * radius * (float)Math.Sin(alpha1) * (float)Math.Sin(beta));
                    /*
                    vertices.Write(a);
                    vertices.Write(new Vector2(0.0f,0.0f));
                    vertices.Write(b);
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    vertices.Write(c);
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    vertices.Write(a);
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    vertices.Write(c);
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    vertices.Write(d);
                    vertices.Write(new Vector2(0.0f, 0.0f));
                    */

                    
                    
                    vertices.Write(c);
                    //vertices.Write(new Vector2((float)(i + 1) / (float)nbStairs, (float)(j + 1) / (float)nbSlices));
                    vertices.Write(new Vector2((float)(j + 1) / (float)nbSlices,(float)(i + 1) / (float)nbStairs));
                    vertices.Write(b);
                    //vertices.Write(new Vector2((float)(i + 0) / (float)nbStairs, (float)(j + 1) / (float)nbSlices));
                    vertices.Write(new Vector2((float)(j + 1) / (float)nbSlices,(float)(i + 0) / (float)nbStairs));
                    vertices.Write(a);
                    //vertices.Write(new Vector2((float)(i + 0) / (float)nbStairs, (float)(j + 0) / (float)nbSlices));
                    vertices.Write(new Vector2((float)(j + 0) / (float)nbSlices, (float)(i + 0) / (float)nbStairs));
                    vertices.Write(d);
                    //vertices.Write(new Vector2((float)(i + 1) / (float)nbStairs, (float)(j + 0) / (float)nbSlices));
                    vertices.Write(new Vector2((float)(j + 0) / (float)nbSlices, (float)(i + 1) / (float)nbStairs));
                    vertices.Write(c);
                    //vertices.Write(new Vector2((float)(i + 1) / (float)nbStairs, (float)(j + 1) / (float)nbSlices));
                    vertices.Write(new Vector2((float)(j + 1) / (float)nbSlices,(float)(i + 1) / (float)nbStairs));
                    vertices.Write(a);
                    //vertices.Write(new Vector2((float)(i + 0) / (float)nbStairs, (float)(j + 0) / (float)nbSlices));
                    vertices.Write(new Vector2((float)(j + 0) / (float)nbSlices,(float)(i + 0) / (float)nbStairs));
                }

            vertices.Position = 0;
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

        public void initBuffers(SlimDX.Direct3D11.Device device)
        {
            vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, (12 + 8) * triangleCount * 3, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            bufferBinding = new VertexBufferBinding(vertexBuffer, 20, 0);
            skyDomeTexture = Texture2D.FromFile(device, "skydome3.bmp");

            /*
            PerlinNoise2D perlinNoise = new PerlinNoise2D();
            ushort[] map = perlinNoise.createMapUniform(10, 16000);

            int mapWidth = 1024;
                 
            Texture2DDescription desc = new Texture2DDescription();
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = SlimDX.DXGI.Format.B8G8R8A8_UNorm;
            desc.Height = mapWidth;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.Usage = ResourceUsage.Immutable;
            desc.Width = mapWidth;
            desc.ArraySize = 1;
            desc.SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);

            byte[] bufferImage = new byte[4 * mapWidth * mapWidth];
            for (int i = 0; i < mapWidth * mapWidth; i++)
            {
                bufferImage[i * 4] = (byte)( (map[i]) >> 8);
                bufferImage[i * 4 + 1] = (byte)( (map[i]) >> 8);
                bufferImage[i * 4 + 2] = (byte)( (map[i]) >> 8);
                bufferImage[i * 4 + 3] = 255;
            }

            DataStream ds = new DataStream(4 * mapWidth * mapWidth, true, true);
            ds.Write(bufferImage,0,bufferImage.Length);
            ds.Position = 0;
            DataRectangle datarect = new DataRectangle(4 * mapWidth, ds);
            skyDomeTexture = new Texture2D(device, desc,datarect);
            */

            //System.Runtime.InteropServices.GCHandle pinnedArray = System.Runtime.InteropServices.GCHandle.Alloc(bufferImage, System.Runtime.InteropServices.GCHandleType.Pinned);
            //IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            //using (System.Drawing.Bitmap image = new System.Drawing.Bitmap(mapWidth, mapWidth, mapWidth * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, pointer))
            //{
            //    image.Save("toto.bmp");
            //}
            //pinnedArray.Free();
    

        }

        public SlimDX.Direct3D11.Texture2D getTexture()
        {
            return skyDomeTexture;
        }

        public string getShader()
        {
            return this.shader;
        }

        public SlimDX.Vector3 getPosition()
        {
            return position;
        }

        public void setPosition(Vector3 pos)
        {
            this.position = pos;
        }

        bool RenderableInterface.useBlending()
        {
            return true;
        }
    }
}
