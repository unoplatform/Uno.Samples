# PhotoViewer

This is a PhotoViewer Sample app, created using the ZoomContentControl. Created using the user-friendly "blank" template from [Uno Template Wizard](https://new.platform.uno), this app aims to demonstrate the ZoomContentControl and its capabilities.

## Getting Started

1. The app seamlessly runs on any target platform. However, in Wasm, be mindful that PointerWheel events might conflict with the browser.
2. No special configurations are required to build and run the project.

## Running the App

1. Upon launch, the application displays an image along with some handy debugging data.
2. Initially, the ZoomContentControl is inactive. Activate it by toggling the switch.
3. Once activated, users can start zooming and panning:

- Use MouseWheel to scroll vertically.
- Hold Shift and scroll MouseWheel for horizontal scrolling.
- Hold Control and scroll MouseWheel to Zoom in and out.

## Other Features

The ZoomContentControl offers additional properties and methods to fine-tune its behavior and manipulate its content effortlessly, without relying solely on pointer events:

- IsActive - Enable and disable features.
- ResetZoom - Set zoom value and center point back to default.
- ResetOffset - Set horizontal and vertical offset back to default values.
- ResetWhenNotActive - Automatically reset zoom and offset when the control is inactive.