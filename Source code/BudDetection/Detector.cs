namespace BudDetection {
	using System.Drawing;
	using Glaukopis.SharpAccessoryIntegration;
	using SharpAccessory.Imaging.Classification.Features.Size;
	using SharpAccessory.Imaging.Segmentation;

	public static class Detector {
		public static IResult<ObjectLayer> Execute(Bitmap bitmap){
			var r = new Result<ObjectLayer>();
			var layer = createLayer(bitmap, 40, "first layer"); // Ein neues Layer wird gemacht
            ObjectLayer layer2 = null;                          // Die Kachel wird in grayscale umgewandelt

			ObjectPixels.ProcessLayer(layer);
			AxesOfCorrespondingEllipse.ProcessLayer(layer);     // Gefundene Buds werden als Objecte mit Features bezeichnet
			r.DebugLayers.Add(layer);
			layer2=layer.CreateAbove((ImageObject io)=>         // Wenn ein Element den Parametern entspricht, wird er markiert 
                io.Features["Area"].Value < 5000 &&             // und als Object auf neues Layer gespeichert
                io.Features["Area"].Value > 500 && 
                io.Features["AreaDiv"].Value<1.55 //&& 
                //io.Features["FormFactor"].Value<30
            );
            layer2.Name = "BUDs";
            r.DebugLayers.Add(layer2);

            r.Value = layer2;

            return r;
		}
		private static ObjectLayer createLayer(Bitmap bitmap, int threshold, string name){
			var map=new Map(bitmap.Width,bitmap.Height);
			using(var gsp = new SharpAccessory.Imaging.Filters.ColorDeconvolution()
                .Get2ndStain(bitmap,SharpAccessory.Imaging.Filters.ColorDeconvolution.KnownStain.HaematoxylinDAB)){
				gsp.WriteBack = false;
				foreach(var grayscalePixel in gsp.Pixels())
                    map[grayscalePixel.X, grayscalePixel.Y] = grayscalePixel.V > threshold?1u:0u;   //Das Bild wird als 0 und 1 dargestellt
			}
			var layer = new ConnectedComponentCollector().Execute(map);
			layer.Name = name;
			return layer;
		}
	}
}
