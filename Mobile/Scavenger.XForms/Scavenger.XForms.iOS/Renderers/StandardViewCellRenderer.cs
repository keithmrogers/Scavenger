using System.Diagnostics;
using Scavenger.XForms.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(StandardViewCellRenderer))]
namespace Scavenger.XForms.iOS.Renderers
{
    public class StandardViewCellRenderer : ViewCellRenderer
    {
        public override UIKit.UITableViewCell GetCell (Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
        {
            var cell = base.GetCell (item, reusableCell, tv);
            Debug.WriteLine ("Style Id" + item.StyleId);
            switch (item.StyleId)
            {
                case "checkmark":
                    cell.Accessory = UIKit.UITableViewCellAccessory.Checkmark;
                    break;
                case "detail-button":
                    cell.Accessory = UIKit.UITableViewCellAccessory.DetailButton;
                    break;
                case "detail-disclosure-button":
                    cell.Accessory = UIKit.UITableViewCellAccessory.DetailDisclosureButton;
                    break;
                case "disclosure":
                    cell.Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator;
                    break;
                default:
                    cell.Accessory = UIKit.UITableViewCellAccessory.None;
                    break;
            }
            return cell;
        }
    }
}

