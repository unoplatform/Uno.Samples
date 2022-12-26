using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ListViewSample.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Storage;
using System.IO;

namespace ListViewSample.ViewModels
{
    public class MainPageViewModel
    {
        public ObservableCollection<WorkplaceFriend> Friends { get; set; }

        public MainPageViewModel()
        {
            Friends = new ObservableCollection<WorkplaceFriend>();
            Initialize();
        }

        public async void Initialize()
        {
            await LoadFriendsAsync();
        }

        public async Task LoadFriendsAsync()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("friends.json", CreationCollisionOption.OpenIfExists);
            var friendsJson = await FileIO.ReadTextAsync(file);
            if (!string.IsNullOrEmpty(friendsJson))
            {
                try
                {
                    JsonSerializer.Deserialize<IList<WorkplaceFriend>>(friendsJson).ToList().ForEach(f => Friends.Add(f));
                }
                catch (System.Text.Json.JsonException)
                {
                    // If the JSON is invalid, just ignore it. 
                }
            }
        }

        public async Task SaveFriendsAsync()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("friends.json", CreationCollisionOption.OpenIfExists);

            var friendsJson = JsonSerializer.Serialize<IList<WorkplaceFriend>>(Friends);
            await FileIO.WriteTextAsync(file, friendsJson);
        }

        public void AddFriend(WorkplaceFriend friend)
        {
            Friends.Add(friend);
        }

        public void RemoveFriend(WorkplaceFriend friend)
        {
            Friends.Remove(friend);
        }

        public void UpdateFriend(WorkplaceFriend friend)
        {
            var friendToUpdate = Friends.FirstOrDefault(f => f.Id == friend.Id);
            friendToUpdate.Name = friend.Name;
            friendToUpdate.Occupation = friend.Occupation;
        }

        public WorkplaceFriend GetFriendById(Guid id)
        {
            return Friends.FirstOrDefault(f => f.Id == id);
        }
    }
}