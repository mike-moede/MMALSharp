﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Processors.Effects;

namespace MMALSharp.Processors.Motion
{
    public abstract class FrameDiffAnalyser : FrameAnalyser
    {
        /// <summary>
        /// Working storage for the Test Frame. This is the original static image we are comparing to.
        /// </summary>
        protected List<byte> TestFrame { get; set; }
        
        /// <summary>
        /// Indicates whether we have a full test frame.
        /// </summary>
        protected bool FullTestFrame { get; set; }

        protected MotionConfig MotionConfig { get; set; }

        protected FrameDiffAnalyser(MotionConfig config, IImageContext imageContext)
            : base(imageContext)
        {
            this.TestFrame = new List<byte>();
            this.MotionConfig = config;
        }

        public void CheckForChanges(Action onDetect)
        {
            var edgeDetection = new EdgeDetection(EDStrength.Medium);
            this.ImageContext.Data = this.TestFrame.ToArray();
            edgeDetection.ApplyConvolution(EdgeDetection.MediumStrengthKernel, 3, 3, this.ImageContext);
            var diff = this.Analyse();

            MMALLog.Logger.Info($"Diff size: {diff}");

            if (diff >= this.MotionConfig.Threshold)
            {
                MMALLog.Logger.Info("Motion detected!");
                onDetect();
            }
        }

        private Bitmap LoadBitmap(MemoryStream stream)
        {
            if (this.ImageContext.Raw)
            {
                return new Bitmap(this.ImageContext.Resolution.Width, this.ImageContext.Resolution.Height, this.ImageContext.PixelFormat);
            }
            
            return new Bitmap(stream);
        }

        private void InitBitmapData(BitmapData bmpData)
        {
            var pNative = bmpData.Scan0;
            Marshal.Copy(this.ImageContext.Data, 0, pNative, this.ImageContext.Data.Length);
        }

        private int Analyse()
        {
            using (var testMemStream = new MemoryStream(this.TestFrame.ToArray()))
            using (var currentMemStream = new MemoryStream(this.WorkingData.ToArray()))
            {
                var testBmp = this.LoadBitmap(testMemStream);
                var currentBmp = this.LoadBitmap(currentMemStream);
                var testBmpData = testBmp.LockBits(new Rectangle(0, 0, testBmp.Width, testBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, testBmp.PixelFormat);
                var currentBmpData = currentBmp.LockBits(new Rectangle(0, 0, currentBmp.Width, currentBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, currentBmp.PixelFormat);

                if (this.ImageContext.Raw)
                {
                    this.InitBitmapData(testBmpData);
                    this.InitBitmapData(currentBmpData);
                }

                var quadA = new Rectangle(0, 0, testBmpData.Width / 2, testBmpData.Height / 2);
                var quadB = new Rectangle(testBmpData.Width / 2, 0, testBmpData.Width / 2, testBmpData.Height / 2);
                var quadC = new Rectangle(0, testBmpData.Height / 2, testBmpData.Width / 2, testBmpData.Height / 2);
                var quadD = new Rectangle(testBmpData.Width / 2, testBmpData.Height / 2, testBmpData.Width / 2, testBmpData.Height / 2);
                
                int diff = 0;

                var bpp = Image.GetPixelFormatSize(testBmp.PixelFormat) / 8;

                var t1 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadA, testBmpData, currentBmpData, bpp);
                });
                var t2 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadB, testBmpData, currentBmpData, bpp);
                });
                var t3 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadC, testBmpData, currentBmpData, bpp);
                });
                var t4 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadD, testBmpData, currentBmpData, bpp);
                });

                Task.WaitAll(t1, t2, t3, t4);

                testBmp.UnlockBits(testBmpData);
                currentBmp.UnlockBits(currentBmpData);
                testBmp.Dispose();
                currentBmp.Dispose();

                return diff;
            }
        }

        private int CheckDiff(Rectangle quad, BitmapData bmpData, BitmapData bmpData2, int pixelDepth)
        {
            unsafe
            {
                var stride1 = bmpData.Stride;
                var stride2 = bmpData2.Stride;
               
                byte* ptr1 = (byte*)bmpData.Scan0;
                byte* ptr2 = (byte*)bmpData2.Scan0;
              
                int diff = 0;
                int lowestX = 0, highestX = 0, lowestY = 0, highestY = 0;

                for (int column = quad.X; column < quad.X + quad.Width; column++)
                {
                    for (int row = quad.Y; row < quad.Y + quad.Height; row++)
                    {
                        var rgb1 = ptr1[(column * 3) + (row * stride1)] +
                        ptr1[(column * pixelDepth) + (row * stride1) + 1] +
                        ptr1[(column * pixelDepth) + (row * stride1) + 2];

                        var rgb2 = ptr2[(column * 3) + (row * stride2)] +
                        ptr2[(column * pixelDepth) + (row * stride2) + 1] +
                        ptr2[(column * pixelDepth) + (row * stride2) + 2];

                        if (rgb2 > rgb1)
                        {
                            diff++;

                            if (row < lowestY || lowestY == 0)
                            {
                                lowestY = row;
                            }

                            if (row > highestY)
                            {
                                highestY = row;
                            }

                            if (column < lowestX || lowestX == 0)
                            {
                                lowestX = column;
                            }

                            if (column > highestX)
                            {
                                highestX = column;
                            }
                        }
                    }
                }

                return diff;
            }
        }
    }
}