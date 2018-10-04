/*
 * @author Sebastian Lohmann
 */
namespace BudDetection {
	using System;
	using System.Drawing;
	using Glaukopis.SharpAccessoryIntegration;
	using SharpAccessory.Imaging.Processors;
	using SharpAccessory.Imaging.Segmentation;
    using SharpAccessory.Imaging.Classification.Features.Size;

    public static class ColorDeconvolution{
		private const int RedThreshold=128;
		private const int DabThreshold=128;
		public static IResult<bool> Execute(Bitmap image){
			var r=new Result<bool>();
			using(var source=image.Clone() as Bitmap){
				var gspR=new GrayscaleProcessor(source.Clone() as Bitmap,RgbToGrayscaleConversion.JustReds);
				gspR.Dispose();
				r.DebugBitmaps.Add(Tuple.Create(gspR.Bitmap,"Red"));
				r.DebugLayers.Add(createLayer(gspR.Bitmap,RedThreshold,"Red"));
				r.DebugVariables.Add("RedThreshold",RedThreshold);
				var gpDAB=new SharpAccessory.Imaging.Filters.ColorDeconvolution().Get2ndStain(source,SharpAccessory.Imaging.Filters.ColorDeconvolution.KnownStain.HaematoxylinDAB);
				gpDAB.Dispose();
				r.DebugBitmaps.Add(Tuple.Create(gpDAB.Bitmap,"DAB"));
				var dabLayer=createLayer(gpDAB.Bitmap,DabThreshold,"DAB");
				r.DebugLayers.Add(dabLayer);
				r.DebugVariables.Add("DabThreshold",DabThreshold);
				r.Value=0<dabLayer.Objects.Count;
			}
			return r;
		}
        public static IResult<ObjectLayer> Detect(Bitmap image)
        {
            var result = new Result<ObjectLayer>();

            using (var source = image.Clone() as Bitmap)
            {
                var gpDAB = new SharpAccessory.Imaging.Filters.ColorDeconvolution().Get2ndStain(source, SharpAccessory.Imaging.Filters.ColorDeconvolution.KnownStain.HaematoxylinDAB);
                gpDAB.Dispose();
                result.DebugBitmaps.Add(Tuple.Create(gpDAB.Bitmap, "DAB"));
                var dabLayer = createLayer(gpDAB.Bitmap, DabThreshold, "DAB");
                result.DebugLayers.Add(dabLayer);
                result.DebugVariables.Add("DabThreshold", DabThreshold);
                ObjectPixels.ProcessLayer(dabLayer);
                var layer = dabLayer.CreateAbove((ImageObject io)=> io.Features["ObjectPixels"].Value < 100);
                layer.Name = "Bud Results";
                result.DebugLayers.Add(layer);
                result.Value = layer;
            }
            return result;
        }
        private static ObjectLayer createLayer(Bitmap bitmap,int threshold,string name){
			var map=new Map(bitmap.Width,bitmap.Height);
			using(var gsp=new GrayscaleProcessor(bitmap,RgbToGrayscaleConversion.JustReds)){
				gsp.WriteBack=false;
				foreach(var grayscalePixel in gsp.Pixels()) map[grayscalePixel.X,grayscalePixel.Y]=grayscalePixel.V>threshold?1u:0u;
			}
			var layer=new ConnectedComponentCollector().Execute(map);
			layer.Name=name;
			return layer;
		}
	}
}
