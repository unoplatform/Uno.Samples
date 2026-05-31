namespace EnterpriseDashboard.Observatory.Models;

public record DataPoint(double X, double Y);

public record BoxData(string Label, double Min, double Q1, double Median, double Q3, double Max, double[] Outliers);

public record SlopeItem(string Label, double Before, double After);

public record HeatmapCell(int Row, int Col, double Value);

public record RingMetric(string Label, double Value, double Radius);

public record HBarItem(string Label, double Value);
