using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LMS_Project.Pages
{
    public sealed partial class Waiting : ContentDialog
    {
        private string title;

        private int _max = 0;
        public int max { get { return _max; } set { _max = value; Progress.Maximum = value; } }
        private int _value = 0;
        public int value
        {
            get { return _value; }
            set
            {
                this._value = value;
                Progress.Value = value;
            }
        }

        public bool _showPregress = false;
        public bool showPregress
        {
            get { return _showPregress; }
            set
            {
                _showPregress = value;
                if (value)
                    Progress.Visibility = Visibility.Visible;
                else
                    Progress.Visibility = Visibility.Collapsed;
            }
        }

<<<<<<< HEAD




=======
>>>>>>> origin/master
        public Waiting(string title = "Waitting")
        {
            this.title = title;
            this.InitializeComponent();
            this.Title = this.title;
            Progress.Maximum = max;
            Progress.Value = value;
            if (showPregress)
                Progress.Visibility = Visibility.Visible;
            else
                Progress.Visibility = Visibility.Collapsed;
        }

    }
}
