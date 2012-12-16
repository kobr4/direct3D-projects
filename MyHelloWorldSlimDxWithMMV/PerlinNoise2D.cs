using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyHelloWorldSlimDxWithMMV
{
    class PerlinNoise2D
    {
        private static Random r = new Random();

        private static int getMapValue(int i, int j, ushort[] map, int size)
        {
            int x = i; 
            int y = j;
            if (i < 0)
                x = size - 1;
            else if (i == size)
                x = 0;

            if (j < 0)
                y = size - 1;
            else if (j == size)
                y = 0;
            return (int)map[y * size + x];
        }
        
        private static void smoothenMap(ushort[] map, int pow2)
        {
            int size = 1 << pow2;

            ushort[] map2 = new ushort[size * size];

            for(int i = 0; i < size;i++)
                for (int j = 0; j < size; j++)
                {
                    int value = getMapValue(i-1,j-1,map,size) +  getMapValue(i+1,j+1,map,size) +
                        getMapValue(i-1,j+1,map,size) + getMapValue(i+1,j-1,map,size)
                        + getMapValue(i, j - 1, map, size) + getMapValue(i, j + 1, map, size) +
                        getMapValue(i + 1, j, map, size) + getMapValue(i - 1, j, map, size);
                    map2[j*size + i] = (ushort)(value >> 3);
                }

            map2.CopyTo(map,0);
        }

        private static ushort[] generateMap(int pow2)
        {
            int size = 1 << pow2;
            ushort[] output = new ushort[size * size];
            for (int i = 0; i < size * size; i++)
            {
                output[i] = (ushort)((float)0xffff * r.NextDouble());
            }
            return output;
        }

        private static void mergeMap(ushort[] dstMap, int dstPow2, ushort[] srcMap, int srcPow2,int coef)
        {
            int dstSize = 1 << dstPow2;
            int srcSize = 1 << srcPow2;
            for (int i = 0;i < dstSize;i++)
                for (int j = 0; j < dstSize; j++)
                {
                    int iSrc = i * srcSize / dstSize;
                    int jSrc = j * srcSize / dstSize;

                    dstMap[i + j * dstSize] = (ushort)(dstMap[i + j * dstSize] / coef + srcMap[iSrc + jSrc * srcSize] / coef);
                }
        }


        public ushort[] createMap(int pow2)
        { 
            ushort[] map = generateMap(pow2);
            smoothenMap(map,pow2);

            for (int i = pow2 - 1; i > 2; i--)
            {
                ushort[] map2 = generateMap(i);
          
                mergeMap(map, pow2, map2, i, 2);
                smoothenMap(map, pow2);
            }
             
            return map;
        }

        private void threshold(ushort[] map, int pow2, ushort value) 
        {
            int size = 1 << pow2;
            for (int i = 0; i < size * size; i++)
            {
                if (map[i] < value)
                    map[i] = 0;
            }
        }


        public ushort[] createMapUniform(int pow2,int value)
        {
            ushort[] map = generateMap(pow2);
            smoothenMap(map, pow2);
            threshold(map, pow2, 34000);
            //smoothenMap(map, pow2);
            return map;
        }
    }
}
