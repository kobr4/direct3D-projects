using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Multimedia;
using SlimDX.RawInput;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyHelloWorldSlimDxWithMMV
{
    public class Camera
    {
        private Vector3 mPosition = new Vector3();
        private float[] mYawpitchroll = new float[3];
        private Vector3 mForward = new Vector3();
        private Boolean isFollowPath;
        private long currentPathTime;
        private List<Vector3> mListPos;
        public void setPosition(float x, float y, float z)
        {
            this.mPosition[0] = x;
            this.mPosition[1] = y;
            this.mPosition[2] = z;
        }

        public Vector3 getPosition()
        {
            return mPosition;
        }


        public void setRotateX(float x)
        {
            this.mYawpitchroll[0] = x;
        }

        public void setRotateY(float y)
        {
            this.mYawpitchroll[1] = y;
        }

        public void setRotateZ(float z)
        {
            this.mYawpitchroll[2] = z;
        }

        public float getRotateX()
        {
            return this.mYawpitchroll[0];
        }

        public float getRotateY()
        {
            return this.mYawpitchroll[1];
        }

        public float getRotateZ()
        {
            return this.mYawpitchroll[2];
        }
        private Vector3 computePosition(List<Vector3> pos, long totaltime,int timeByPos)
        {
            int currentPos = (int)((totaltime / timeByPos) % pos.Count);
            int antePos = currentPos - 1;
            if (currentPos == 0)
                antePos = pos.Count - 1;
            Vector3 p1 = pos[antePos];
            Vector3 p2 = pos[currentPos];
            Vector3 p3 = pos[(currentPos + 1) % pos.Count];
            Vector3 p4 = pos[(currentPos + 2) % pos.Count];
            int delta = (int)(totaltime % timeByPos);
            float t = (float)delta / (float)timeByPos;
            Vector3 computedPos = Vector3.CatmullRom(p1, p2, p3, p4,t);
            return computedPos;
        }
        private void startPath()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Vector3>));
            System.IO.FileStream fs = new System.IO.FileStream("outputFile.txt", System.IO.FileMode.Open);
            this.mListPos = (List<Vector3>)serializer.Deserialize(fs);
            fs.Close();
            this.isFollowPath = true;
            this.currentPathTime = 0;
            Renderer.DebugLog("Starting auto-camera");
        }

        private void stopPath()
        {
            this.isFollowPath = false;
            Renderer.DebugLog("Stopping auto-camera");
        }

        public void updateCamera(int dt)
        {
            
            if (this.isFollowPath)
            {
                this.currentPathTime += dt;
                this.mPosition = computePosition(this.mListPos,this.currentPathTime,10000);
            }
        }

        private void writePositionToFile()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Vector3>));
            List<Vector3> pos = new List<Vector3>();
            if (System.IO.File.Exists("outputFile.txt"))
            {
                System.IO.FileStream fs = new System.IO.FileStream("outputFile.txt", System.IO.FileMode.Open);
                pos = (List < Vector3 >)serializer.Deserialize(fs);
                fs.Close();
            }
            pos.Add(this.getPosition());
            System.IO.FileStream fs2 = new System.IO.FileStream("outputFile.txt", System.IO.FileMode.OpenOrCreate);
            serializer.Serialize(fs2, pos);
            fs2.Close();
            Renderer.DebugLog("Writing camera position to outputFile.txt " + pos.Count + " elements");
        }

        public Camera()
        {
            SlimDX.RawInput.Device.RegisterDevice(UsagePage.Generic, UsageId.Keyboard, DeviceFlags.None);
            SlimDX.RawInput.Device.RegisterDevice(UsagePage.Generic, UsageId.Mouse, DeviceFlags.None);

            SlimDX.RawInput.Device.KeyboardInput += Device_KeyboardInput;
            SlimDX.RawInput.Device.MouseInput += Device_MouseInput;

            mForward = new Vector3(0f, 0f, 0.5f);
        }
        public static Vector3 Matrix33Xvector(Matrix matrix3x3, Vector3 vector)
        {
            Vector3 product = new Vector3();

            product.X = matrix3x3.M11 * vector.X + matrix3x3.M12 * vector.Y + matrix3x3.M13 * vector.Z;
            product.Y = matrix3x3.M21 * vector.X + matrix3x3.M22 * vector.Y + matrix3x3.M23 * vector.Z;
            product.Z = matrix3x3.M31 * vector.X + matrix3x3.M32 * vector.Y + matrix3x3.M33 * vector.Z;
            return product;
        }
        private Vector3 getForwardVector()
        {
            Quaternion q1 = new Quaternion(new Vector3(0, 1, 0), this.mYawpitchroll[1]);
            Quaternion q2 = new Quaternion(new Vector3(1, 0, 0), this.mYawpitchroll[0]);
            Quaternion q3 = new Quaternion(new Vector3(0, 0, 1), 0);
            Matrix m = Matrix.RotationQuaternion(Quaternion.Normalize(q1 * q2 * q3));
            return Matrix33Xvector(m, this.mForward);
                
        }

        private void Device_MouseInput(object sender, MouseInputEventArgs e)
        {
            //process input here
            
            switch (e.ButtonFlags)
            {
                case MouseButtonFlags.None :

                    if (0 < e.X)
                        this.mYawpitchroll[1] = this.mYawpitchroll[1] + 0.01f;
                    else
                        if (0 > e.X)
                            this.mYawpitchroll[1] = this.mYawpitchroll[1] - 0.01f;
                        
                    if (0 < e.Y)
                        this.mYawpitchroll[0] -= 0.01f;
                    if (0 > e.Y)
                        this.mYawpitchroll[0] += 0.01f;
                    
                    break;
            }
            /*
            if (this.mYawpitchroll[1] > (float)Math.PI)
            {
                this.mYawpitchroll[1] = -(float)Math.PI;
            }
            if (this.mYawpitchroll[1] < -(float)Math.PI)
            {
                this.mYawpitchroll[1] = (float)Math.PI;
            }

            if (this.mYawpitchroll[0] > (float)Math.PI)
            {
                this.mYawpitchroll[0] = -(float)Math.PI;
            }
            if (this.mYawpitchroll[0] < -(float)Math.PI)
            {
                this.mYawpitchroll[0] = (float)Math.PI;
            }
            */
        }
       
        private void Device_KeyboardInput(object sender, KeyboardInputEventArgs e)
        {
            //process input here
            switch (e.Key)
            {
                case System.Windows.Forms.Keys.Up :

                    this.mPosition += getForwardVector();
                    break;
                case System.Windows.Forms.Keys.Down:
                    this.mPosition -= getForwardVector();
                    break;
                case System.Windows.Forms.Keys.Left:
                    this.mPosition[0] -= 1f;
                    break;
                case System.Windows.Forms.Keys.Right:
                    this.mPosition[0] += 1f;
                    break;
                case System.Windows.Forms.Keys.O:
                    if (e.State == KeyState.Released)
                    {
                        Renderer.DebugLog("Toto");
                    }
                    break;

                case System.Windows.Forms.Keys.P:
                    if (e.State == KeyState.Released)
                    {
                        this.writePositionToFile();
                    }
                    break;
                case System.Windows.Forms.Keys.A:
                    if (e.State == KeyState.Released)
                    {
                        if (this.isFollowPath == false)
                            this.startPath();
                        else this.stopPath();
                    }
                    break;
            }
        }

        public Matrix generateViewMatrix()
        {
            Quaternion q1 = new Quaternion(new Vector3(0, 1, 0), this.mYawpitchroll[1]);
            Quaternion q2 = new Quaternion(new Vector3(1, 0, 0), this.mYawpitchroll[0]);
            //Quaternion q2 = new Quaternion(new Vector3(1, 0, 0), 0);
            Quaternion q3 = new Quaternion(new Vector3(0, 0, 1), 0);
            // Order of operations:
            // First pitch up/down, then rotate around up, 
            // then translate by position. Note that you want 
            // the inverse transform of the camera's transform, 
            // as it goes from the world, to the camera.

            Matrix m = Matrix.Translation(-this.getPosition()) * Matrix.RotationQuaternion(Quaternion.Normalize((q1 * q2 * q3)));
            //Matrix m = Matrix.Translation(this.getPosition()) * Matrix.RotationY(this.mYawpitchroll[1]) * Matrix.RotationX(this.mYawpitchroll[0]);
            
            return m;
        }
    }
}
