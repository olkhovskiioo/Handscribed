using Handscribed.DataLoader;
using Handscribed.Dataset;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using Handscribed.WpfUtils;
using System.Collections.ObjectModel;
using System;

namespace Handscribed.TrainDataViewer
{
    public class TrainDataViewerViewModel : ObservableObject
    {
        private List<MnistImage> _mnistData;
        private int _currentIndex;
        private BitmapSource _currentImage;
        private int _imageCount;
        private string _currentLabel;
        private string _currentIndexText;
        private bool _isDataLoaded;
        private ObservableCollection<string> _MNISTOptions;
        private string _selectedMNISTOption;
        private string _folderName;

        public TrainDataViewerViewModel()
        {
            LoadCommand = new RelayCommand(SelectDataFolder);
            PreviousCommand = new RelayCommand(PreviousImage, CanNavigatePrevious);
            NextCommand = new RelayCommand(NextImage, CanNavigateNext);
            _MNISTOptions = new ObservableCollection<string>() { Consts.TRAIN_DATA_TYPE_1, Consts.TRAIN_DATA_TYPE_2 };

            if (!string.IsNullOrEmpty(Properties.Settings.Default.TrainingDataPath) && Path.Exists(Properties.Settings.Default.TrainingDataPath))
                _folderName = Properties.Settings.Default.TrainingDataPath;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.TrainingDataType))
                SelectedMNISTOption = Properties.Settings.Default.TrainingDataType;

        }

        public event EventHandler DataSelected;
        public ObservableCollection<string> MNISTOptions => _MNISTOptions; 
        public string SelectedMNISTOption
        {
            get => _selectedMNISTOption;
            set
            {
                _selectedMNISTOption = value;
                OnPropertyChanged();
                SetData(value, _folderName);
                DataSelected?.Invoke(this, EventArgs.Empty);
            }
        }


        public ICommand LoadCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public BitmapSource CurrentImage
        {
            get => _currentImage;
            set => SetField(ref _currentImage, value);
        }

        public string CurrentLabel
        {
            get => _currentLabel;
            set => SetField(ref _currentLabel, value);
        }

        public string CurrentIndexText
        {
            get => _currentIndexText;
            set => SetField(ref _currentIndexText, value);
        }

        public int ImageCount
        {
            get => _imageCount;
            set => SetField(ref _imageCount, value);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (value >= 0 && value < ImageCount)
                {
                    SetField(ref _currentIndex, value);
                    ShowImage(value);
                }
            }
        }

        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set => SetField(ref _isDataLoaded, value);
        }
        public string FolderName => _folderName;

        private void SelectDataFolder(object parameter)
        {
            try
            {
                var dialog = new OpenFolderDialog
                {
                    Title = "Select MNIST folder"
                };

                if (dialog.ShowDialog() == true)
                {
                    _folderName = dialog.FolderName;
                    SelectedMNISTOption = MNISTOptions.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetData(string type, string folder)
        {
            var (dataPath, labelsPath) = GetDataFilePath(folder); 
            _mnistData = MnistDataLoader.LoadImages(dataPath, labelsPath);
            ImageCount = _mnistData.Count;
            IsDataLoaded = true;
            ShowImage(0);
        }

        
        private (string, string) GetDataFilePath(string folder)
        {
            switch (SelectedMNISTOption)
            {
                case Consts.TRAIN_DATA_TYPE_1:
                    return (Path.Combine(folder, Consts.TRAIN_DATA_TYPE_1_FILE_NAME), Path.Combine(folder, Consts.TRAIN_LABELS_TYPE_1_FILE_NAME));
                case Consts.TRAIN_DATA_TYPE_2:
                    return (Path.Combine(folder, Consts.TRAIN_DATA_TYPE_2_FILE_NAME), Path.Combine(folder, Consts.TRAIN_LABELS_TYPE_2_FILE_NAME));
                default:
                    throw new Exception("unknown train data type");
            }
        }
        private void ShowImage(int index)
        {
            if (_mnistData == null || index < 0 || index >= _mnistData.Count)
                return;

            _currentIndex = index;
            OnPropertyChanged(nameof(CurrentIndex));
            var data = _mnistData[index];

            CurrentIndexText = $"{index} / {_mnistData.Count - 1}";
            CurrentLabel = data.Label.ToString();
            CurrentImage = BitmapConverter.ConvertToBitmap(data);

            ((RelayCommand)PreviousCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
        }

        private void PreviousImage(object parameter)
        {
            if (_currentIndex > 0)
            {
                ShowImage(_currentIndex - 1);
            }
        }

        private bool CanNavigatePrevious(object parameter)
        {
            return _mnistData != null && _currentIndex > 0;
        }

        private void NextImage(object parameter)
        {
            if (_currentIndex < _mnistData.Count - 1)
            {
                ShowImage(_currentIndex + 1);
            }
        }

        private bool CanNavigateNext(object parameter)
        {
            return _mnistData != null && _currentIndex < _mnistData.Count - 1;
        }

        internal void Save()
        {
            if (!string.IsNullOrEmpty(_folderName) && Path.Exists(_folderName))
                Properties.Settings.Default.TrainingDataPath = _folderName;

            Properties.Settings.Default.TrainingDataType = SelectedMNISTOption;
        }
    }
}
