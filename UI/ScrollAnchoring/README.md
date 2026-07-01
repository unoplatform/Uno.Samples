# Scroll Anchoring

This sample demonstrates scroll anchoring on the Uno Platform `ScrollViewer`. When items are inserted or removed above the currently anchored item, the `ScrollViewer` keeps the anchor's on-screen position stable instead of letting the content jump.

It uses the `ScrollViewer` anchoring API: `RegisterAnchorCandidate`/`UnregisterAnchorCandidate` to register live descendants as candidates (on `Loaded`/`Unloaded`), the `CurrentAnchor` property to read the element the viewport is currently anchored to, and `VerticalAnchorRatio` to choose which point of the viewport (top, center, or bottom) is preserved.

The visible position-preservation effect works on the Skia Desktop head.
