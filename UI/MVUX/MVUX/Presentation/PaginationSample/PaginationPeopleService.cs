namespace MVUX.Presentation.PaginationSample;

public partial record Person(int Id, string FirstName, string LastName);

public interface IPaginationPeopleService
{
	ValueTask<IImmutableList<Person>> GetPeopleAsync(uint pageSize, uint firstItemIndex, CancellationToken ct);

	ValueTask<uint> GetPageCount(uint pageSize, CancellationToken ct);

	ValueTask<(IImmutableList<Person> CurrentPage, int? NextPersonIdCursor)> GetPeopleAsync(int? personIdCursor, uint pageSize, CancellationToken ct);
}

public class PaginationPeopleService : IPaginationPeopleService
{
	public async ValueTask<IImmutableList<Person>> GetPeopleAsync(uint pageSize, uint firstItemIndex, CancellationToken ct)
	{
		// convert to int for use with LINQ
		var (size, count) = ((int)pageSize, (int)firstItemIndex);

		// fake delay to simulate loading data
		await Task.Delay(TimeSpan.FromSeconds(1), ct);

		// this is where we would asynchronously load actual data from a remote data store
		var people = GetPeople();

		return people
			.Skip(count)
			.Take(size)
			.ToImmutableList();
	}

	// Determines how many pages we'll need to display all the data.
	public async ValueTask<uint> GetPageCount(uint pageSize, CancellationToken ct) =>
		(uint)Math.Ceiling(GetPeople().Length / (double)pageSize);

	public async ValueTask<(IImmutableList<Person> CurrentPage, int? NextPersonIdCursor)> GetPeopleAsync(int? personIdCursor, uint pageSize, CancellationToken ct)
	{
		// fake delay to simulate loading data
		await Task.Delay(TimeSpan.FromSeconds(1), ct);

		var people = GetPeople();

		var collection = people
			// select only subsequent items
			.Where(person => person.Id >= personIdCursor.GetValueOrDefault())
			// take only n number of rows, plus the first entity of the next page
			.Take((int)pageSize + 1)
			// using array to enable range access
			.ToArray();

		// determine if there's another page ahead
		var noMoreItems = collection.Length <= pageSize;

		// use the last item as the cursor of next page, if it exceeds the page-size
		var lastIndex = noMoreItems ? ^0 : ^1;
		var nextPersonIdCursor = noMoreItems ? default(int?) : collection[^1].Id;

		// this returns a tuple of two elements
		// first element is the current page's entities except the last
		// the second contains the last item in the collection, which is a cursor for next page
		return (CurrentPage: collection[..lastIndex].ToImmutableList(), NextPersonIdCursor: nextPersonIdCursor);
	}

	private Person[] GetPeople() =>
		new Person[]
		{
			new(1, "Liam", "Wilson"),
			new(2, "Emma", "Murphy"),
			new(3, "Noah", "Jones"),
			new(4, "Olivia", "Harris"),
			new(5, "William", "Jackson"),
			new(6, "Ava", "Martin"),
			new(7, "James", "Lee"),
			new(8, "Sophia", "Garcia"),
			new(9, "Logan", "Rodriguez"),
			new(10, "Isabella", "Martinez"),
			new(11, "Benjamin", "Davis"),
			new(12, "Mia", "Anderson"),
			new(13, "Mason", "Thomas"),
			new(14, "Charlotte", "Moore"),
			new(15, "Elijah", "Jackson"),
			new(16, "Amelia", "Johnson"),
			new(17, "Ethan", "White"),
			new(18, "Harper", "Clark"),
			new(19, "Michael", "Lewis"),
			new(20, "Evelyn", "Robinson"),
			new(21, "Daniel", "Walker"),
			new(22, "Abigail", "Perez"),
			new(23, "Alexander", "Hall"),
			new(24, "Emily", "Young"),
			new(25, "Matthew", "Allen"),
			new(26, "Madison", "King"),
			new(27, "Aiden", "Wright"),
			new(28, "Victoria", "Scott"),
			new(29, "Samuel", "Green"),
			new(30, "Chloe", "Baker"),
			new(31, "Christopher", "Adams"),
			new(32, "Sofia", "Nelson"),
			new(33, "Andrew", "Carter"),
			new(34, "Ella", "Mitchell"),
			new(35, "Joshua", "Parker"),
			new(36, "Addison", "Turner"),
			new(37, "Avery", "Phillips"),
			new(38, "David", "Campbell"),
			new(39, "Scarlett", "Parker"),
			new(40, "Joseph", "Evans"),
			new(41, "Lily", "Edwards"),
			new(42, "Nathan", "Collins"),
			new(43, "Aubrey", "Stewart"),
			new(44, "Brandon", "Sanchez"),
			new(45, "Hannah", "Morris"),
			new(46, "Justin", "Nguyen"),
			new(47, "Isabelle", "Rivera"),
			new(48, "Caleb", "Coleman"),
			new(49, "Samantha", "Gray"),
			new(50, "Mason", "Bryant"),
			new(51, "Zoe", "Cruz"),
			new(52, "Jacob", "Reed"),
			new(53, "Layla", "Henderson"),
			new(54, "Logan", "Gonzales"),
			new(55, "Gabriel", "Roberts"),
			new(56, "Audrey", "Turner"),
			new(57, "Lucas", "Phillips"),
			new(58, "Skylar", "Wilson"),
			new(59, "Ethan", "Gonzalez"),
			new(60, "Natalie", "Barnes"),
			new(61, "Kaylee", "Cox"),
			new(62, "William", "Ross"),
			new(63, "Aaliyah", "Cooper"),
			new(64, "Aiden", "Hayes"),
			new(65, "Brooklyn", "Green"),
			new(66, "Samuel", "Hill"),
			new(67, "Avery", "Baker"),
			new(68, "Benjamin", "Cruz"),
			new(69, "Leah", "Ortiz"),
			new(70, "David", "Garcia"),
			new(71, "Aubrey", "Barnes"),
			new(72, "Elijah", "Diaz"),
			new(73, "Emma", "Torres"),
			new(74, "Connor", "Rogers"),
			new(75, "Addison", "Peterson"),
			new(76, "Carter", "Coleman"),
			new(77, "Abigail", "West"),
			new(78, "Noah", "Foster"),
			new(79, "Lila", "Sanders"),
			new(80, "Christopher", "Powell"),
			new(81, "Caroline", "Sullivan"),
			new(82, "Mason", "Johnson"),
			new(83, "Grace", "Adams"),
			new(84, "Jackson", "Flores"),
			new(85, "Madelyn", "Mitchell"),
			new(86, "Aiden", "Butler"),
			new(87, "Eva", "Frazier"),
			new(88, "Lincoln", "Bishop"),
			new(89, "Emerson", "Walsh"),
			new(90, "Lydia", "Holt"),
			new(91, "Colin", "Morrison"),
			new(92, "Vivian", "Sharp"),
			new(93, "Finn", "Black"),
			new(94, "Cassidy", "Conner"),
			new(95, "Gabriella", "Higgins"),
			new(96, "Wyatt", "Barton"),
			new(97, "Makayla", "Lambert"),
			new(98, "Hudson", "Pierce"),
			new(99, "Jocelyn", "Harrington"),
			new(100, "Nathaniel", "Gibson"),
			new(101, "Aurora", "Fuller"),
			new(102, "Seth", "Fields"),
			new(103, "Kinsley", "Greer"),
			new(104, "Damian", "Reyes"),
			new(105, "Kendall", "Hodges"),
			new(106, "Landon", "Castillo"),
			new(107, "Malia", "Shaw"),
			new(108, "Maximus", "Fleming"),
			new(109, "Lyla", "Riley"),
			new(110, "Colton", "Navarro"),
			new(111, "Alexa", "Meadows"),
			new(112, "Emmett", "McGuire"),
			new(113, "Kiara", "Griffin"),
			new(114, "Brantley", "Summers"),
			new(115, "Delilah", "Wilkerson"),
			new(116, "Miles", "Hubbard"),
			new(117, "Elise", "Conrad"),
			new(118, "Dominic", "Barrera"),
			new(119, "Gianna", "Huang"),
			new(120, "Jaxson", "Vaughn"),
			new(121, "Aaliyah", "Nash"),
			new(122, "Joel", "Roman"),
			new(123, "Adalynn", "Dickson"),
			new(124, "Beau", "Owens"),
			new(125, "Brynn", "Huff"),
			new(126, "Dante", "Richmond"),
			new(127, "Celeste", "Pham"),
			new(128, "Ryker", "Keller"),
			new(129, "Eloise", "Sheppard"),
			new(130, "Israel", "Briggs"),
			new(131, "Ivy", "Velasquez"),
			new(132, "Maximilian", "Hoover"),
			new(133, "Alexandra", "Chan"),
			new(134, "Greyson", "Short"),
			new(135, "Iris", "Blevins"),
			new(136, "Jace", "Langley"),
			new(137, "Kira", "Benton"),
			new(138, "Ryland", "Orozco"),
			new(139, "Nora", "Wilcox"),
			new(140, "Tobias", "Knox"),
			new(141, "Amina", "Zhang"),
			new(142, "Griffin", "Wilkinson"),
			new(143, "Astrid", "Ritter"),
			new(144, "River", "Herman"),
			new(145, "Emersyn", "Hatfield"),
			new(146, "Rhys", "Sexton"),
			new(147, "Saylor", "Soria"),
			new(148, "Gideon", "Roach"),
			new(149, "Marley", "Lutz"),
			new(150, "Jagger", "Dougherty"),
			new(151, "Maren", "Frost"),
			new(152, "Emiliano", "McClain"),
			new(153, "Mikayla", "Cohen"),
			new(154, "Phoenix", "Acevedo"),
			new(155, "Averie", "Fulton"),
			new(156, "Ronan", "Whitaker"),
			new(157, "Elaina", "Werner"),
			new(158, "Kian", "Blackwell"),
			new(159, "Kaydence", "Hanna"),
			new(160, "Apollo", "Chase"),
			new(161, "Nina", "Lara"),
			new(162, "Zaiden", "Randall"),
			new(163, "Paloma", "Hester"),
			new(164, "Mauricio", "Morse")
		};
}
