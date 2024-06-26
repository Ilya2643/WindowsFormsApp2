﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApp2
{
    internal class View
    {
        Bitmap textureImage;
        int VBOtexture;
        public int min = 0;

        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        Color TransferFunction(short value)
        {
            
            int max = Bin.Z;
            int newVal = Clamp((value - min) * 255 / (max - min), 0, 255);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        public void DrawQuads(int layerNumber)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);

            for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
            {
                for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                {
                    short value;
                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord);

                    value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord + 1);

                    value = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    value = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord);

                }
            }
            GL.End();
        }

        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(
                new Rectangle(0, 0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            ErrorCode er = GL.GetError();
            string str = er.ToString();

        }

        public void generateTextureImage(int layerNumber) // генерируем изображение из томограммы при помощи TF
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);
            for (int i = 0; i < Bin.X; i++)
                for (int j = 0; j < Bin.Y; j++)
                {
                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber]));
                }
        }

        public void DrawTexture() //включает 2d текстурирование
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);

            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }



    }
}
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;

//namespace WindowsFormsApp2
//{
//    internal class View
//    {
//        Bitmap textureImage;
//        int VBOtexture;


//        public void SetupView(int width, int height)
//        {
//            GL.ShadeModel(ShadingModel.Smooth);
//            GL.MatrixMode(MatrixMode.Projection);
//            GL.LoadIdentity();
//            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
//            GL.Viewport(0, 0, width, height);
//        }

//        public int Clamp(int value, int min, int max)
//        {
//            if (value < min)
//                return min;
//            if (value > max)
//                return max;
//            return value;
//        }

//        Color TransferFunction(short value, short value2)
//        {
//            int min = value;
//            int max = value + value2;
//            int newVal = Clamp((value2 - min) * 255 / (max - min), 0, 255);
//            return Color.FromArgb(255, newVal, newVal, newVal);
//        }
//        public void DrawQuads(int layerNumber, int layerNumber2)
//        {
//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
//            GL.Begin(BeginMode.Quads);

//            for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
//            {
//                for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
//                {
//                    short value;
//                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
//                    short value2;
//                    value2 = Bin.array[x_coord + y_coord * Bin.X + layerNumber2 * Bin.X * Bin.Y];
//                    GL.Color3(TransferFunction(value, value2));
//                    GL.Vertex2(x_coord, y_coord);

//                    value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
//                    value2 = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber2 * Bin.X * Bin.Y];
//                    GL.Color3(TransferFunction(value, value2));
//                    GL.Vertex2(x_coord, y_coord + 1);

//                    value = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
//                    value2 = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.X + layerNumber2 * Bin.X * Bin.Y];
//                    GL.Color3(TransferFunction(value, value2));
//                    GL.Vertex2(x_coord + 1, y_coord + 1);

//                    value = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
//                    value2 = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNumber2 * Bin.X * Bin.Y];
//                    GL.Color3(TransferFunction(value, value2));
//                    GL.Vertex2(x_coord + 1, y_coord);

//                }
//            }
//            GL.End();
//        }

//        public void Load2DTexture()
//        {
//            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
//            BitmapData data = textureImage.LockBits(
//                new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height),
//                ImageLockMode.ReadOnly,
//                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

//            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
//                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
//                PixelType.UnsignedByte, data.Scan0);

//            textureImage.UnlockBits(data);

//            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
//                (int)TextureMinFilter.Linear);
//            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
//                (int)TextureMagFilter.Linear);

//            ErrorCode er = GL.GetError();
//            string str = er.ToString();

//        }

//        public void generateTextureImage(int layerNumber, int layerNumber2)
//        {
//            textureImage = new Bitmap(Bin.X, Bin.Y);
//            for (int i = 0; i < Bin.X; i++)
//                for (int j = 0; j < Bin.Y; j++)
//                {
//                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
//                    int pixelNumber2 = i + j * Bin.X + layerNumber2 * Bin.X * Bin.Y;
//                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber], Bin.array[pixelNumber2]));
//                }
//        }

//        public void DrawTexture()
//        {
//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
//            GL.Enable(EnableCap.Texture2D);
//            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

//            GL.Begin(BeginMode.Quads);
//            GL.Color3(Color.White);
//            GL.TexCoord2(0f, 0f);
//            GL.Vertex2(0, 0);

//            GL.TexCoord2(0f, 1f);
//            GL.Vertex2(0, Bin.Y);

//            GL.TexCoord2(1f, 1f);
//            GL.Vertex2(Bin.X, Bin.Y);

//            GL.TexCoord2(1f, 0f);
//            GL.Vertex2(Bin.X, 0);
//            GL.End();

//            GL.Disable(EnableCap.Texture2D);
//        }



//    }
//}
