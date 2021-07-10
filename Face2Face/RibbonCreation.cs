using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Face2Face
{
    class RibbonCreation : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var tabName = "Tab";
            var panelName = "Smalle_tools";
            application.CreateRibbonTab(tabName);
            var panel = application.CreateRibbonPanel(tabName, panelName);
            //获得面生面类程序集的存储位置和完整类名
            var assemblyType = new Face2Face().GetType();
            var location = assemblyType.Assembly.Location;
            var className = assemblyType.FullName;
            //信息存入data从而可以通过点击按钮执行面生面类
            var pushButtonData = new PushButtonData("tool", "Face2Face", location, className);
            //按钮图标
            //反射技术，获得现在运行的文件集ElementCUB的路径location，然后找到它的目录Directory
            var imageSource = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Image\Setting_Pic.png";
            pushButtonData.LargeImage = new BitmapImage(new Uri(imageSource));
            //把pushbutton添加到panel上
            var pushButton = panel.AddItem(pushButtonData) as PushButton;

            //分隔符
            panel.AddSeparator();

            return Result.Succeeded;
        }
    }
    
}
