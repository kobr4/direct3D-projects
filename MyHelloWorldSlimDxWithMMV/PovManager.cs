using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MyHelloWorldSlimDxWithMMV
{
    public class PovManager
    {
        public class RenderableStruct
        {
            public List<RenderableInterface> renderableList = new List<RenderableInterface>();
            public Vector3 position = new Vector3();
        }

        List<RenderableStruct> mOutputList = new List<RenderableStruct>();
        List<RenderableInterface> mOutputRenderableList = new List<RenderableInterface>();
        RenderableInterface mSkyBox = null;
        RenderableStruct[] mGridLayout;

        int size;
        float physicalSize;
        public PovManager(int size,float physicalSize)
        {
            mGridLayout = new RenderableStruct[size * size];
            for (int i = 0; i < size * size; i++)
                mGridLayout[i] = new RenderableStruct();
            this.size = size;
            this.physicalSize = physicalSize;
        }

        public void addSkybox(RenderableInterface renderable)
        {
            mSkyBox = renderable;
        }


        public void addRenderable(RenderableInterface renderable,int i,int j)
        {
            mGridLayout[i + j * size].renderableList.Add(renderable);
            mGridLayout[i + j * size].position.X = i * physicalSize;
            mGridLayout[i + j * size].position.Z = j * physicalSize;
        }

        private RenderableStruct getRenderableList(int i, int j)
        {
            int bi = i;
            int bj = j;
            if (i < 0)
                bi = i + ((-i / size)  + 1)*size;
            if (j < 0)
                bj = j + ((-j / size) + 1) * size;
            mGridLayout[(bi % size) + (bj % size) * size].position.X = i * physicalSize;
            mGridLayout[(bi % size) + (bj % size) * size].position.Z = j * physicalSize;
            //Renderer.DebugLog("DRAWING AT : "+bi+"-"+bj+" "+mGridLayout[(bi % size) + (bj % size) * size].position);
            return mGridLayout[(bi%size) + (bj%size) * size];
        }

        public List<RenderableInterface> getRenderableList()
        {
            mOutputRenderableList.Clear();
            for (int i = 0; i < size * size; i++)
            {
                mOutputRenderableList.AddRange(mGridLayout[i].renderableList);
            }
            return mOutputRenderableList;
        }

        public List<RenderableStruct> getRenderableList(Vector3 position)
        {
            mOutputList.Clear();
            int i = (int)(position.X / physicalSize);
            int j = (int)(position.Z / physicalSize);
            if (position.X < 0f)
                i = i - 1;
            if (position.Z < 0f)
                j = j - 1;

            mOutputList.Add(getRenderableList(i, j));    
            
            mOutputList.Add(getRenderableList(i+1, j+1));
            mOutputList.Add(getRenderableList(i + 1, j));
            mOutputList.Add(getRenderableList(i, j + 1));

            mOutputList.Add(getRenderableList(i - 1, j - 1));
            mOutputList.Add(getRenderableList(i - 1, j));
            mOutputList.Add(getRenderableList(i, j - 1));

            if (mSkyBox != null)
            {
                Vector3 pos = mSkyBox.getPosition();
                Vector3 newpos = new Vector3();
                newpos.X = position.X;
                newpos.Z = position.Z;
                newpos.Y = pos.Y;
                mSkyBox.setPosition(newpos);
                //sbpostion.X = position.X;
                //sbpostion.Y = position.Y;
                //sbpostion.Z = position.Z;
            }
            return mOutputList;
        }


    }
}
