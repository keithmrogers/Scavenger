using System;
using Acquaint.Data;
using FFImageLoading;
using FFImageLoading.Transformations;
using UIKit;

namespace Acquaint.Native.iOS
{
	/// <summary>
	/// Acquaintance cell. The layout for this Cell is defined almost entirely in Main.storyboard.
	/// </summary>
	public partial class AcquaintanceCell : UITableViewCell
	{
		// This constructor signature is required, for marshalling between the managed and native instances of this class.
		public AcquaintanceCell(IntPtr handle) : base(handle) { }

		/// <summary>
		/// Update the cell's child views' values and presentation.
		/// </summary>
		/// <param name="acquaintance">Acquaintance.</param>
		public void Update(Acquaintance acquaintance)
		{
			// use FFImageLoading library to:
			ImageService
				.LoadFileFromApplicationBundle(String.Format(acquaintance.PhotoUrl)) 	// get the image from the app bundle
				.LoadingPlaceholder("placeholderProfileImage.png") 						// specify a placeholder image
				.Transform(new CircleTransformation()) 									// transform the image to a circle
				.Into(ProfilePhotoImageView); 											// load the image into the UIImageView

			// use FFImageLoading library to asynchronously:
			//	ImageService
			//		.LoadUrl(acquaintance.SmallPhotoUrl) 				// get the image from a URL
			//		.LoadingPlaceholder("placeholderProfileImage.png") 	// specify a placeholder image
			//		.Transform(new CircleTransformation()) 				// transform the image to a circle
			//		.IntoAsync(ProfilePhotoImageView); 					// load the image into the UIImageView

				NameLabel.Text = acquaintance.DisplayLastNameFirst;
				CompanyLabel.Text = acquaintance.Company;
				JobTitleLabel.Text = acquaintance.JobTitle;

				// set disclousure indicator accessory for the cell
				Accessory = UITableViewCellAccessory.DisclosureIndicator;

				// add the colored border to the image
				double min = Math.Min(ProfilePhotoImageView.Frame.Height, ProfilePhotoImageView.Frame.Height);
				ProfilePhotoImageView.Layer.CornerRadius = (float)(min / 2.0);
				ProfilePhotoImageView.Layer.MasksToBounds = false;
				ProfilePhotoImageView.Layer.BorderColor = UIColor.FromRGB(84, 119, 153).CGColor;
				ProfilePhotoImageView.Layer.BorderWidth = 3;
				ProfilePhotoImageView.BackgroundColor = UIColor.Clear;
				ProfilePhotoImageView.ClipsToBounds = true;
		}
	}
}
