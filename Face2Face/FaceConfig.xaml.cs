using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Face2Face
{
    /// <summary>
    /// FaceConfig.xaml 的交互逻辑
    /// </summary>
    public partial class FaceConfig : Window
    {
        //传出去的值,返回值
        public WallType SelectedWallType { get; private set; }
        //传进来的从文档中找到的所有walltype
        public FaceConfig(List<WallType> wallTypes)
        {
            InitializeComponent();
            //把walltype作为数据源赋给combobox
            var vm = new ViewModel();
            wallTypes.ForEach(x => vm.wallTypes.Add(x));
            DataContext = vm;
        }

        private void cbWallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var wallType = e.AddedItems?.Count > 0 ? (e.AddedItems[0] as WallType) : null;
            if (wallType != null)
                SelectedWallType = wallType;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWallType != null)
                DialogResult = true;
            else
            {
                MessageBox.Show("Choose the WallType");
                DialogResult = false;
            }
        }

    }

    //MVVM 
    public class ViewModel
    {
        public ObservableCollection<WallType> wallTypes { get; } = new ObservableCollection<WallType>();

    }
}
