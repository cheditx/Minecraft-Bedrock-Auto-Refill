using System.Runtime.InteropServices;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace AutoRefill;

public abstract class AutoRefill {
    
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);
    
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);
    
    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [StructLayout(LayoutKind.Sequential)]
    private struct Point {
        
        public int X;
        public int Y;
    }

    private static readonly int ScreenWidth = GetSystemMetrics(0);
    private static readonly int ScreenHeight = GetSystemMetrics(1);

    private static Point GetCursorPosition() {
        
        GetCursorPos(out var point);
        return new Point { X = point.X * 65535 / ScreenWidth, Y = point.Y * 65535 / ScreenHeight };
    }
    
    private static string GetActiveWindowTitle() {
        
        const int nChars = 256;
        
        var buff = new StringBuilder(nChars);
        var handle = GetForegroundWindow();

        return GetWindowText(handle, buff, nChars) > 0 ? buff.ToString() : "";
    }
    
    private static readonly InputSimulator InputSimulator = new();

    private static void Main() {

        while (true) {

            var activeWindowTitle = GetActiveWindowTitle();
            if (!activeWindowTitle.Contains("Minecraft")) {
                continue;
            }
            
            if (!InputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_E)) {
                continue;
            }
            
            Thread.Sleep(25); // Use 25 only if you use flarial with fast inventory mod, otherwise use 250.
            InputSimulator.Keyboard.KeyDown(VirtualKeyCode.LSHIFT);
            
            var x = 23615;
            var y = 35441;

            var row = 0;
            while (row < 3) {
                
                for (var i = 0; i < 9; i++) {
                    
                    InputSimulator.Mouse.MoveMouseTo(x, y);
                    InputSimulator.Mouse.LeftButtonClick();
                    
                    x += 2304;
                    
                    if (i == 8) {
                        y += 3800;
                        x = 23615;
                        row++;
                    }
                    
                    Thread.Sleep(6);
                }
            }

            InputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_E);
        }
    }
}