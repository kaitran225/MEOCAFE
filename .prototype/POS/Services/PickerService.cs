using Windows.Storage.Pickers;
using Windows.Storage;

namespace POS.Services
{
    public class PickerService
    {
        private readonly IntPtr _windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        public async Task<StorageFile> PickFileAsync()
        {

            var openPicker = new FileOpenPicker();
            
            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, _windowHandle);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".csv");
            openPicker.FileTypeFilter.Add(".xlsx");

            return await openPicker.PickSingleFileAsync();
        }

        public async Task<StorageFile> PickFileTrainedDataAsync()
        {

            var openPicker = new FileOpenPicker();

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, _windowHandle);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".zip");

            return await openPicker.PickSingleFileAsync();
        }

        public async Task<StorageFile?> OpenFileSavePickerAsync()
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "ExportedData"
            };

            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, _windowHandle);

            savePicker.FileTypeChoices.Add("CSV File", new List<string> { ".csv" });
            savePicker.FileTypeChoices.Add("Excel File", new List<string> { ".xlsx" });
            return await savePicker.PickSaveFileAsync();
        }

        public async Task<StorageFile?> OpenFileTrainSavePickerAsync()
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "SalesModel"
            };

            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, _windowHandle);

            // Zip file
            savePicker.FileTypeChoices.Add("Zip File", new List<string> { ".zip" });
            return await savePicker.PickSaveFileAsync();
        }
    }
}
