using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using SkiaSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnnxSamples.Models
{
    internal class TensorflowMNISTClassifier
    {
        #region Variable(s)

        const int ImageSizeX = 14;
        const int ImageSizeY = 14;
        const string ModelInputName = "dense_6_input";

        byte[] _model;
        byte[] _sampleImage;
        InferenceSession _session;
        Task _initTask;

        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        #endregion

        #region Method(s)

        async Task InitTask()
        {
            var assembly = GetType().Assembly;


            // Get model
            var modelResource = EmbeddedResources.First(item => item.EndsWith("mnist_model.onnx"));
            using var modelStream = assembly.GetManifestResourceStream(modelResource);
            using var modelMemoryStream = new MemoryStream();

            await modelStream.CopyToAsync(modelMemoryStream);
            _model = modelMemoryStream.ToArray();

            _session = new InferenceSession(_model);

            // Get sample image
            var imageResource = EmbeddedResources.First(item => item.EndsWith("handwrittenn.jpeg"));
            using var sampleImageStream = assembly.GetManifestResourceStream(imageResource);
            using var sampleImageMemoryStream = new MemoryStream();

            sampleImageStream.CopyTo(sampleImageMemoryStream);
            _sampleImage = sampleImageMemoryStream.ToArray();
        }

        Task InitAsync()
        {
            if (_initTask == null || _initTask.IsFaulted)
                _initTask = InitTask();

            return _initTask;
        }

        public async Task<string> GetPredictionAsync(byte[] image)
        {
            await InitAsync().ConfigureAwait(false);
            using var sourceBitmap = SKBitmap.Decode(image);
            var pixels = sourceBitmap.Bytes;
            var bytesPerPixel = sourceBitmap.BytesPerPixel;

            // Rescale.
            if (sourceBitmap.Width != ImageSizeX || sourceBitmap.Height != ImageSizeY)
            {
                float ratio = (float)Math.Min(ImageSizeX, ImageSizeY) / Math.Min(sourceBitmap.Width, sourceBitmap.Height);

                using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(ImageSizeX, ImageSizeY), SKFilterQuality.High);

                pixels = scaledBitmap.Bytes;
                bytesPerPixel = scaledBitmap.BytesPerPixel;
            }

            var rowLength = ImageSizeX * bytesPerPixel;
            var channelLength = ImageSizeX * ImageSizeY;
            var channelData = new float[channelLength * 4];
            var channelDataIndex = 0;

            for (int y = 0; y < ImageSizeY; y++)
            {
                var rowOffset = y * rowLength;

                for (int x = 0, columnOffset = 0; x < ImageSizeX; x++, columnOffset += bytesPerPixel)
                {
                    var pixelOffset = rowOffset + columnOffset;

                    var pixelR = pixels[pixelOffset];
                    var pixelG = pixels[pixelOffset + 1];
                    var pixelB = pixels[pixelOffset + 2];

                    var rChannelIndex = channelDataIndex;
                    var gChannelIndex = channelDataIndex + channelLength;
                    var bChannelIndex = channelDataIndex + (channelLength * 2);

                    channelData[rChannelIndex] = (255f - pixelR) / 255f;
                    channelData[gChannelIndex] = (255f - pixelG) / 255f;
                    channelData[bChannelIndex] = (255f - pixelB) / 255f;

                    channelDataIndex++;
                }
            }
            
            var input = new DenseTensor<float>(channelData, new int[] { 1, 28*28 });
            using var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(ModelInputName, input) });

            var output = results.FirstOrDefault(i => i.Name.StartsWith("dense"));

            if (output == null)
                return "Unknown";

            var scores = output.AsTensor<float>().ToList();
            var highestScore = scores.Max();
            var highestScoreIndex = scores.IndexOf(highestScore);

            return highestScoreIndex.ToString();
        }

        public byte[] GetSampleImage()
        {
            _ = InitAsync();
            return _sampleImage;
        }

        #endregion

    }
}
