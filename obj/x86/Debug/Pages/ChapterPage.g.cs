﻿#pragma checksum "C:\Users\Mantis\Documents\Visual Studio 2017\Projects\LMS_Project\LMS_Project\Pages\ChapterPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9FE07DE2F626FCD38193226CBF17762C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LMS_Project.Pages
{
    partial class ChapterPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Grid element1 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 11 "..\..\..\Pages\ChapterPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)element1).Loading += this.Grid_Loading;
                    #line default
                }
                break;
            case 2:
                {
                    this.LoadingIndicator = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 3:
                {
                    this.wpContent = (global::Windows.UI.Xaml.Controls.WebView)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

