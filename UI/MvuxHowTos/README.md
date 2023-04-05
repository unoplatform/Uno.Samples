# MVUX templates

By name:

- WeatherApp

    The WeatherApp is an introduction to MVUX.
    It utilizes an `IFeed<T>` to asynchronously retrieve a single record type from a service and display on the View.
    In addition, it makes use of the `FeedView`'s `Refresh` command and demonstrates customizing its `ProgressTemplate`.
    It's explained in [this](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Mvux/Overview.html) tutorial.

- PeopleApp

    This app displays a read-only list of people using the `IListFeed<T>` where `T` is a `Person` record.

- EditablePeopleApp
- SliderApp
- StockMarket
- TheFancyWeddingHall

By category:

- Feed
    - WeatherApp
- ListFeed
- Feed with AsyncEnumerable 
- State
- ListState