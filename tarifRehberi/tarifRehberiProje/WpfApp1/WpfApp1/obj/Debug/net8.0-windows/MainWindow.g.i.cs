﻿#pragma checksum "..\..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "08A1BD363477235581B3CB5D6D07000ED64AD1CB"
//------------------------------------------------------------------------------
// <auto-generated>
//     Bu kod araç tarafından oluşturuldu.
//     Çalışma Zamanı Sürümü:4.0.30319.42000
//
//     Bu dosyada yapılacak değişiklikler yanlış davranışa neden olabilir ve
//     kod yeniden oluşturulursa kaybolur.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace TarifRehberi {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 65 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SearchBox;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox FilterCategory;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FilterTime;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FilterCost;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox SortOption;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid RecipeList;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl IngredientList;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid SuggestedRecipeList;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WpfApp1;V1.0.0.0;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SearchBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 66 "..\..\..\MainWindow.xaml"
            this.SearchBox.GotFocus += new System.Windows.RoutedEventHandler(this.SearchBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 66 "..\..\..\MainWindow.xaml"
            this.SearchBox.LostFocus += new System.Windows.RoutedEventHandler(this.SearchBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.FilterCategory = ((System.Windows.Controls.ComboBox)(target));
            
            #line 67 "..\..\..\MainWindow.xaml"
            this.FilterCategory.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnFilterChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.FilterTime = ((System.Windows.Controls.TextBox)(target));
            
            #line 69 "..\..\..\MainWindow.xaml"
            this.FilterTime.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.OnFilterChanged);
            
            #line default
            #line hidden
            
            #line 71 "..\..\..\MainWindow.xaml"
            this.FilterTime.GotFocus += new System.Windows.RoutedEventHandler(this.FilterTime_GotFocus);
            
            #line default
            #line hidden
            
            #line 71 "..\..\..\MainWindow.xaml"
            this.FilterTime.LostFocus += new System.Windows.RoutedEventHandler(this.FilterTime_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.FilterCost = ((System.Windows.Controls.TextBox)(target));
            
            #line 73 "..\..\..\MainWindow.xaml"
            this.FilterCost.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.OnFilterChanged);
            
            #line default
            #line hidden
            
            #line 75 "..\..\..\MainWindow.xaml"
            this.FilterCost.GotFocus += new System.Windows.RoutedEventHandler(this.FilterCost_GotFocus);
            
            #line default
            #line hidden
            
            #line 75 "..\..\..\MainWindow.xaml"
            this.FilterCost.LostFocus += new System.Windows.RoutedEventHandler(this.FilterCost_LostFocus);
            
            #line default
            #line hidden
            return;
            case 5:
            this.SortOption = ((System.Windows.Controls.ComboBox)(target));
            
            #line 76 "..\..\..\MainWindow.xaml"
            this.SortOption.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnFilterChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 83 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnSearchClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.RecipeList = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 8:
            this.IngredientList = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 9:
            
            #line 101 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnSuggestRecipeClick);
            
            #line default
            #line hidden
            return;
            case 10:
            this.SuggestedRecipeList = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 11:
            
            #line 122 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnAddRecipeClick);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 123 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnUpdateRecipeClick);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 124 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnDeleteRecipeClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

