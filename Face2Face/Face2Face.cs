using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face2Face
{
    [Transaction(TransactionMode.Manual)]
    public class Face2Face : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = commandData.Application.ActiveUIDocument.Document;
            //要求用户选择一个objekt，返回这个objekt的reference,这里限定了选面
            var faceReference = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face, "Select surface");
            //get到这个face对应的墙
            var wallofFace = doc.GetElement(faceReference) as Wall;
            // get到拾取到的面的本身，这个get方法返回的是geometric object,而我们上面选的也是object reference
            var face = wallofFace.GetGeometryObjectFromReference(faceReference) as Face;

            //想要指定一个墙类型，先应该把文档中所有墙类型找出来
            var wallTypes = from element in new FilteredElementCollector(doc).OfClass(typeof(WallType)) let type = element as WallType select type;

            var faceConfigWin = new FaceConfig(wallTypes.ToList());
            var result = faceConfigWin.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var tran = new Transaction(doc, "Create surface");
                tran.Start();
                //自己创建一个函数
                CreateFace(doc, face, wallofFace, faceConfigWin.SelectedWallType);
                tran.Commit();
                return Result.Succeeded;
            }
            return Result.Cancelled;

        }

        private void CreateFace(Document doc, Face face, Wall wallofFace, WallType selectedWallType)
        {
            var profile = new List<Curve>();
            //获取可能的洞口轮廓集成
            var openingArrays = new List<CurveArray>();
            //获取墙体宽度
            var width = selectedWallType.Width;
            //获取墙体和洞口的轮廓线,自创函数，ref传出参数
            ExtractFaceOutline(face, width, ref profile, ref openingArrays);
            //创建墙面
            var wall = Wall.Create(doc, profile, selectedWallType.Id, wallofFace.LevelId, false);
            //设置墙从标高0开始起来
            wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
            foreach (var item in openingArrays)
            {
                doc.Create.NewOpening(wall, item.get_Item(0).GetEndPoint(0), item.get_Item(1).GetEndPoint(1));
            }

        }

        private void ExtractFaceOutline(Face face, double width, ref List<Curve> profile, ref List<CurveArray> openingArrays)
        {
            var curveLoops = face.GetEdgesAsCurveLoops();
            var normal = (face as PlanarFace)?.FaceNormal;
            if (normal == null) throw new ArgumentException("Face2Face currently don't support curved surface");
            var translation = Transform.CreateTranslation(normal * width / 2);
            int i = 0;
            foreach (var curveloop in curveLoops.OrderByDescending(x => x.GetExactLength()))
            {
                curveloop.Transform(translation);
                var array = new CurveArray();
                foreach (var curve in curveloop)
                {
                    if (i == 0)
                        profile.Add(curve);
                    else array.Append(curve);
                }
                if (i != 0) openingArrays.Add(array);
                i++;
            }

        }


    }
}
