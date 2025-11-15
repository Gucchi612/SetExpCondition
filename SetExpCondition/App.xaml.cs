using Microsoft.Maui;
using Microsoft.Maui.Controls;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
#endif

namespace SetExpCondition;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

#if WINDOWS
        // Windows の場合のみ、ウィンドウサイズを画面解像度に合わせて柔軟に設定する
        Microsoft.Maui.Controls.Application.Current.Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(200); // ウィンドウ生成が完了するまで少し待つ

            var window = App.Current.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (window is null) return;

            var hwnd = WindowNative.GetWindowHandle(window);
            var winId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(winId);
            if (appWindow is null) return;

            // 表示領域（作業領域）を取得
            var displayArea = DisplayArea.GetFromWindowId(winId, DisplayAreaFallback.Primary);
            var work = displayArea.WorkArea; // PointInt32/SizeInt32

            // 画面に対する割合でサイズ決定（例: 80% 幅、70% 高さ）、かつ最小・最大を設定
            const double widthRatio = 0.4;
            const double heightRatio = 0.72;
            const int minWidth = 1000;
            const int minHeight = 600;
            const int maxWidth = 1600; // 必要なら制限
            const int maxHeight = 1000;

            var targetWidth = (int)(work.Width * widthRatio);
            var targetHeight = (int)(work.Height * heightRatio);

            if (targetWidth < minWidth) targetWidth = minWidth;
            if (targetHeight < minHeight) targetHeight = minHeight;
            if (targetWidth > work.Width) targetWidth = work.Width;
            if (targetHeight > work.Height) targetHeight = work.Height;
            if (targetWidth > maxWidth) targetWidth = maxWidth;
            if (targetHeight > maxHeight) targetHeight = maxHeight;

            appWindow.Resize(new SizeInt32(targetWidth, targetHeight));

            // ウィンドウを作業領域の中央に配置
            var centerX = work.X + (work.Width - targetWidth) / 2;
            var centerY = work.Y + (work.Height - targetHeight) / 2;
            appWindow.Move(new PointInt32(centerX, centerY));
        });
#endif
    }
}
