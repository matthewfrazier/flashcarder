using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DAP.Adorners;
using System.ComponentModel;
namespace Protomeme
{
	/// <summary>
	/// Interaction logic for ImageCropper.xaml
	/// </summary>
	public partial class ImageCropper : UserControl
	{
		public ImageCropper()
		{
			InitializeComponent();
			this.InitCropping();
		}

		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SourceProperty =
			Image.SourceProperty.AddOwner(typeof(ImageCropper),
			new PropertyMetadata(new PropertyChangedCallback(OnSourceChanged)));

		private static void OnSourceChanged(
			DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((ImageCropper)d).OnSourceChanged(e);
		}

		protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			this.imageToCrop.Source = e.NewValue as ImageSource;
			// add other initialization and cleanup code that is
			// required by your control when changing the image source
		}

		public Int32Rect CroppedImageBounds
		{
			get { return (Int32Rect)GetValue(CroppedImageBoundsProperty); }
			set { SetValue(CroppedImageBoundsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CroppedImageBounds.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CroppedImageBoundsProperty =
			DependencyProperty.Register("CroppedImageBounds", 
			typeof(Int32Rect), 
			typeof(ImageCropper), 
			new UIPropertyMetadata(Int32Rect.Empty,OnCroppedImageBoundsChanged));

		CroppingAdorner croppingAdorner;
		FrameworkElement _felCur = null;
		Brush originalBrush;


		private static void OnCroppedImageBoundsChanged(
			DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((ImageCropper)d).OnCroppedImageBoundsChanged(e);
		}

		protected virtual void OnCroppedImageBoundsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.CroppedImageBounds = (Int32Rect)e.NewValue;
			
			// add other initialization and cleanup code that is
			// required by your control when changing the image source
			//ApplyCroppedBounds(this.croppingAdorner);
		}

		protected virtual void InitCropping()
		{
			AddCropToElement(this.imageToCrop);
			originalBrush = croppingAdorner.Fill;
			ReportCroppedBounds(this.croppingAdorner);
		}

		private void AddCropToElement(FrameworkElement fel)
		{
			if (_felCur != null)
			{
				RemoveCropFromCur();
			}
			Rect rcInterior = new Rect(
				fel.ActualWidth * 0.2,
				fel.ActualHeight * 0.2,
				fel.ActualWidth * 0.6,
				fel.ActualHeight * 0.6);
			AdornerLayer aly = AdornerLayer.GetAdornerLayer(fel);
			croppingAdorner = new CroppingAdorner(fel, rcInterior);
			aly.Add(croppingAdorner);

			croppingAdorner.CropChanged += CropChanged;
			_felCur = fel;
			SetClipColorGrey();
		}

		private void RemoveCropFromCur()
		{
			AdornerLayer aly = AdornerLayer.GetAdornerLayer(_felCur);
			aly.Remove(croppingAdorner);
		}

		private void SetClipColorGrey()
		{
			if (croppingAdorner != null)
			{
				Color clr = Colors.Black;
				clr.A = 110;
				croppingAdorner.Fill = new SolidColorBrush(clr);
			}
		}

		protected virtual void ApplyCroppedBounds(CroppingAdorner ca)
		{
			if (ca == null)
				return;
			Rect rc = ca.ClippingRectangle;

			if (this.imageToCrop.Source != null)
			{
				double imageWidth = this.imageToCrop.Source.Width;
				double imageHeight = this.imageToCrop.Source.Height;

				double xscale = ca.ActualWidth / imageWidth;
				double yscale = ca.ActualHeight / imageHeight;

				double newLeft = rc.Left * xscale;
				double newTop = rc.Top * yscale;
				double newWidth = rc.Width * xscale;
				double newHeight = rc.Height * yscale;

				ca.ClippingRectangle = new Rect(
					newLeft, newTop, newWidth, newHeight);
			}

		}

		protected virtual void ReportCroppedBounds(CroppingAdorner ca)
		{
			if (ca == null)
				return;
			Rect rc = ca.ClippingRectangle;
			//TODO: this is the rect in screen coords, need to convert them back to image coords

			if (this.imageToCrop.Source != null)
			{
				double imageWidth = this.imageToCrop.Source.Width;
				double imageHeight = this.imageToCrop.Source.Height;

				double xscale = ca.ActualWidth / imageWidth;
				double yscale = ca.ActualHeight / imageHeight;

				int newLeft = (int)Math.Round(rc.Left * xscale);
				int newTop = (int)Math.Round(rc.Top * yscale);
				int newWidth = (int)Math.Round(rc.Width * xscale);
				int newHeight = (int)Math.Round(rc.Height * yscale);
				this.CroppedImageBounds = new Int32Rect(
					newLeft,
					newTop,
					newWidth,
					newHeight
					);
			}
		}

		private void CropChanged(Object sender, RoutedEventArgs rea)
		{
			ReportCroppedBounds(sender as CroppingAdorner);
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ReportCroppedBounds(this.croppingAdorner);
		}
	}
}
