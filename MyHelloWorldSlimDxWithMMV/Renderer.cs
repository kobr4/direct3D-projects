using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using System.Collections;
using System.Runtime.InteropServices;

namespace MyHelloWorldSlimDxWithMMV
{


    public class Renderer
    {
        struct Shader
        {
            public VertexShader vertexShader;
            public PixelShader pixelShader;
            public InputLayout layout;
        }
        Device device;
        SwapChain swapChain;

        System.Collections.Generic.Dictionary<String, Shader> shaderStore = new Dictionary<string,Shader>();
        RenderTargetView renderTarget;

        DeviceContext context;
        RenderForm form;
        List<RenderableInterface> renderableList = new List<RenderableInterface>();
        List<PovManager> povManagerList = new List<PovManager>();
        SlimDX.Direct3D11.Buffer inputBuffer;

        DepthStencilView depthStencilView;
        DepthStencilState depthStencilState;

        // Matrices
        Matrix worldMatrix = Matrix.Identity;
        Matrix viewMatrix = Matrix.Identity;
        Matrix finalMatrix = Matrix.Identity;
        const float fov = 0.8f;
        Matrix projectionMatrix;

        Camera camera;

        //Text rendering
        KeyedMutex mutexD3D10;
        KeyedMutex mutexD3D11;
        SlimDX.Direct2D.RenderTarget dwRenderTarget;
        Adapter1 adapter1;
        SlimDX.DirectWrite.TextFormat textFormat;
        SlimDX.DirectWrite.TextFormat textFormatDebug;
        BlendState BlendState_Transparent;
        SlimDX.Direct2D.SolidColorBrush brushSolidWhite;
        DataStream verticesText;
        SlimDX.Direct3D11.Texture2D textureD3D11;
        SlimDX.Direct3D11.Buffer vertexBufferText;
        // DirectX DXGI 1.1 factory
        Factory1 factory1;
        SlimDX.Direct3D11.VertexBufferBinding textbufferBinding;

        //[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        /*
        struct VertexPositionTexture
        {
            public Vector4 Position;
            public Vector2 TexCoord;
            public static readonly InputElement[] inputElements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("TEXCOORD",0,Format.R32G32_Float, 12 ,0)
            };
            public static readonly int SizeInBytes = Marshal.SizeOf(typeof(VertexPositionTexture));
            public VertexPositionTexture(Vector4 position, Vector2 texCoord)
            {
                Position = position;
                TexCoord = texCoord;
            }
            public VertexPositionTexture(Vector3 position, Vector2 texCoord)
            {
                Position = new Vector4(position, 1);
                TexCoord = texCoord;
            }
        }
         * */


        public void addShader(String shaderFile)
        {
            InputLayout layout;
            InputElement[] elements;
            ShaderSignature inputSignature;
            VertexShader vertexShader;
            PixelShader pixelShader;

            // load and compile the vertex shader
            using (ShaderBytecode bytecode = ShaderBytecode.CompileFromFile(shaderFile, "VShader", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);

                vertexShader = new VertexShader(device, bytecode);
            }


            // load and compile the pixel shader
            using (ShaderBytecode bytecode = ShaderBytecode.CompileFromFile(shaderFile, "PShader", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0), 
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0) };
            layout = new InputLayout(device, inputSignature, elements);

            Shader shader = new Shader();
            shader.layout = layout;
            shader.pixelShader = pixelShader;
            shader.vertexShader = vertexShader;

            shaderStore.Add(shaderFile,shader);
        }

        public void init()
        {
            form = new RenderForm("My HelloWorld SlimDX with Matrix Model View app");
            form.ClientSize = new System.Drawing.Size(1280, 720);
            form.Show();
            var description = new SwapChainDescription()
            {
                /*
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
                 */
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };


            //Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out swapChain);
            factory1 = new Factory1();
            
            // The 1st graphics adapter
            adapter1 = factory1.GetAdapter1(0);

            //SlimDX.Direct3D11.Device.CreateWithSwapChain(adapter1, DeviceCreationFlags.None, description, out device, out swapChain);
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, description, out device, out swapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created

            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            context = device.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);

            this.projectionMatrix = Matrix.PerspectiveFovLH(fov, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 1000.0f);



            var depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.D32_Float,
                Height = form.ClientSize.Height,
                Width = form.ClientSize.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            using (var depthBuffer = new Texture2D(device, depthBufferDesc))
            {
                depthStencilView = new DepthStencilView(device, depthBuffer);
            }

            var dssd = new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less
            };

            context.OutputMerger.SetTargets(depthStencilView,renderTarget);
            context.Rasterizer.SetViewports(viewport);
           


            
            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // handle alt+enter ourselves
            form.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == System.Windows.Forms.Keys.Enter)
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
            };

            // handle form size changes
            form.UserResized += (o, e) =>
            {
                renderTarget.Dispose();

                swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                    renderTarget = new RenderTargetView(device, resource);

                context.OutputMerger.SetTargets(renderTarget);
            };


            depthStencilState = DepthStencilState.FromDescription(device, dssd);

            BufferDescription inputBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = (16 * 4 +16),
                StructureByteStride = sizeof(float),
                Usage = ResourceUsage.Dynamic,
            };

            inputBuffer = new SlimDX.Direct3D11.Buffer(device, inputBufferDescription);

            camera = new Camera();
            camera.setPosition(0f, 0f, 5f);

            setupTextRendering();
        }

        long celapsed = 0;
        public void start()
        {
            MessagePump.Run(form, () =>
            {
                
                if (celapsed != 0)
                {
                    long d = DateTime.Now.Ticks - celapsed;
                    TimeSpan ts = new TimeSpan(d);

                    camera.updateCamera((int)ts.TotalMilliseconds);
                }
                celapsed = DateTime.Now.Ticks;
                    
                // clear the render target to a soothing blue
                context.ClearRenderTargetView(renderTarget, new Color4(0.5f, 0.5f, 1.0f));
                render();
                //camera.setRotateX(camera.getRotateX() + 0.001f);
                //Matrix rotationX = Matrix.RotationX(camera.getRotateX());
                //Matrix rotationY = Matrix.RotationY(camera.getRotateY());
                //Matrix rotationZ = Matrix.RotationZ(camera.getRotateZ());
                
                //Matrix rotation = Matrix.RotationYawPitchRoll(camera.getRotateY(), camera.getRotateX(), 0.0f);
                //Matrix translation = Matrix.Translation(camera.getPosition());

                //viewMatrix = translation * rotation;
                viewMatrix = camera.generateViewMatrix();
                
                foreach (RenderableInterface renderable in renderableList)
                {
                    drawRenderable(renderable,renderable.getPosition());
                }

                foreach (PovManager manager in povManagerList)
                {
                    drawPovManager(manager);
                }

                drawTextInTexture();

                // draw the triangle
                swapChain.Present(0, PresentFlags.None);
            });
        }

        public void drawRenderable(RenderableInterface renderable, Vector3 position)
        {
            if (!shaderStore.ContainsKey(renderable.getShader()))
            {
                this.addShader(renderable.getShader());
            }

            Shader shader = shaderStore[renderable.getShader()];
            context.InputAssembler.InputLayout = shader.layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.VertexShader.Set(shader.vertexShader);
            context.PixelShader.Set(shader.pixelShader);

            // Texture
            SamplerDescription descSampler = new SamplerDescription();
            descSampler.Filter = Filter.MinMagMipLinear;
            descSampler.AddressU = TextureAddressMode.Wrap;
            descSampler.AddressV = TextureAddressMode.Wrap;
            descSampler.AddressW = TextureAddressMode.Wrap;
            descSampler.MaximumAnisotropy = 1;
            descSampler.ComparisonFunction = Comparison.Always;
            descSampler.MaximumLod = float.MaxValue;
            SamplerState samplerLinear = SamplerState.FromDescription(device, descSampler);

            context.PixelShader.SetSampler(samplerLinear, 0);



            int time = (Environment.TickCount / 10) % 360;
            float costime = (float)Math.Cos(Math.PI / 180d * (double)time);
            float lineartime = (float)(Environment.TickCount % 100000) / 100000.0f;
            worldMatrix = Matrix.Translation(position);
            finalMatrix = worldMatrix * viewMatrix * projectionMatrix;

            //Important when not using the Effect direct3d framework
            finalMatrix = Matrix.Transpose(finalMatrix);

            DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, MapMode.WriteDiscard, SlimDX.Direct3D11.MapFlags.None);
            for (int i = 0; i < 16; i++)
                input.Data.Write((float)finalMatrix.ToArray()[i]);
            input.Data.Write(costime);
            input.Data.Write(lineartime);
            device.ImmediateContext.UnmapSubresource(inputBuffer, 0);

            context.InputAssembler.SetVertexBuffers(0, renderable.getVertices());

            ShaderResourceView resourceView = null;
            if (renderable.getTexture() != null)
            {
                resourceView = new ShaderResourceView(device, renderable.getTexture());
                context.PixelShader.SetShaderResource(resourceView, 0);
                
            }
           

            // Think of the shared textureD3D10 as an overlay.
            // The overlay needs to show the text but let the underlying triangle (or whatever)
            // show thru, which is accomplished by blending.

            if (renderable.useBlending())
            {

                BlendStateDescription bsd = new BlendStateDescription();
                bsd.RenderTargets[0].BlendEnable = true;
                bsd.RenderTargets[0].SourceBlend = BlendOption.SourceColor;
                bsd.RenderTargets[0].DestinationBlend = BlendOption.DestinationColor;
                bsd.RenderTargets[0].BlendOperation = BlendOperation.Add;
                bsd.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
                bsd.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
                bsd.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
                bsd.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                BlendState oBlendState_Transparent = BlendState.FromDescription(device, bsd);

                context.OutputMerger.BlendState = oBlendState_Transparent;
            }
            else
            {
                context.OutputMerger.BlendState = null;
            }
            context.Draw(renderable.getTriangleCount() * 3, 0);

            if (resourceView != null)
                resourceView.Dispose();
                   
        }

        public void  addRenderable(RenderableInterface renderable)
        {
            renderable.initBuffers(device);
           
            this.renderableList.Add(renderable);
        }

        public void addPovManager(PovManager manager)
        {
            List<RenderableInterface> rl = manager.getRenderableList();
            foreach (RenderableInterface r in rl)
            {
                r.initBuffers(device);
            }
            this.povManagerList.Add(manager);
        }


        public void render()
        {
            
            context.OutputMerger.DepthStencilState = this.depthStencilState;
            context.OutputMerger.BlendState = null;
            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);

            // configure the Input Assembler portion of the pipeline with the vertex data
            //context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // set the shaders
            //context.VertexShader.Set(vertexShader);
            //context.PixelShader.Set(pixelShader);

            context.VertexShader.SetConstantBuffer(inputBuffer, 0);
        }


        public void dispose()
        {
            //layout.Dispose();
            //inputSignature.Dispose();
            //vertexShader.Dispose();
            //pixelShader.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }


        private void setupTextRendering()
        {




            // A DirectX 10.1 device is required because DirectWrite/Direct2D are unable
            // to access DirectX11.  BgraSupport is required for DXGI interaction between
            // DirectX10/Direct2D/DirectWrite.
            SlimDX.Direct3D10_1.Device1 device10_1 = new SlimDX.Direct3D10_1.Device1(
                adapter1,
                SlimDX.Direct3D10.DriverType.Hardware,
                SlimDX.Direct3D10.DeviceCreationFlags.BgraSupport | SlimDX.Direct3D10.DeviceCreationFlags.None,
                SlimDX.Direct3D10_1.FeatureLevel.Level_10_0
            );

            

            // Create the DirectX11 texture2D.  This texture will be shared with the DirectX10
            // device.  The DirectX10 device will be used to render text onto this texture.  DirectX11
            // will then draw this texture (blended) onto the screen.
            // The KeyedMutex flag is required in order to share this resource.
            textureD3D11 = new Texture2D(device, new Texture2DDescription
            {

                Width = form.Width,
                Height = form.Height,
                /*
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.KeyedMutex
                 */
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.KeyedMutex
            });

            // A DirectX10 Texture2D sharing the DirectX11 Texture2D
            //SlimDX.DXGI.Resource sharedResource = new SlimDX.DXGI.Resource(textureD3D11);
            //SlimDX.Direct3D10.Texture2D textureD3D10 = device10_1.OpenSharedResource(sharedResource);

            // A DirectX10 Texture2D sharing the DirectX11 Texture2D
           
            SlimDX.DXGI.Resource sharedResource = new SlimDX.DXGI.Resource(textureD3D11);
            //SlimDX.DXGI.Resource sharedResource = SlimDX.DXGI.Resource.FromPointer(textureD3D11.ComPointer);
            //SlimDX.Direct3D10.Texture2D textureD3D10 = SlimDX.Direct3D10.Texture2D.FromPointer(sharedResource.SharedHandle);
            //device10_1.OpenSharedResource
            IntPtr toto = sharedResource.SharedHandle;

            SlimDX.Direct3D10.Texture2D textureD3D10 = device10_1.OpenSharedResource<SlimDX.Direct3D10.Texture2D>(toto);
            

            //SlimDX.Direct3D10.Texture2D textureD3D10 = device10_1.OpenSharedResource(sharedResource.SharedHandle);

            // The KeyedMutex is used just prior to writing to textureD3D11 or textureD3D10.
            // This is how DirectX knows which DirectX (10 or 11) is supposed to be writing
            // to the shared texture.  The keyedMutex is just defined here, they will be used
            // a bit later.
            mutexD3D10 = new KeyedMutex(textureD3D10);
            mutexD3D11 = new KeyedMutex(textureD3D11);

            // Direct2D Factory
            SlimDX.Direct2D.Factory d2Factory = new SlimDX.Direct2D.Factory(
                SlimDX.Direct2D.FactoryType.SingleThreaded,
                SlimDX.Direct2D.DebugLevel.Information
            );

            // Direct Write factory
            SlimDX.DirectWrite.Factory dwFactory = new SlimDX.DirectWrite.Factory(
                SlimDX.DirectWrite.FactoryType.Isolated
            );

            // The textFormat we will use to draw text with
            textFormat = new SlimDX.DirectWrite.TextFormat(
                dwFactory,
                "Arial",
                SlimDX.DirectWrite.FontWeight.Normal,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                18,
                "en-US"
            );
            textFormat.TextAlignment = SlimDX.DirectWrite.TextAlignment.Trailing;
            textFormat.ParagraphAlignment = SlimDX.DirectWrite.ParagraphAlignment.Near;


            // The textFormat we will use to draw text with
            textFormatDebug = new SlimDX.DirectWrite.TextFormat(
                dwFactory,
                "Arial",
                SlimDX.DirectWrite.FontWeight.Bold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                14,
                "en-US"
            );
            textFormatDebug.TextAlignment = SlimDX.DirectWrite.TextAlignment.Leading;
            textFormatDebug.ParagraphAlignment = SlimDX.DirectWrite.ParagraphAlignment.Far;

            // Query for a IDXGISurface.
            // DirectWrite and DirectX10 can interoperate thru DXGI.
            Surface surface = textureD3D10.AsSurface();
            SlimDX.Direct2D.RenderTargetProperties rtp = new SlimDX.Direct2D.RenderTargetProperties();
            rtp.MinimumFeatureLevel = SlimDX.Direct2D.FeatureLevel.Direct3D10;
            rtp.Type = SlimDX.Direct2D.RenderTargetType.Hardware;
            rtp.Usage = SlimDX.Direct2D.RenderTargetUsage.None;
            rtp.PixelFormat = new SlimDX.Direct2D.PixelFormat(Format.Unknown, SlimDX.Direct2D.AlphaMode.Premultiplied);
            dwRenderTarget = SlimDX.Direct2D.RenderTarget.FromDXGI(d2Factory, surface, rtp);

            // Brush used to DrawText
            brushSolidWhite = new SlimDX.Direct2D.SolidColorBrush(
                dwRenderTarget,
                new Color4(1, 1, 1, 1)
            );

            // Think of the shared textureD3D10 as an overlay.
            // The overlay needs to show the text but let the underlying triangle (or whatever)
            // show thru, which is accomplished by blending.
            BlendStateDescription bsd = new BlendStateDescription();
            bsd.RenderTargets[0].BlendEnable = true;
            bsd.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            bsd.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            bsd.RenderTargets[0].BlendOperation = BlendOperation.Add;
            bsd.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            bsd.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
            bsd.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            bsd.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            BlendState_Transparent = BlendState.FromDescription(device, bsd);

            verticesText = new DataStream((12+8) * 4, true, true);
            verticesText.Write(new Vector3(-1f, 1f, 0));
            verticesText.Write(new Vector2(0f, 0f));
            verticesText.Write(new Vector3(1f, 1f, 0));
            verticesText.Write(new Vector2(1f, 0f));
            verticesText.Write(new Vector3(-1, -1, 0));
            verticesText.Write(new Vector2(0f, 1f));
            verticesText.Write(new Vector3(1f, -1f, 0f));
            verticesText.Write(new Vector2(1f, 1f));

            verticesText.Position = 0;

            // create the text vertex layout and buffer
            //InputLayout layoutText = new InputLayout(device11, effect.GetTechniqueByName("Text").GetPassByIndex(0).Description.Signature, VertexPositionTexture.inputElements);
            vertexBufferText = new SlimDX.Direct3D11.Buffer(device, verticesText, 20 * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            //verticesText.Close();

            textbufferBinding = new VertexBufferBinding(vertexBufferText, 20, 0);
            
            addShader("text.fx");

            RasterizerStateDescription rasterizerStateDescription = new RasterizerStateDescription { CullMode = CullMode.Back, FillMode = FillMode.Solid };
            device.ImmediateContext.Rasterizer.State = RasterizerState.FromDescription(device, rasterizerStateDescription);

            Renderer.DebugLog("-- Starting --");
            Renderer.DebugLog("Hello World !");
        }

        private long elapsed = 0;
        private int countElapsed = 0;
        private float fps = 0f;
        public static StringBuilder[] bText;
        public static void DebugLog(String text)
        {
            if (bText == null)
            {
                bText = new StringBuilder[4];
                for (int i = 0; i < 4; i++)
                    bText[i] = new StringBuilder();
            }
            for (int i = 0; i < bText.Length; i++)
            {
                if (bText[i].Length == 0)
                {
                    bText[i].Append("[" + DateTime.Now.ToString() + "] " + text + "\n");
                    return;
                }
            }
            
            for (int i = 0; i < bText.Length - 1; i++)
            {
                bText[i].Clear();
                bText[i].Append(bText[i+1]);
            }
            
            bText[3].Clear();
            bText[3].Append("[" + DateTime.Now.ToString() + "] " + text + "\n");
        }

        private static String getDebugLog()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < bText.Length; i++)
            {
                b.Append(bText[i]);
            }
            return b.ToString();
        }

        private void drawPovManager(PovManager manager)
        {
            List<PovManager.RenderableStruct> list = manager.getRenderableList(camera.getPosition());
            foreach (PovManager.RenderableStruct rl in list)
            {
                foreach (RenderableInterface r in rl.renderableList)
                {
                    drawRenderable(r, rl.position);
                }
            }
        }



        private void drawTextInTexture()
        {
            
            countElapsed++;
            if (countElapsed == 50)
            {
                
                elapsed = (DateTime.Now.Ticks - elapsed);
                TimeSpan elapsedSpan = new TimeSpan(elapsed);
                fps = (float)50f / ((float)elapsedSpan.TotalSeconds);
                fps = (int)fps;
                elapsed = DateTime.Now.Ticks;
                countElapsed = 0;
            }
            // Draw Text on the shared Texture2D
            // Need to Acquire the shared texture for use with DirectX10
            mutexD3D10.Acquire(0, 100);
            dwRenderTarget.BeginDraw();
            dwRenderTarget.Clear(new Color4(0, 0, 0, 0));
            string text = adapter1.Description1.Description;
            //string text = "Hello World !";
            
            //dwRenderTarget.DrawText(text, textFormat, new System.Drawing.Rectangle(0, 0, form.Width, form.Height), brushSolidWhite);
            dwRenderTarget.DrawText(text + "\n FPS=" + fps.ToString() + "\n Camera="+this.camera.getPosition().ToString(), textFormat, new System.Drawing.Rectangle(0, 0, form.Width, form.Height), brushSolidWhite);


            dwRenderTarget.DrawText(getDebugLog(), textFormatDebug, new System.Drawing.Rectangle(0, 0, form.Width, form.Height), brushSolidWhite);


            //dwRenderTarget.DrawRectangle(brushSolidWhite, new System.Drawing.Rectangle(1, 1, form.Width, form.Height));
            
            dwRenderTarget.EndDraw();
            mutexD3D10.Release(0);

            // Draw the shared texture2D onto the screen
            // Need to Aquire the shared texture for use with DirectX11
            mutexD3D11.Acquire(0, 100);
            ShaderResourceView srv = new ShaderResourceView(device, textureD3D11);
            
            //effect.GetVariableByName("g_textOverlay").AsResource().SetResource(srv);

            Shader shader = shaderStore["text.fx"];
            context.InputAssembler.InputLayout = shader.layout;

            context.VertexShader.Set(shader.vertexShader);
            context.PixelShader.Set(shader.pixelShader);
            context.PixelShader.SetShaderResource(srv, 0);


            //context.InputAssembler.InputLayout = layoutText;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            //context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBufferText, (12+8)*4, 0));
            context.InputAssembler.SetVertexBuffers(0, this.textbufferBinding);
            
            context.OutputMerger.BlendState = this.BlendState_Transparent;


            

            /*
            currentTechnique = effect.GetTechniqueByName("Text");
            for (int pass = 0; pass < currentTechnique.Description.PassCount; ++pass)
            {
                EffectPass Pass = currentTechnique.GetPassByIndex(pass);
                System.Diagnostics.Debug.Assert(Pass.IsValid, "Invalid EffectPass");
                Pass.Apply(context);
                context.Draw(4, 0);
            }*/
            context.Draw(4, 0);

            srv.Dispose();
            mutexD3D11.Release(0);
        }
    }
}
