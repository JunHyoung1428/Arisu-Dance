using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Dancing_Arisu
{
    public partial class MainWindow : Window
    {
        private int lastHit;
        private int comboCount;
        private DispatcherTimer comboTimer;
        private Dictionary<int, BitmapImage> imageCache;

        private float RESETTIME = 5.0f;

        public MainWindow()
        {
            InitializeComponent();
            Hook.KeyboardHook.KeyDown += KeyboardHook_KeyDown;
            Hook.KeyboardHook.KeyUp += KeyboardHook_KeyUp;
            Hook.KeyboardHook.HookStart();

            InitializeImageCache();

            comboTimer = new DispatcherTimer();
            comboTimer.Interval = TimeSpan.FromSeconds(RESETTIME);
            comboTimer.Tick += ComboTimer_Tick;
        }

        private void InitializeImageCache()
        {
            imageCache = new Dictionary<int, BitmapImage>();
            for (int i = 0; i <= 9; i++)
            {
                string imagePath = $"pack://application:,,,/Resources/{i}.png";
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                imageCache[i] = bitmap;
            }
        }

        ~MainWindow()
        {
            Hook.KeyboardHook.HookEnd();
        }

        private bool KeyboardHook_KeyDown(int vkCode)
        {
            if (lastHit == vkCode) //동일키 반복 입력 방지
            {
                return true;
            }
            lastHit = vkCode;
            ComboUp();
            return true;
        }

        private bool KeyboardHook_KeyUp(int vkCode)
        {
            if (lastHit == vkCode)
            {
                lastHit = -1;
            }
            return true;
        }

        private void ComboUp()
        {
            if (ComboPanel.Visibility == Visibility.Collapsed)
                return;

            comboTimer.Stop();
            comboCount++;
            UpdateComboImage(comboCount);
            comboTimer.Start();
        }

        private void UpdateComboImage(int count)
        {
            var digits = GetDigits(count);
            var imageSources = new List<BitmapImage>();

            foreach (var digit in digits)
            {
                if (imageCache.ContainsKey(digit))
                {
                    imageSources.Add(imageCache[digit]);
                }
            }
            ComboImages.ItemsSource = imageSources;
        }
        private IEnumerable<int> GetDigits(int number)
        {
            var digits = new Stack<int>();
            while (number > 0)
            {
                digits.Push(number % 10);
                number /= 10;
            }

            if (digits.Count == 0)
            {
                digits.Push(0);
            }

            return digits;
        }

        private void ComboTimer_Tick(object sender, EventArgs e)
        {
            InitCombo();
        }

        private void InitCombo()
        {
            comboTimer.Stop();
            comboCount = 0;
            UpdateComboImage(comboCount);
        }

        private void MainImage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InitCombo();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            else
            {
                ToggleCombo();
            }
        }

        private void ToggleCombo()
        {
            if (ComboPanel.Visibility == Visibility.Visible)
            {
                ComboPanel.Visibility = Visibility.Collapsed;
                InitCombo();
            }
            else
            {
                ComboPanel.Visibility = Visibility.Visible;
            }
        }
    }
}