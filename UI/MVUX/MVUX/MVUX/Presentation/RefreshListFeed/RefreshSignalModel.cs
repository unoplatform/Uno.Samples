namespace MVUX
{
    public partial record RefreshSignalModel
    {
        private readonly Signal _refreshList = new();
        private int _updateCounter = 0;
        
        public RefreshSignalModel()
        {
        }
        
        public IListFeed<string> SimpleList => ListFeed.Async(GetStringsAsync, _refreshList);
        
        public async ValueTask<IImmutableList<string>> GetStringsAsync(CancellationToken ct)
        {
            await Task.Delay(2000, ct);
            
            // Simulate data that changes with each API call
            _updateCounter++;
            return ImmutableList.Create(
                $"Item {_updateCounter * 1}",
                $"Item {_updateCounter * 2}",
                $"Item {_updateCounter * 3}"
            );
        }
        
        public async ValueTask RefreshList()
        {
            _refreshList.Raise(); 
            await Task.Delay(1000);
        }
    }
}
