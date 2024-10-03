# ScottPlot Samples

[ScottPlot](https://scottplot.net/) is a free and open-source plotting library for .NET that makes it easy to interactively display large datasets. Line plots, bar charts, pie graphs, scatter plots, and more can be created with just a few lines of code.

## Quickstart sample

[This sample](/QuickstartSample/) app was created by following the [ScottPlot Uno Platform Quickstart documentation](https://scottplot.net/quickstart/unoplatform).

![ScottPlot Quickstart Sample App](doc/assets/Quickstart-Sample.gif)

### Screenshots

| | | |
|:-------------------------:|:-------------------------:|:-------------------------:|
|<img width="500" alt="ScottPlot Quickstart Sample App - Windows" src="doc/assets/scottplot-winui-quickstart.png">  **Windows** |  <img width="500" alt="ScottPlot Quickstart Sample App - WebAssembly" src="doc/assets/unoplatform-quickstart-webassembly.png"> **WebAssembly** |<img width="500" alt="ScottPlot Quickstart Sample App - Android" src="doc/assets/unoplatform-quickstart-android.png"> **Android** |
|<img width="500" alt="ScottPlot Quickstart Sample App - iOS" src="doc/assets/unoplatform-quickstart-iOS.png"> **iOS** |  <img width="500" alt="ScottPlot Quickstart Sample App - Mac Catalyst" src="doc/assets/unoplatform-quickstart-mac-catalyst.png"> **Mac Catalyst** |<img width="500" alt="ScottPlot Quickstart Sample App - Desktop" src="doc/assets/unoplatform-quickstart-desktop.png"> **Desktop** |
|<img width="500" alt="ScottPlot Quickstart Sample App - Desktop WSL" src="doc/assets/unoplatform-quickstart-desktop-wsl.png"> **Desktop WSL** | | |

## Signal plot with 5 million points sample

[This sample](/SignalPlotFiveMillionPointsSample/) app was created the same way by following the [ScottPlot Uno Platform Quickstart documentation](https://scottplot.net/quickstart/unoplatform). Only the code-behind defers to display a signal plot with 5 million random points.

![ScottPlot Demo Sample App - Signal plot with 5 million points](doc/assets/FiveMillionPoints-Sample.gif)

## SQLite Data Persistence and Large Dataset Visualization Sample

[This sample](/DataPersistedSample/) demonstrates how to combine SQLite for database-driven data persistence with ScottPlot for visualizing large datasets. It showcases how to handle and visualize different plot types while persisting the data in a database for long-term storage:

- **SignalPlot and SignalConst**: These are the most memory-efficient for large datasets with evenly spaced X-values.
- **ScatterPlot (with downsampling)**: Ideal for non-uniform X-values, using downsampling to optimize performance.
- **Heatmap**: Suitable for visualizing 2D grid data.

The chosen plot types above prioritize performance and visualization quality, especially for platforms like WebAssembly with resource constraints.
Other plot types were excluded for this sample due to potential performance issues with large datasets based on ScottPlot documentation.

![ScottPlot Demo Sample App - SQLite Data Persistence with ScottPlot](doc/assets/DataPersisted-Sample.gif)

## What is the Uno Platform

[Uno Platform](https://platform.uno) is an open-source .NET platform for building single codebase native mobile, web, desktop, and embedded apps quickly.
For additional information about Uno Platform or if you have any feedback to share, please refer to the [README.md](../../README.md) file in this Samples repository.

## ScottPlot Support

If you have questions about the open-source ScottPlot plotting library:

* ScottPlot website: [https://scottplot.net/](https://scottplot.net/)
* ScottPlot on GitHub: [create an issue](https://github.com/ScottPlot/ScottPlot/issues) or [start a discussion](https://github.com/ScottPlot/ScottPlot/discussions)
* [ScottPlot Discord Server](https://discord.gg/Dru6fnY2UX) (All are welcome!)
