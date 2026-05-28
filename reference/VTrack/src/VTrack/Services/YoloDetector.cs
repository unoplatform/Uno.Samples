using System.IO;
using System.Net.Http;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace VTrack.Services;

public sealed record Detection(string Label, float Confidence, Rect Box);

public sealed class YoloDetector : IDisposable
{
    private const string WeightsUrl = "https://github.com/AlexeyAB/darknet/releases/download/yolov4/yolov4-tiny.weights";
    private const string CfgUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov4-tiny.cfg";
    private const string NamesUrl = "https://raw.githubusercontent.com/pjreddie/darknet/master/data/coco.names";

    private const float ConfidenceThreshold = 0.35f;
    private const float NmsThreshold = 0.45f;
    private const int InputSize = 416;

    private Net? _net;
    private string[] _classNames = Array.Empty<string>();
    private string[] _outputLayerNames = Array.Empty<string>();

    public bool IsReady => _net != null;
    public string ModelDirectory { get; }
    public string BackendName { get; private set; } = "uninitialized";
    public double LastInferenceMs { get; private set; }
    public IReadOnlyList<string> ClassNames => _classNames;

    public YoloDetector()
    {
        ModelDirectory = Path.Combine(Path.GetTempPath(), "vtrack-models");
        Directory.CreateDirectory(ModelDirectory);
    }

    public async Task LoadAsync(IProgress<string>? status = null, CancellationToken ct = default)
    {
        if (_net != null) return;

        var weightsPath = Path.Combine(ModelDirectory, "yolov4-tiny.weights");
        var cfgPath = Path.Combine(ModelDirectory, "yolov4-tiny.cfg");
        var namesPath = Path.Combine(ModelDirectory, "coco.names");

        status?.Report("Checking model files...");
        await EnsureDownloaded(WeightsUrl, weightsPath, status, ct);
        await EnsureDownloaded(CfgUrl, cfgPath, status, ct);
        await EnsureDownloaded(NamesUrl, namesPath, status, ct);

        status?.Report("Loading YOLO network...");
        var (net, backend) = await Task.Run(() =>
        {
            var n = CvDnn.ReadNetFromDarknet(cfgPath, weightsPath)
                ?? throw new InvalidOperationException($"Failed to load YOLO network from {cfgPath} / {weightsPath}");
            n.SetPreferableBackend(global::OpenCvSharp.Dnn.Backend.OPENCV);

            // Try OpenCL (GPU) first; fall back to CPU if it fails on this machine.
            string used;
            try
            {
                n.SetPreferableTarget(Target.OPENCL);
                using var probe = new Mat(InputSize, InputSize, MatType.CV_8UC3, Scalar.All(0));
                using var blob = CvDnn.BlobFromImage(probe, 1.0 / 255.0,
                    new OpenCvSharp.Size(InputSize, InputSize), default, true, false);
                n.SetInput(blob);
                using var probeOut = n.Forward();
                used = "OpenCL (GPU)";
            }
            catch
            {
                n.SetPreferableTarget(Target.CPU);
                used = "CPU";
            }
            return (n, used);
        }, ct);

        _net = net;
        BackendName = backend;
        _classNames = await File.ReadAllLinesAsync(namesPath, ct);
        _outputLayerNames = _net.GetUnconnectedOutLayersNames()
            .Where(n => n != null)
            .Select(n => n!)
            .ToArray();
        status?.Report($"Ready. {_classNames.Length} classes via {backend}.");
    }

    private static async Task EnsureDownloaded(string url, string path, IProgress<string>? status, CancellationToken ct)
    {
        if (File.Exists(path) && new FileInfo(path).Length > 0) return;

        status?.Report($"Downloading {Path.GetFileName(path)}...");
        using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        var bytes = await http.GetByteArrayAsync(url, ct);
        await File.WriteAllBytesAsync(path, bytes, ct);
    }

    public IReadOnlyList<Detection> Detect(Mat frame)
    {
        if (_net == null) return Array.Empty<Detection>();

        var w = frame.Width;
        var h = frame.Height;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        using var blob = CvDnn.BlobFromImage(
            frame, 1.0 / 255.0,
            new OpenCvSharp.Size(InputSize, InputSize),
            default, swapRB: true, crop: false);
        _net.SetInput(blob);

        var outputs = new Mat[_outputLayerNames.Length];
        for (int i = 0; i < outputs.Length; i++) outputs[i] = new Mat();

        try
        {
            _net.Forward(outputs, _outputLayerNames);

            var boxes = new List<Rect>();
            var confidences = new List<float>();
            var classIds = new List<int>();

            foreach (var output in outputs)
            {
                // Each row: [center_x, center_y, box_w, box_h, objectness, class_scores...]
                var rows = output.Rows;
                var cols = output.Cols;
                var data = new float[rows * cols];
                System.Runtime.InteropServices.Marshal.Copy(output.Data, data, 0, rows * cols);

                for (int r = 0; r < rows; r++)
                {
                    var rowOffset = r * cols;
                    // class probabilities start at index 5
                    var bestClassId = 0;
                    var bestClassProb = 0f;
                    for (int c = 5; c < cols; c++)
                    {
                        var p = data[rowOffset + c];
                        if (p > bestClassProb)
                        {
                            bestClassProb = p;
                            bestClassId = c - 5;
                        }
                    }
                    if (bestClassProb < ConfidenceThreshold) continue;

                    var cx = data[rowOffset + 0] * w;
                    var cy = data[rowOffset + 1] * h;
                    var bw = data[rowOffset + 2] * w;
                    var bh = data[rowOffset + 3] * h;
                    var left = (int)(cx - bw / 2);
                    var top = (int)(cy - bh / 2);

                    boxes.Add(new Rect(left, top, (int)bw, (int)bh));
                    confidences.Add(bestClassProb);
                    classIds.Add(bestClassId);
                }
            }

            if (boxes.Count == 0) return Array.Empty<Detection>();

            CvDnn.NMSBoxes(boxes, confidences, ConfidenceThreshold, NmsThreshold, out var keepIndices);

            var result = new List<Detection>(keepIndices.Length);
            foreach (var idx in keepIndices)
            {
                var classId = classIds[idx];
                var label = classId >= 0 && classId < _classNames.Length ? _classNames[classId] : $"class-{classId}";
                result.Add(new Detection(label, confidences[idx], boxes[idx]));
            }
            sw.Stop();
            LastInferenceMs = sw.Elapsed.TotalMilliseconds;
            return result;
        }
        finally
        {
            foreach (var o in outputs) o.Dispose();
        }
    }

    public void Dispose()
    {
        _net?.Dispose();
        _net = null;
    }
}
