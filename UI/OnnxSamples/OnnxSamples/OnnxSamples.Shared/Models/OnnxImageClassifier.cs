using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxSamples.Models
{
    internal class OnnxImageClassifier
    {
        #region Variable(s)

        const int DimBatchSize = 1;
        const int DimNumberOfChannels = 3;
        const int ImageSizeX = 224;
        const int ImageSizeY = 224;
        const string ModelInputName = "input";
        const string ModelOutputName = "output";

        byte[] _model;
        byte[] _sampleImage;
        List<string> _labels;
        InferenceSession _session;
        Task _initTask;

        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        #endregion

        #region Constructor(s)

        public OnnxImageClassifier()
        {
            _ = InitAsync();
        }

        #endregion

        #region Method(s)

        public async Task<byte[]> GetSampleImageAsync(string filename)
        {
            await InitAsync(filename).ConfigureAwait(false);
            var assembly = GetType().Assembly;
            // Get sample image
            var imageResource = EmbeddedResources.First(item => item.EndsWith(filename));
            using var sampleImageStream = assembly.GetManifestResourceStream(imageResource);
            using var sampleImageMemoryStream = new MemoryStream();

            sampleImageStream.CopyTo(sampleImageMemoryStream);
            _sampleImage = sampleImageMemoryStream.ToArray();
            return _sampleImage;
        }

        public async Task<string> GetClassificationAsync(byte[] image, string filename)
        {
            await InitAsync(filename).ConfigureAwait(false);
            using var sourceBitmap = SKBitmap.Decode(image);
            var pixels = sourceBitmap.Bytes;

            // Preprocess image data according to model requirements
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#preprocessing

            // Scale and crop the original image if necessary to match the way the model has been trained
            // In this case, the model expects 224x224 images
            if (sourceBitmap.Width != ImageSizeX || sourceBitmap.Height != ImageSizeY)
            {
                float ratio = (float)Math.Min(ImageSizeX, ImageSizeY) / Math.Min(sourceBitmap.Width, sourceBitmap.Height);

                using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(
                    (int)(ratio * sourceBitmap.Width),
                    (int)(ratio * sourceBitmap.Height)),
                    SKFilterQuality.Medium);

                var horizontalCrop = scaledBitmap.Width - ImageSizeX;
                var verticalCrop = scaledBitmap.Height - ImageSizeY;
                var leftOffset = horizontalCrop == 0 ? 0 : horizontalCrop / 2;
                var topOffset = verticalCrop == 0 ? 0 : verticalCrop / 2;

                var cropRect = SKRectI.Create(
                    new SKPointI(leftOffset, topOffset),
                    new SKSizeI(ImageSizeX, ImageSizeY));

                using SKImage currentImage = SKImage.FromBitmap(scaledBitmap);
                using SKImage croppedImage = currentImage.Subset(cropRect);
                using SKBitmap croppedBitmap = SKBitmap.FromImage(croppedImage);

                pixels = croppedBitmap.Bytes;
            }

            // Preprocess the image data into the format expected by the model.

            // In this case, the model expects RGB values in the range of [0, 1] normalized using
            // mean = [0.485, 0.456, 0.406] and std = [0.229, 0.224, 0.225]

            // The loop below iterates over the image pixels one row at a time,
            // applies the requisite normalization to each RGB value, then stores each in the channelData array.

            // The channelData array stores the normalized RGB values sequentially one channel at a time (instead of the original RGB, RGB, ... sequence) i.e.
            // first all the R values,
            // then all the G values,
            // then all the B values

            // The resulting channelData array is used to create the requisite Tensor object as input to the InferenceSession.Run method
            var bytesPerPixel = sourceBitmap.BytesPerPixel;
            var rowLength = ImageSizeX * bytesPerPixel;
            var channelLength = ImageSizeX * ImageSizeY;
            var channelData = new float[channelLength * 3];
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

                    channelData[rChannelIndex] = (pixelR / 255f - 0.485f) / 0.229f;
                    channelData[gChannelIndex] = (pixelG / 255f - 0.456f) / 0.224f;
                    channelData[bChannelIndex] = (pixelB / 255f - 0.406f) / 0.225f;

                    channelDataIndex++;
                }
            }

            // Create Tensor model input
            // The model expects input to be in the shape of (N x 3 x H x W) i.e.
            // mini-batches (where N is the batch size) of 3-channel RGB images with H and W of 224
            // https://onnxruntime.ai/docs/api/csharp-api#systemnumericstensor
            var input = new DenseTensor<float>(channelData, new[] { DimBatchSize, DimNumberOfChannels, ImageSizeX, ImageSizeY });

            // Run inferencing
            // https://onnxruntime.ai/docs/api/csharp-api#methods-1
            // https://onnxruntime.ai/docs/api/csharp-api#namedonnxvalue
            using var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(ModelInputName, input) });

            // Resolve model output
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#output
            // https://onnxruntime.ai/docs/api/csharp-api#disposablenamedonnxvalue
            var output = results.FirstOrDefault(i => i.Name == ModelOutputName);

            if (output == null)
                return "Unknown";

            // Postprocess output (get highest score and corresponding label)
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#postprocessing
            var scores = output.AsTensor<float>().ToList();
            var highestScore = scores.Max();
            var highestScoreIndex = scores.IndexOf(highestScore);
            var label = _labels.ElementAt(highestScoreIndex);

            return label;
        }

        Task InitAsync(string filename = "dogg.jpg")
        {
            if (_initTask == null || _initTask.IsFaulted)
                _initTask = InitTask(filename);

            return _initTask;
        }

        async Task InitTask(string filename)
        {
            var assembly = GetType().Assembly;

            // Get labels
            var textResource = EmbeddedResources.First(item => item.EndsWith(".imagenet_classes.txt"));
            using var labelsStream = assembly.GetManifestResourceStream(textResource);
            using var reader = new StreamReader(labelsStream);

            string text = await reader.ReadToEndAsync();
            _labels = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Get model
            var modelResource = EmbeddedResources.First(item => item.EndsWith(".mobilenetv2-7.onnx"));
            using var modelStream = assembly.GetManifestResourceStream(modelResource);
            using var modelMemoryStream = new MemoryStream();

            modelStream.CopyTo(modelMemoryStream);
            _model = modelMemoryStream.ToArray();

            // Create InferenceSession (runtime representation of the model with optional SessionOptions)
            // This can be reused for multiple inferences to avoid unnecessary allocation/dispose overhead
            // https://onnxruntime.ai/docs/api/csharp-api#inferencesession
            // https://onnxruntime.ai/docs/api/csharp-api#sessionoptions
            _session = new InferenceSession(_model);

            // Get sample image
            var imageResource = EmbeddedResources.First(item => item.EndsWith(filename));
            using var sampleImageStream = assembly.GetManifestResourceStream(imageResource);
            using var sampleImageMemoryStream = new MemoryStream();

            sampleImageStream.CopyTo(sampleImageMemoryStream);
            _sampleImage = sampleImageMemoryStream.ToArray();
        }



        #endregion

    }
}
