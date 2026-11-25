# Globe coastline mask

`landmask.bin` is an equirectangular 512×256 land mask, 1 bit per pixel packed MSB-first per row (1 = land), rasterized on 2026-10-08 from
Natural Earth 1:110m "Land" polygons (`ne_110m_land.geojson`, nvkelso/natural-earth-vector master). Natural Earth
places its raster and vector data in the public domain (https://www.naturalearthdata.com/about/terms-of-use/).
The brief's source bundle ships its own 512×256 bitmask; this one is regenerated from the same public-domain data
because that bundle was not available to the build.
