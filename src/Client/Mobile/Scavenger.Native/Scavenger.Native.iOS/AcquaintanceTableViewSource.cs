using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acquaint.Data;
using Foundation;
using UIKit;

namespace Acquaint.Native.iOS
{
	public class AcquaintanceTableViewSource : UITableViewSource
	{
		/// <summary>
		/// The acquaintance data source.
		/// </summary>
		readonly IDataSource<Acquaintance> _AcquaintanceDataSource;

		/// <summary>
		/// Gets the acquaintances.
		/// </summary>
		/// <value>The acquaintances.</value>
		public List<Acquaintance> Acquaintances { get; private set; }

		public AcquaintanceTableViewSource()
        {
			_AcquaintanceDataSource = new AcquaintanceDataSource();
        }

		// <summary>
		// Loads the acquaintances.
		// </summary>
		// <returns>The acquaintances.</returns>
		public async Task LoadAcquaintances()
		{
			Acquaintances = (await _AcquaintanceDataSource.GetItems()).ToList();
		}

		#region implemented abstract members of UITableViewSource

		/// <summary>
		/// Gets the cell.
		/// </summary>
		/// <returns>The cell.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// try to get a cell that's currently off-screen, not being displayed
			var cell = tableView.DequeueReusableCell("AcquaintanceCell", indexPath) as AcquaintanceCell;

			// get an item from the collection, using the table row index as the collection index
			var acquaintance = Acquaintances[indexPath.Row];

			cell.Update(acquaintance);

			return cell;
		}

		/// <summary>
		/// Gets the number of rows in a table section.
		/// </summary>
		/// <returns>The number of items in the section.</returns>
		/// <param name="tableview">Tableview.</param>
		/// <param name="section">Section.</param>
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Acquaintances.Count;
		}

		#endregion
	}
}

