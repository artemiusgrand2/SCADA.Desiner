﻿#pragma checksum "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F8D6A4378713F64AF76F05CC00F5BA7B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SCADA.Desiner.WorkForms {
    
    
    /// <summary>
    /// SetAngle
    /// </summary>
    public partial class SetAngle : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox text_box_angle;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_OK;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Cancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SCADA.Desiner;component/windowwork/workforms/setangle.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
            ((SCADA.Desiner.WorkForms.SetAngle)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.text_box_angle = ((System.Windows.Controls.TextBox)(target));
            
            #line 10 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
            this.text_box_angle.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.text_box_angle_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.button_OK = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
            this.button_OK.Click += new System.Windows.RoutedEventHandler(this.button_OK_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.button_Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\..\..\WindowWork\WorkForms\SetAngle.xaml"
            this.button_Cancel.Click += new System.Windows.RoutedEventHandler(this.button_Cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

