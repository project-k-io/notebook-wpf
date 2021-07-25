using System.Text.Json.Serialization;
using System.Windows;

namespace ProjectK.Notebook.WinApp.Models
{
    public class WindowSettingsModel
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WindowState WindowState { get; set; }

        public double Top { get; set; } //                   " value="100" />
        public double Left { get; set; } //                  " value="100" />
        public double Width { get; set; } // " value="800" />
        public double Height { get; set; } // " value="600" />

        public void SizeToFit()
        {
            if (Height > SystemParameters.VirtualScreenHeight) Height = SystemParameters.VirtualScreenHeight;

            if (Width > SystemParameters.VirtualScreenWidth) Width = SystemParameters.VirtualScreenWidth;
        }

        public void MoveIntoView()
        {
            if (Top + Height / 2 >
                SystemParameters.VirtualScreenHeight)
                Top =
                    SystemParameters.VirtualScreenHeight -
                    Height;

            if (Left + Width / 2 >
                SystemParameters.VirtualScreenWidth)
                Left =
                    SystemParameters.VirtualScreenWidth -
                    Width;

            if (Top < 0) Top = 0;

            if (Left < 0) Left = 0;
        }
    }
}