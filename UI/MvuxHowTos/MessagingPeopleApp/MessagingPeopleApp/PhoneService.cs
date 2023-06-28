using System.Collections.Immutable;
using CommunityToolkit.Mvvm.Messaging;

namespace MessagingPeopleApp
{
    public partial record Phone(int Id, int PersonId, string Number);

    public interface IPhoneService
    {
        public ValueTask<IImmutableList<Phone>> GetAllPhones(Person person, CancellationToken ct);

        public ValueTask DeletePhoneAsync(int phoneId, CancellationToken ct);
    }

    public class PhoneService : IPhoneService
    {
        protected IMessenger Messenger { get; }

        public PhoneService(IMessenger messenger)
        {
            Messenger = messenger;
        }

        public async ValueTask<IImmutableList<Phone>> GetAllPhones(Person person, CancellationToken ct)
        {
            await Task.Delay(500);

            return _phones
                .Where(phone => phone.PersonId == person.Id)
                .ToImmutableList();
        }

        public async ValueTask DeletePhoneAsync(int phoneId, CancellationToken ct)
        {
            await Task.Delay(500);

            var phone = _phones.SingleOrDefault(p => p.Id == phoneId);

            if (phone is not null)
            {
                _phones.Remove(phone);
                Messenger.Send(new EntityMessage<Phone>(EntityChange.Deleted, phone));
            }
        }

        private HashSet<Phone> _phones = new()
        {
            new (Id: 1, PersonId: 1, Number: "1111"),
            new (Id: 2, PersonId: 1, Number: "11111111"),
            new (Id: 3, PersonId: 2, Number: "2222"),
            new (Id: 4, PersonId: 2, Number: "22222222")
        };
    }
}
