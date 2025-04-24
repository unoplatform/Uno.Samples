# Uno Platform MVUX Samples

This repository provides simple, to-the-point code samples for [MVUX](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Overview.html?tabs=viewmodel%2Cmodel).

## Samples

- [ListFeed](src/MVUX/Presentation/ListFeedSample/)
  - Docs: [How-To: Create a ListFeed](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Tutorials/HowTo-ListFeed.html)
- [Feed](src/MVUX/Presentation/FeedSample)
  - Docs: [How-To: Create a Feed](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Tutorials/HowTo-SimpleFeed.html)
- [Create & Update a State](src/MVUX/Presentation/UpdateStateSample)
  - Docs: [How-To: Create and Update a State](https://platform.uno/docs/articles/external/uno.extensions/doc/Reference/Reactive/state.html)
- [Selection](src/MVUX/Presentation/SelectionSample)
  - Docs: [Selection Item](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Advanced/Selection.html)
- [Pagination](src/MVUX/Presentation/PaginationSample)
  - Docs: [How-To: Paginate Data](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Advanced/Pagination.html)
- [FeedView](src/MVUX/Presentation/FeedViewSample)
  - Docs: [How-To: Use a FeedView](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Tutorials/HowTo-SimpleFeed.html#using-a-feedview)
- [FeedView Command](src/MVUX/Presentation/FeedViewCommandSample)
- [Update  Feed using IMessenger](src/MVUX/Presentation/IMessengerSample)
  - Docs: [Messaging](xref:Uno.Extensions.Mvux.Advanced.Messaging)

# MVUX Project Samples

- [By name](#Tab/ByName)

- WeatherApp

    The WeatherApp is an introduction to MVUX.  
    It utilizes an `IFeed<T>` to asynchronously retrieve a single record type from a service and display on the View.  
    In addition, it makes use of the `FeedView`'s `Refresh` command and demonstrates customizing its `ProgressTemplate`.  
    It's explained in [this](https://platform.uno/docs/articles/external/uno.extensions/doc/Overview/Mvux/Overview.html) tutorial.

- PeopleApp

    This app displays a read-only list of people using the `IListFeed<T>` where `T` is a `Person` record.

- EditablePeopleApp

    This app displays an editable list of people using the `IListState<T>` where `T` is a `Person` record.

- SelectionPeopleApp

    This app displays a read-only list of people and demonstrates the use of MVUX selection.

- PaginationPeopleApp

    This app displays a read-only list of people and demonstrates the use of MVUX pagination.

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

- [By category](#Tab/ByCategory)

- Feed
    - WeatherApp
- ListFeed
    - PeopleApp
    - SelectionPeopleApp
    - PaginationPeopleApp
- Feed with AsyncEnumerable 
    - StockMarketApp
- State
    - WeddingHallApp
    - SliderApp
    - SelectionPeopleApp
    - PaginationPeopleApp
- ListState
    - EditablePeopleApp
- Messaging
    - MessagingPeopleApp

---

If you encounter any issues with these samples above, please open an issue [here](https://github.com/unoplatform/Uno.Samples/issues/new).
