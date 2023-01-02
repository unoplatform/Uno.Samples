using BERTTokenizers;

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnnxSamples.Models
{
    internal class PytorchBertQnA
    {
        #region Variable(s)

        const int DimBatchSize = 1;
       
        List<(string Token, int VocabularyIndex, long SegmentIndex)> tokens;
        BertUncasedLargeTokenizer tokenizer;
        InferenceSession _session;
        Task _initTask;
        byte[] _model;
        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        #endregion

        #region Method(s)

        async Task InitTask()
        {
            var assembly = GetType().Assembly;
            // Get Model
            var modelResource = EmbeddedResources.First(item => item.EndsWith("bert_QnA.onnx"));
            using var modelStream = assembly.GetManifestResourceStream(modelResource);
            using var modelMemoryStream = new MemoryStream();

            modelStream.CopyTo(modelMemoryStream);
            _model = modelMemoryStream.ToArray();

            // Create InferenceSession (runtime representation of the model with optional SessionOptions)
            // This can be reused for multiple inferences to avoid unnecessary allocation/dispose overhead
            // https://onnxruntime.ai/docs/api/csharp-api#inferencesession
            // https://onnxruntime.ai/docs/api/csharp-api#sessionoptions
            _session = new InferenceSession(_model);

            await Task.CompletedTask;
        }

        Task InitAsync()
        {
            if (_initTask == null || _initTask.IsFaulted)
                _initTask = InitTask();

            return _initTask;
        }

        BertInput PreProcessInput(string question, string context)
        {
            var sentence = "{\"question\": \" " + question + "\", \"context\": \"" + context + "\"}";
           
            tokenizer = new BertUncasedLargeTokenizer();
            tokens = tokenizer.Tokenize(sentence);
            var encoded = tokenizer.Encode(tokens.Count(), sentence);


            return new BertInput
            {
                InputIds = encoded.Select(tuple => tuple.InputIds).ToArray(),
                AttentionMask = encoded.Select(tuple => tuple.AttentionMask).ToArray(),
                TypeIds = encoded.Select(tuple => tuple.TokenTypeIds).ToArray()
            };
        }

        Tensor<long> GetInputTensor(long[] input, int dimensions)
        {
            var inputTensor = new DenseTensor<long>(input, new[] { DimBatchSize, dimensions });

            return inputTensor;
        }

        public async Task<(bool, string)> GetAnswerFromBert(string question, string context)
        {
            await InitAsync();

            var bertInput = PreProcessInput(question, context);

            var inputIdsTensor = GetInputTensor(bertInput.InputIds, bertInput.InputIds.Length);
            var attentionMaskTensor = GetInputTensor(bertInput.AttentionMask, bertInput.AttentionMask.Length);
            var typeIdsTensor = GetInputTensor(bertInput.TypeIds, bertInput.TypeIds.Length);

            var inputData = new List<NamedOnnxValue>
            { 
                NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
                NamedOnnxValue.CreateFromTensor("input_mask", attentionMaskTensor),
                NamedOnnxValue.CreateFromTensor("segment_ids", typeIdsTensor),
            };

            var output = _session.Run(inputData);

            // Extract the Human understandable result.
            var startLogits = (output.ToList().First().Value as IEnumerable<float>).ToList();
            var endLogits = (output.ToList().Last().Value as IEnumerable<float>).ToList();

            var startIndex = startLogits.ToList().IndexOf(startLogits.Max());
            var endIndex = endLogits.ToList().IndexOf(endLogits.Max());

            var predictedTokens = tokens
              .Skip(startIndex)
              .Take(endIndex + 1 - startIndex)
              .Select(o => tokenizer.IdToToken((int)o.VocabularyIndex))
              .ToList();

            var result = string.Join(" ", predictedTokens);

            return (true, result);

        }
        #endregion

    }
}
