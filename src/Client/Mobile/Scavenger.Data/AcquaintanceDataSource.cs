using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace Scavenger.Data
{
	public class AcquaintanceDataSource : IDataSource<Acquaintance>
	{
		public AcquaintanceDataSource(bool simulateNetworkLatency = true)
		{
			if (!simulateNetworkLatency)
				_LatencyTimeSpan = TimeSpan.Zero;

			_RootFolder = FileSystem.Current.LocalStorage;
		}

		#region IDataSource implementation

		public async Task SaveItem(Acquaintance item)
		{
			await EnsureInitialized().ConfigureAwait(false);

			if (_Acquaintances.Any(c => c.Id == item.Id))
			{
				int i = _Acquaintances.IndexOf(item);
				_Acquaintances[i] = item;
			}
			else
			{
				_Acquaintances.Add(item);
			}

			await WriteFile(_RootFolder, _FileName, JsonConvert.SerializeObject(_Acquaintances)).ConfigureAwait(false);
		}

		public async Task DeleteItem(string id)
		{
			await EnsureInitialized().ConfigureAwait(false);

			_Acquaintances.RemoveAll(c => c.Id == id);

			await WriteFile(_RootFolder, _FileName, JsonConvert.SerializeObject(_Acquaintances)).ConfigureAwait(false);
		}

		public async Task<Acquaintance> GetItem(string id)
		{
			await EnsureInitialized().ConfigureAwait(false);

			return await Task.FromResult(_Acquaintances.SingleOrDefault(x => x.Id == id)).ConfigureAwait(false);
		}

		public async Task<ICollection<Acquaintance>> GetItems(int start = 0, int count = 100, string query = "")
		{
			await EnsureInitialized().ConfigureAwait(false);

			await Latency;

			var items = AcquaintanceDataSourceHelper
				.BasicQueryFilter(_Acquaintances, query)
				.Skip(start)
				.Take(count)
				.ToList();

			return items;
		}

		#endregion

		const string _FileName = "acquaintances.json";

	    readonly IFolder _RootFolder;

		// you can change this value to simulate network latency
		static readonly int latencyInSeconds = 0;
		readonly TimeSpan _LatencyTimeSpan = TimeSpan.FromSeconds(latencyInSeconds);

		bool _IsInitialized;

		List<Acquaintance> _Acquaintances;

		async Task Initialize()
		{
			if (!await FileExists(_RootFolder, _FileName).ConfigureAwait(false))
			{
				await CreateFile(_RootFolder, _FileName).ConfigureAwait(false);
			}

			if (string.IsNullOrWhiteSpace(await GetFileContents(await GetFile(_RootFolder, _FileName).ConfigureAwait(false)).ConfigureAwait(false)))
			{
				_Acquaintances = GenerateAcquaintances();

				await WriteFile(_RootFolder, _FileName, JsonConvert.SerializeObject(_Acquaintances)).ConfigureAwait(false);
			}
			else
			{
				_Acquaintances = JsonConvert.DeserializeObject<List<Acquaintance>>(await GetFileContents(await GetFile(_RootFolder, _FileName).ConfigureAwait(false)).ConfigureAwait(false));    
			}

			_IsInitialized = true;
		}

		async Task EnsureInitialized()
		{
			if (!_IsInitialized)
				await Initialize().ConfigureAwait(false);
		}

		/// <summary>
		/// Generates the acquaintances.
		/// </summary>
		/// <returns>The acquaintances.</returns>
		static List<Acquaintance> GenerateAcquaintances()
		{
			return new List<Acquaintance>()
			{
				new Acquaintance() { Id = "00004363-F79A-44E7-BC32-6128E2EC8401", FirstName = "Joseph", LastName = "Grimes", Company = "GG Mechanical", JobTitle = "Vice President", Email = "jgrimes@ggmechanical.com", Phone = "414-367-4348", Street = "2030 Judah St", City = "San Francisco", PostalCode = "94144", State = "CA", PhotoUrl = "josephgrimes.jpg" },
				new Acquaintance() { Id = "c227bfd2-c6f6-49b5-93ec-afef9eb18d08", FirstName = "Monica", LastName = "Green", Company = "Calcom Logistics", JobTitle = "Director", Email = "mgreen@calcomlogistics.com", Phone = "925-353-8029", Street = "231 3rd Ave", City = "San Francisco", PostalCode = "94118", State = "CA", PhotoUrl = "monicagreen.jpg" },
				new Acquaintance() { Id = "31bf6fe5-18f1-4354-9571-2cdecb0c00af", FirstName = "Joan", LastName = "Mancum", Company = "Bay Unified School District", JobTitle = "Principal", Email = "joan.mancum@busd.org", Phone = "914-870-7670", Street = "448 Grand Ave", City = "South San Francisco", PostalCode = "94080", State = "CA", PhotoUrl = "joanmancum.jpg" },
				new Acquaintance() { Id = "45d2ddc0-a8e9-4aea-8b51-2860c708e30d", FirstName = "Alvin", LastName = "Gray", Company = "Pacific Cabinetry", JobTitle = "Office Manager", Email = "agray@pacificcabinets.com", Phone = "720-344-7823", Street = "1773 Lincoln St", City = "Santa Clara", PostalCode = "95050", State = "CA", PhotoUrl = "alvingray.jpg" },
				new Acquaintance() { Id = "c9ebe513-0db2-41d3-b595-20a49454a421", FirstName = "Michelle", LastName = "Wilson", Company = "Evergreen Mechanical", JobTitle = "Sales Manager", Email = "mwilson@evergreenmech.com", Phone = "917-245-7975", Street = "208 Jackson St", City = "San Jose", PostalCode = "95112", State = "CA", PhotoUrl = "michellewilson.jpg" },
				new Acquaintance() { Id = "e4029998-5d6e-4ed8-b802-e6f940f307a1", FirstName = "Jennifer", LastName = "Gillespie", Company = "Peninsula University", JobTitle = "Superintendent", Email = "jgillespie@peninsula.org", Phone = "831-427-6746", Street = "10002 N De Anza Blvd", City = "Cupertino", PostalCode = "95014", State = "CA", PhotoUrl = "jennifergillespie.jpg" },
				new Acquaintance() { Id = "2323e8b6-ed1c-44fe-9cff-90dcd97d3bb5", FirstName = "Thomas", LastName = "White", Company = "Creative Automotive Group", JobTitle = "Service Manager", Email = "tom.white@creativeauto.com", Phone = "214-865-0771", Street = "1181 Linda Mar Blvd", City = "Pacifica", PostalCode = "94044", State = "CA", PhotoUrl = "thomaswhite.jpg" },
				new Acquaintance() { Id = "00F8A566-2538-4AF7-AE10-997B61537DC0", FirstName = "Leon", LastName = "Muks", Company = "Spacey", JobTitle = "President", Email = "leon.muks@spacey.io", Phone = "310-586-0181", Street = "2518 Durant Ave", City = "Berkeley", PostalCode = "94704", State = "CA", PhotoUrl = "leonmuks.jpg" },
				new Acquaintance() { Id = "FEB64319-1222-4C76-A8E5-EDFF84838B43", FirstName = "Floyd", LastName = "Bell", Company = "Netcore", JobTitle = "Procurement", Email = "floyd.bell@netcore.net", Phone = "603-226-4115", Street = "450 15th St", City = "Oakland", PostalCode = "94612", State = "CA", PhotoUrl = "floydbell.jpg" },
				new Acquaintance() { Id = "4FB56717-D2CF-4A5F-894A-C87383A8239D", FirstName = "Vanessa", LastName = "Thornton", Company = "Total Sources", JobTitle = "Product Manager", Email = "vanessa.thornton@totalsourcesinc.com", Phone = "419-998-6611", Street = "550 Quarry Rd", City = "San Carlos", PostalCode = "94070", State = "CA", PhotoUrl = "vanessathornton.jpg" },
				new Acquaintance() { Id = "89149CDA-5B31-4B15-88D2-6011B028F7AE", FirstName = "John", LastName = "Boone", Company = "A. L. Price", JobTitle = "Executive Associate", Email = "jboone@alpricellc.com", Phone = "973-579-4610", Street = "233 E Harris Ave", City = "South San Francisco", PostalCode = "94080", State = "CA", PhotoUrl = "johnboone.jpg" },
				new Acquaintance() { Id = "BFD74C2A-7840-45DD-9C47-13F90DE01F8B", FirstName = "Ann", LastName = "Temple", Company = "Foxmoor", JobTitle = "Director", Email = "ann.temple@foxmoorinc.com", Phone = "608-821-7667", Street = "1270 San Pablo Ave", City = "Berkeley", PostalCode = "94706", State = "CA", PhotoUrl = "anntemple.jpg" },
				new Acquaintance() { Id = "CF568F71-C4B6-45A9-A922-C9E69EF17B49", FirstName = "Joseph", LastName = "Meeks", Company = "Rose Records", JobTitle = "Manager", Email = "jmeeks@roserecordsllc.com", Phone = "978-628-6826", Street = "28 N 1st St", City = "San Jose", PostalCode = "95113", State = "CA", PhotoUrl = "josephmeeks.jpg" },
				new Acquaintance() { Id = "D81687FD-FE84-4F0F-AFDD-A104B28EC1A7", FirstName = "Michelle", LastName = "Herring", Company = "Full Color", JobTitle = "Production Specialist", Email = "michelle.herring@fullcolorus.com", Phone = "201-319-9344", Street = "213 2nd Ave", City = "San Mateo", PostalCode = "94401", State = "CA", PhotoUrl = "michelleherring.jpg" },
				new Acquaintance() { Id = "FB580ADA-4381-43EE-ADA1-D072878F08CE", FirstName = "Daniel", LastName = "Jones", Company = "Flexus", JobTitle = "Quality Assurance Associate", Email = "daniel.jones@flexusinc.com", Phone = "228-432-8712", Street = "850 Bush St", City = "San Francisco", PostalCode = "94108", State = "CA", PhotoUrl = "danieljones.jpg" },
				new Acquaintance() { Id = "953d9588-e6be-49cf-881d-68431b8285c3", FirstName = "Margaret", LastName = "Cargill", Company = "Redwood City Medical Group", JobTitle = "Director", Email = "mcargill@rcmg.org", Phone = "208-816-9793", Street = "1037 Middlefield Road", City = "Redwood City", PostalCode = "94063", State = "CA", PhotoUrl = "margaretcargill.jpg" },
				new Acquaintance() { Id = "450fe593-433f-4bca-9f39-f2a0e4c64dc6", FirstName = "Benjamin", LastName = "Jones", Company = "JH Manufacturing", JobTitle = "Head of Manufacturing", Email = "ben.jones@jh.com", Phone = "505.562.3086", Street = "2091 Cowper St", City = "Palo Alto", PostalCode = "94306", State = "CA", PhotoUrl = "benjaminjones.jpg" },
				new Acquaintance() { Id = "5c957b8f-6e76-470c-941f-789d12f10a42", FirstName = "Ivan", LastName = "Diaz", Company = "XYZ Robotics", JobTitle = "CEO", Email = "ivan.diaz@xyzrobotics.com", Phone = "406-496-8774", Street = "1960 Mandela Parkway", City = "Oakland", PostalCode = "94607", State = "CA", PhotoUrl = "ivandiaz.jpg" },
				new Acquaintance() { Id = "6FEFF721-2A97-4C0F-AACB-30B1F521ABF6", FirstName = "Eric", LastName = "Grant", Company = "MMSRI, Inc.", JobTitle = "Senior Manager", Email = "egrant@mmsri.com", Phone = "360-693-2388", Street = "2043 Martin Luther King Jr. Way", City = "Berkeley", PostalCode = "94704", State = "CA", PhotoUrl = "ericgrant.jpg" },
				new Acquaintance() { Id = "CA0A6161-6898-421D-9F29-A51B60F36BEE", FirstName = "Stacey", LastName = "Valdovinos", Company = "Global Manufacturing", JobTitle = "CEO", Email = "svaldovinos@globalmanuf.com", Phone = "440-243-7987", Street = "98 Udayakavi Lane", City = "Danville", PostalCode = "94526", State = "CA", PhotoUrl = "staceyvaldovinos.jpg" },
				new Acquaintance() { Id = "9CD6310F-1439-4898-9F51-EEC96D032CD3", FirstName = "Jesus", LastName = "Cardell", Company = "Pacific Marine Supply", JobTitle = "Manager", Email = "jcardella@pacificmarine.com", Phone = "410-745-5521", Street = "1008 Rachele Road", City = "Walnut Creek", PostalCode = "94597", State = "CA", PhotoUrl = "jesuscardell.jpg" },
				new Acquaintance() { Id = "D5E85894-129F-4F39-A75D-893DAB128ECD", FirstName = "Wilma", LastName = "Woolley", Company = "Mission School District", JobTitle = "Superintendent", Email = "wwoolley@missionsd.org", Phone = "940-696-1852", Street = "7277 Moeser Lane", City = "El Cerrito", PostalCode = "94530", State = "CA", PhotoUrl = "wilmawoolley.jpg" },
				new Acquaintance() { Id = "6CF4DE3E-FE50-4860-8E5C-6DCF479D4737", FirstName = "Evan", LastName = "Armstead", Company = "City of Richmond", JobTitle = "Board Member", Email = "evan.armstead@richmond.org", Phone = "415-336-2228", Street = "398 23rd St", City = "Richmond", PostalCode = "94804", State = "CA", PhotoUrl = "evanarmstead.jpg" },
				new Acquaintance() { Id = "DAFB9C5C-54A3-4F18-BC01-10AD2491AEC7", FirstName = "James", LastName = "Jones", Company = "East Bay Commercial Bank", JobTitle = "Manager", Email = "james.jones@eastbaybank.com", Phone = "313-248-7644", Street = "4501 Pleasanton Way", City = "Pleasanton", PostalCode = "94556", State = "CA", PhotoUrl = "jamesjones.jpg" },
				new Acquaintance() { Id = "AB6F1601-94F3-4E32-A08A-089B5B52DA36", FirstName = "Douglas", LastName = "Greenly", Company = "Bay Tech Credit Union", JobTitle = "Vice President", Email = "d.greenly@baytechcredit.com", Phone = "201-929-0094", Street = "2267 Alameda Ave", City = "Alameda", PostalCode = "94501", State = "CA", PhotoUrl = "douglasgreenly.jpg" },
				new Acquaintance() { Id = "70EB3223-4ED2-4FE2-9AC1-F72B474FF05F", FirstName = "Brent", LastName = "Mason", Company = "Rockridge Hotel", JobTitle = "Concierge", Email = "brent.mason@rockridgehotel.com", Phone = "940-482-7759", Street = "1960 Mandela Parkway", City = "Oakland", PostalCode = "94607", State = "CA", PhotoUrl = "brentmason.jpg" },
				new Acquaintance() { Id = "A5A8F111-FE08-4478-A90B-222F4BA033DD", FirstName = "Richard", LastName = "Hogan", Company = "Marin Luxury Senior Living", JobTitle = "Customer Care", Email = "rhogan@marinseniorliving.com", Phone = "978-658-7545", Street = "674 Tiburon Blvd", City = "Belvedere Tiburon", PostalCode = "94920", State = "CA", PhotoUrl = "richardhogan.jpg" },
				new Acquaintance() { Id = "6348C5F4-2073-4868-959C-D1650FD8C186", FirstName = "Daniel", LastName = "Granville", Company = "Cityview Consulting", JobTitle = "Consultant", Email = "dgranville@cityviewconsulting.com", Phone = "330-616-7467", Street = "300 Spencer Ave", City = "Sausalito", PostalCode = "94965", State = "CA", PhotoUrl = "danielgranville.jpg" },
				new Acquaintance() { Id = "303A5E88-E91D-43ED-9391-FDE9F7C03A66", FirstName = "Margaret", LastName = "Kidd", Company = "Marin Cultural Center", JobTitle = "President", Email = "mkidd@marincultural.org", Phone = "406-784-0602", Street = "106 Throckmorton Ave", City = "Mill Valley", PostalCode = "94941", State = "CA", PhotoUrl = "margaretkidd.jpg" },
				new Acquaintance() { Id = "0782C981-F003-44A4-87D1-771D3C6EB6B3", FirstName = "Leo", LastName = "Parson", Company = "San Rafel Chamber of Commerce", JobTitle = "Board Member", Email = "leo.parson@sanrafaelcoc.org", Phone = "773-991-5214", Street = "199 Clorinda Ave", City = "San Rafael", PostalCode = "94901", State = "CA", PhotoUrl = "leoparson.jpg" },
			};
		}

		static async Task<bool> FileExists(IFolder folder, string fileName)
		{
			return await Task.FromResult<bool>(await folder.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists).ConfigureAwait(false);
		}

		static async Task<IFile> CreateFile(IFolder folder, string fileName)
		{
			return await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
		}

		static async Task<IFile> GetFile(IFolder folder, string fileName)
		{
			return await folder.GetFileAsync(fileName).ConfigureAwait(false);
		}

		static async Task WriteFile(IFolder folder, string fileName, string fileContents)
		{
			var file = await GetFile(folder, fileName).ConfigureAwait(false);

			await file.WriteAllTextAsync(fileContents).ConfigureAwait(false);
		}

		static async Task<string> GetFileContents(IFile file)
		{
			return await file.ReadAllTextAsync().ConfigureAwait(false);
		}

		Task Latency
		{
			get
			{
				var random = new Random();
				var ms = random.Next((int)_LatencyTimeSpan.TotalMilliseconds);
				return Task.Delay(ms);
			}
		}
	}

	public static class AcquaintanceDataSourceHelper
	{
		static int MatchScore(Acquaintance c, string query)
		{
			return new[]
			{
				$"{c.FirstName} {c.LastName}",
				c.Email,
				c.Company,
			}.Sum(label => MatchScore(label, query));
		}

		static int MatchScore(string data, string query)
		{
			int score = query.Length;

			if (string.IsNullOrEmpty(data))
				return 0;

			data = data.ToLower();
			if (!data.Contains(query))
				return 0;

			if (data == query)
				score += 2;
			else if (data.StartsWith(query))
				score++;

			return score;
		}

		public static IEnumerable<Acquaintance> BasicQueryFilter(IEnumerable<Acquaintance> source, string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				return source.OrderBy(e => e.LastName ?? "");
			}

			query = query.ToLower();
			return source
				.Select(c => Tuple.Create(MatchScore(c, query), c))
				.Where(c => c.Item1 != 0)
				.OrderByDescending(e => e.Item1)
				.Select(c => c.Item2);
		}
	}
}
