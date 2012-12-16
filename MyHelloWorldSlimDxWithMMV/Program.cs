using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SlimDX;

namespace MyHelloWorldSlimDxWithMMV
{
    static class Program
    {
        static void Main()
        {
            Renderer renderer = new Renderer();
            renderer.init();
            
            PovManager manager = new PovManager(2,64f);

            HeightMap heightMap = HeightMap.generateHeightMap(6, -5.0f, 5.0f);
            MyTerrain myTerrain = new MyTerrain(heightMap,64);
            MyGrassPatch myGrassPatch = new MyGrassPatch(heightMap,64);

            manager.addRenderable(myTerrain, 0, 0);
            manager.addRenderable(myGrassPatch, 0, 0);
 
            HeightMap heightMapBottom = heightMap.createMirrorX();
            MyTerrain myTerrainBottom = new MyTerrain(heightMapBottom, 64);
            myTerrainBottom.setPosition(new Vector3(0f, 0f, 64f));
            MyGrassPatch myGrassPatchBottom = new MyGrassPatch(heightMapBottom, 64);
            myGrassPatchBottom.setPosition(new Vector3(0f, 0f, 64f));

            manager.addRenderable(myTerrainBottom, 0, 1);
            manager.addRenderable(myGrassPatchBottom, 0, 1);

            HeightMap heightMapBottomRight = heightMapBottom.createMirrorY();
            MyTerrain myTerrainBottomRight = new MyTerrain(heightMapBottomRight, 64);
            myTerrainBottomRight.setPosition(new Vector3(64f, 0f, 64f));
            MyGrassPatch myGrassPatchBottomRight = new MyGrassPatch(heightMapBottomRight, 64);
            myGrassPatchBottomRight.setPosition(new Vector3(64f, 0, 64f));

            manager.addRenderable(myTerrainBottomRight, 1, 1);
            manager.addRenderable(myGrassPatchBottomRight, 1, 1);
            
            HeightMap heightMapLeft = heightMap.createMirrorY();
            MyTerrain myTerrainLeft = new MyTerrain(heightMapLeft, 64);
            myTerrainLeft.setPosition(new Vector3(64f, 0f, 0));
            MyGrassPatch myGrassPatchLeft = new MyGrassPatch(heightMapLeft, 64);
            myGrassPatchLeft.setPosition(new Vector3(64f, 0f, 0f));

            manager.addRenderable(myTerrainLeft, 1, 0);
            manager.addRenderable(myGrassPatchLeft, 1, 0);

            SkyDome skydome = new SkyDome();
            manager.addSkybox(skydome);

            renderer.addPovManager(manager);
            renderer.addRenderable(skydome);
            renderer.start();
            renderer.dispose();
        }
    }
}
