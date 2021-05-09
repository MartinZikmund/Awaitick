using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using EventCountdowns.Core.Services.BackgroundPicker;

namespace EventCountdowns.Core.Services
{
    public class BackgroundPickerService : IBackgroundPickerService
    {
        private const int BufferSize = 1024;

        public async Task<string> PickBackgroundAsync()
        {
            //pick image
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add( ".jpg" );
            filePicker.FileTypeFilter.Add( ".jpeg" );
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            StorageFile file = await filePicker.PickSingleFileAsync();
            if ( file == null )
            {
                return null;
            }
            else
            {
                var rawFileStream = await file.OpenAsync( FileAccessMode.Read );
                var resizedStream = await ResizeJpegStreamAsync( 2000, 100, rawFileStream );
                var inputStream = resizedStream.GetInputStreamAt( 0 );
                Guid backgoundId = Guid.NewGuid();
                var userBackgroundsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync( "UserBackgrounds", CreationCollisionOption.OpenIfExists );
                var imageFile = await userBackgroundsFolder.CreateFileAsync( backgoundId + ".jpg" );
                using ( DataReader reader = new DataReader( inputStream ) )
                {
                    using ( IRandomAccessStream fileStream = await imageFile.OpenAsync( FileAccessMode.ReadWrite ) )
                    {
                        using ( IOutputStream outputStream = fileStream.GetOutputStreamAt( 0 ) )
                        {
                            using ( DataWriter writer = new DataWriter( outputStream ) )
                            {
                                const int bufferSize = 8192;

                                uint loadedBytes = 0;
                                while ( ( loadedBytes = ( await reader.LoadAsync( bufferSize ) ) ) > 0 )
                                {
                                    IBuffer buffer = reader.ReadBuffer( loadedBytes );
                                    writer.WriteBuffer( buffer );
                                    await writer.StoreAsync();
                                }
                            }
                        }
                    }
                }
                return imageFile.Path;
            }
        }


        public void ChoosePictureFromLibrary( int maxPixelDimension, int percentQuality, Action<Stream> pictureAvailable, Action assumeCancelled )
        {
            TakePictureCommon( StorageFileFromDisk, maxPixelDimension, percentQuality, pictureAvailable, assumeCancelled );
        }

        public void TakePicture( int maxPixelDimension, int percentQuality, Action<Stream> pictureAvailable, Action assumeCancelled )
        {
            TakePictureCommon( StorageFileFromCamera, maxPixelDimension, percentQuality, pictureAvailable, assumeCancelled );
        }

        public Task<Stream> ChoosePictureFromLibrary( int maxPixelDimension, int percentQuality )
        {
            var task = new TaskCompletionSource<Stream>();
            ChoosePictureFromLibrary( maxPixelDimension, percentQuality, task.SetResult, () => task.SetResult( null ) );
            return task.Task;
        }

        public Task<Stream> TakePicture( int maxPixelDimension, int percentQuality )
        {
            var task = new TaskCompletionSource<Stream>();
            TakePicture( maxPixelDimension, percentQuality, task.SetResult, () => task.SetResult( null ) );
            return task.Task;
        }

        public void ContinueFileOpenPicker( object args )
        {

        }

        private async void TakePictureCommon( Func<Task<StorageFile>> storageFile, int maxPixelDimension, int percentQuality, Action<Stream> pictureAvailable,
                                             Action assumeCancelled )
        {
            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            await dispatcher.RunAsync( CoreDispatcherPriority.Normal,
                                async () =>
                                {
                                    await
                                        Process( storageFile, maxPixelDimension, percentQuality, pictureAvailable,
                                                assumeCancelled );
                                } );
        }

        private async Task Process( Func<Task<StorageFile>> storageFile, int maxPixelDimension, int percentQuality, Action<Stream> pictureAvailable, Action assumeCancelled )
        {
            var file = await storageFile();
            if ( file == null )
            {
                assumeCancelled();
                return;
            }

            var rawFileStream = await file.OpenAsync( FileAccessMode.Read );
            var resizedStream = await ResizeJpegStreamAsync( maxPixelDimension, percentQuality, rawFileStream );

            pictureAvailable( resizedStream.AsStreamForRead() );
        }

        private static async Task<StorageFile> StorageFileFromCamera()
        {
            var dialog = new CameraCaptureUI();
            var file = await dialog.CaptureFileAsync( CameraCaptureUIMode.Photo );
            return file;
        }

        private static async Task<StorageFile> StorageFileFromDisk()
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add( ".jpg" );
            filePicker.FileTypeFilter.Add( ".jpeg" );
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            return await filePicker.PickSingleFileAsync();
        }

        private async Task<IRandomAccessStream> ResizeJpegStreamAsync( int maxPixelDimension, int percentQuality, IRandomAccessStream input )
        {
            var decoder = await BitmapDecoder.CreateAsync( input );

            int targetHeight;
            int targetWidth;
            TargetWidthAndHeight( maxPixelDimension, ( int )decoder.PixelWidth, ( int )decoder.PixelHeight, out targetWidth, out targetHeight );

            var transform = new BitmapTransform() { ScaledHeight = ( uint )targetHeight, ScaledWidth = ( uint )targetWidth };
            var pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Rgba8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.DoNotColorManage );

            var destinationStream = new InMemoryRandomAccessStream();
            var bitmapPropertiesSet = new BitmapPropertySet();
            bitmapPropertiesSet.Add( "ImageQuality", new BitmapTypedValue( ( ( double )percentQuality ) / 100.0, PropertyType.Single ) );
            var encoder = await BitmapEncoder.CreateAsync( BitmapEncoder.JpegEncoderId, destinationStream, bitmapPropertiesSet );
            encoder.SetPixelData( BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, ( uint )targetWidth, ( uint )targetHeight, decoder.DpiX, decoder.DpiY, pixelData.DetachPixelData() );
            await encoder.FlushAsync();
            destinationStream.Seek( 0L );
            return destinationStream;
        }

        private void TargetWidthAndHeight( int maxPixelDimension, int currentWidth, int currentHeight, out int targetWidth, out int targetHeight )
        {
            var ratio = 1.0;
            if ( currentWidth > currentHeight )
                ratio = ( maxPixelDimension ) / ( ( double )currentWidth );
            else
                ratio = ( maxPixelDimension ) / ( ( double )currentHeight );

            targetWidth = ( int )Math.Round( ratio * currentWidth );
            targetHeight = ( int )Math.Round( ratio * currentHeight );
        }
    }
}
