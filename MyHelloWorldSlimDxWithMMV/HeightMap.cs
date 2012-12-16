using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MyHelloWorldSlimDxWithMMV
{
    class HeightMap
    {
        private ushort[] heightMap;
        private int pow2;
        private int size;
        private float rangeMin;
        private float rangeMax;
        private static ushort[] createReverseXMap(ushort[] heightMap, int pow2)
        {
            int size = 1 << pow2;
            ushort[] dest = new ushort[size * size];
            for (int i = 0;i < size;i++)
                for (int j = 0; j < size; j++)
                {
                    dest[i + (size-1-j)*size] = heightMap[i + j * size];
                }
            return dest;
        }
        private static ushort[] createReverseYMap(ushort[] heightMap, int pow2)
        {
            int size = 1 << pow2;
            ushort[] dest = new ushort[size * size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    dest[(size-1-i) + j * size] = heightMap[i + j * size];
                }
            return dest;
        }

        public HeightMap createMirrorX()
        {
            HeightMap map = new HeightMap();
            ushort[] backmap = createReverseXMap(heightMap,pow2);
            map.setBackend(backmap, pow2, rangeMin, rangeMax);
            return map;
        }

        public HeightMap createMirrorY()
        {
            HeightMap map = new HeightMap();
            ushort[] backmap = createReverseYMap(heightMap, pow2);
            map.setBackend(backmap, pow2, rangeMin, rangeMax);
            return map;
        }

        public static HeightMap generateHeightMap(int pow2, float rangeMin, float rangeMax)
        {
            PerlinNoise2D noise = new PerlinNoise2D();
            ushort[] heightMap = noise.createMap(6);
            HeightMap map = new HeightMap();
            map.setBackend(heightMap, pow2, rangeMin, rangeMax);
            return map;
        }

        private void setBackend(ushort[] map, int pow2, float rangeMin, float rangeMax)
        {
            this.heightMap = map;
            this.pow2 = pow2;
            this.size = 1 << pow2;
            this.rangeMin = rangeMin;
            this.rangeMax = rangeMax;
        }

        public float getHeight(float x,float y)
        {
            return HeightMap.getHeight(heightMap, size, x, y, rangeMax - rangeMin) + rangeMin;

        }

        private static float getHeightInt(ushort[] heightMap, int mapSize, int i, int j, float max)
        {
            if (i >= mapSize)
                i = i%mapSize;
            if (j >= mapSize)
                j = j % mapSize;
            ushort value = heightMap[j * mapSize + i];
            return (float)value * max / (float)0xffff;
        }


        private static bool isOnTriange(float x1, float y1, float x2, float y2, float x3, float y3, float x, float y)
        {
            float a;
            float b;
            bool s;
            bool s2;
            //----------------------------
            if (x2 - x1 != 0.0f)
            {
                a = (y2 - y1) / (x2 - x1);
                b = y1 - a * x1;

                if (a * x3 + b > y3)
                    s = true;
                else s = false;


                if (a * x + b > y)
                    s2 = true;
                else s2 = false;

                if ((s != s2) && (a * x + b != y))
                    return false;

            }
            else
            {
                if (x1 > x3)
                    s = true;
                else s = false;

                if (x1 > x)
                    s2 = true;
                else s2 = false;

                if ((s != s2) && (x1 != x))
                    return false;
            }

            //-----------------------------
            if (x3 - x2 != 0.0f)
            {
                a = (y3 - y2) / (x3 - x2);
                b = y2 - a * x2;

                if (a * x1 + b > y1)
                    s = true;
                else s = false;


                if (a * x + b > y)
                    s2 = true;
                else s2 = false;

                if ((s != s2) && (a * x + b != y))
                    return false;

            }
            else
            {
                if (x2 > x1)
                    s = true;
                else s = false;

                if (x2 > x)
                    s2 = true;
                else s2 = false;

                if ((s != s2) && (x1 != x))
                    return false;
            }

            //-----------------------------        
            if (x1 - x3 != 0.0f)
            {
                a = (y1 - y3) / (x1 - x3);
                b = y3 - a * x3;

                if (a * x2 + b > y2)
                    s = true;
                else s = false;



                if (a * x + b > y)
                    s2 = true;
                else s2 = false;

                if ((s != s2) && (a * x + b != y))
                    return false;

            }
            else
            {
                if (x1 > x2)
                    s = true;
                else s = false;

                if (x1 > x)
                    s2 = true;
                else s2 = false;

                if ((s != s2) && (x1 != x))
                    return false;
            }
            return true;

        }

        private static float getHeight(ushort[] heightMap, int mapSize, float x, float y, float max)
        {
            Vector3 p1;
            Vector3 p2;
            Vector3 p3;
            
            if (isOnTriange((int)x, (int)y, (int)x + 1, (int)y, (int)x, (int)y + 1, x, y))
            {
                p1 = new Vector3((int)x, getHeightInt(heightMap, mapSize, (int)x, (int)y, max), (int)y);
                p2 = new Vector3((int)x, getHeightInt(heightMap, mapSize, (int)x, (int)y+1, max), (int)y + 1);
                p3 = new Vector3((int)x + 1, getHeightInt(heightMap, mapSize, (int)x+1, (int)y, max), (int)y);
            }
            else
            {
                p2 = new Vector3((int)x, getHeightInt(heightMap, mapSize, (int)x, (int)y+1, max), (int)y + 1);
                p1 = new Vector3((int)x + 1, getHeightInt(heightMap, mapSize, (int)x+1, (int)y+1, max), (int)y + 1);
                p3 = new Vector3((int)x + 1, getHeightInt(heightMap, mapSize, (int)x+1, (int)y, max), (int)y);
            }
            

            Vector3 normal = getNormal(p1, p2, p3);
            float h = getHeight(normal, p1, x, y);
            return h;
        }

        private static float getHeight(Vector3 normal, Vector3 p1, float x, float z)
        {
            return -1.0f / normal.Y * ((x - p1.X) * normal.X + (z - p1.Z) * normal.Z) + p1.Y;
        }

        private static Vector3 getNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1);
            normal.Normalize();
            return normal;
        }
    }
}
