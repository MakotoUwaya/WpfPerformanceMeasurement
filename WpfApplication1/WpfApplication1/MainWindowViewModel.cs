using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;

namespace WpfApplication1
{
    public class MainWindowViewModel : BindableBase
    {
        private string totalMemory;

        public string TotalMemory
        {
            get { return this.totalMemory; }
            set { this.SetProperty(ref this.totalMemory, value); }
        }

        private string repeatCount;

        public string RepeatCount
        {
            get { return this.repeatCount; }
            set
            {
                int result;
                if (!int.TryParse(value, out result))
                {
                    return; 
                };
                this.SetProperty(ref this.repeatCount, value);
            }
        }

        private ObservableCollection<ListItem> listItemData;
        public ObservableCollection<ListItem> ListItemData
        {
            get { return this.listItemData; }
            set { this.SetProperty(ref this.listItemData, value); }
        }

        private ICommand doWork;
        public ICommand DoWork
        {
            get { return this.doWork; }
            set { this.SetProperty(ref this.doWork, value); }
        }

        private ICommand niceWork;
        public ICommand NiceWork
        {
            get { return this.niceWork; }
            set { this.SetProperty(ref this.niceWork, value); }
        }

        public MainWindowViewModel()
        {
            this.TotalMemory = "";
            this.repeatCount = "10";
            this.ListItemData = new ObservableCollection<ListItem>();
            this.NiceWork = new DelegateCommand(this.Nice);
            this.DoWork = new DelegateCommand(this.Work);
        }

        private void Work()
        {
            var repeatCount = int.Parse(this.RepeatCount);
            for (var i = 0; i < repeatCount; i++)
            {
                var thread = new Thread(this.CreateBitmap)
                {
                    Name = $"Bitmap worker: {i}",
                    IsBackground = true,
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                this.TotalMemory = $"TotalMemory = {GC.GetTotalMemory(true) / 1024:#,0} KB";
                this.ListItemData.Insert(0, new ListItem { Name = this.TotalMemory });
            }
        }

        private void CreateBitmap()
        {
            var border = new Border { Background = Brushes.Red };
            var text = new TextBlock { Text = "hoge" };

            border.Child = text;
            border.Arrange(new System.Windows.Rect(0, 0, 200, 200));

            var bitmap = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Default);
            bitmap.Render(border);
            bitmap.Freeze();
        }

        private void Nice()
        {
            var repeatCount = int.Parse(this.RepeatCount);
            for (var i = 0; i < repeatCount; i++)
            {
                var thread = new Thread(this.NiceCreateBitmap)
                {
                    Name = $"Bitmap worker: {i}",
                    IsBackground = true,
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                this.TotalMemory = $"TotalMemory = {GC.GetTotalMemory(true) / 1024:#,0} KB";
                this.ListItemData.Insert(0, new ListItem { Name = this.TotalMemory });
            }
        }

        private void NiceCreateBitmap()
        {
            var border = new Border { Background = Brushes.Red };
            var text = new TextBlock { Text = "hoge" };

            border.Child = text;
            border.Arrange(new System.Windows.Rect(0, 0, 200, 200));

            var bitmap = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Default);
            bitmap.Render(border);
            bitmap.Freeze();

            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
            Dispatcher.Run();
        }
    }
}
