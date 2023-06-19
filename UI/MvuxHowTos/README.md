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

    This app displays an editable list of people using the `IListState<T>` where `T` is a `Person` record.

- AdvancedPeopleApp

    This app displays a read-only list of people using the `IListState<T>` where `T` is a `Person` record, enabling selection and pagination.

- MessagingPeopleApp

    This app is similar to EditablePeopleApp, except it uses CommunityToolkit.Mvvm.Messaging and Uno.Extensions.Reactive.Messaging to pass messages around.

- SliderApp

    This app utilizes an `IState<T>` where `T` is a primitive (`double`).    
    It demonstrates the use of connecting a slider to a State as well as the use of Commands to update it.

- StockMarket

    In this app we use an `IAsyncEnumerable` to push information and update an `IFeed<T>` to display stock market values.

- WeddingHallApp

    This app gets or sets the crowdedness in a wedding-hall.  
    For this purpose it uses an `IState<T>` where `T` is a record type holding the number of attendants present in the hall.
    It's explained in [this](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Mvux/Overview.html) tutorial.

By category:

- Feed
    - WeatherApp
- ListFeed
    - PeopleApp
    - AdvancedPeopleApp
- Feed with AsyncEnumerable 
    - StockMarketApp
- State
    - WeddingHallApp
    - SliderApp
- ListState
    - EditablePeopleApp
- Messaging
    - MessagingPeopleApp