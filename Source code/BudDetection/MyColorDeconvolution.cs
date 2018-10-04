namespace BudDetection {
	using System.Drawing;
	using Glaukopis.SharpAccessoryIntegration;
	public static class MyColorDeconvolution{
		private const int RedThresholdMin=100;
		private const int DabThresholdMax=128;
		public static IResult<bool> Execute(Bitmap image){  // Lernstichprobe wird im Kachel-Gitter geteilt und als Image importiert
            var r = new Result<bool>();
            r.Value = false;
            using (var source=image.Clone() as Bitmap){
                using (var gsp = new SharpAccessory.Imaging.Filters.ColorDeconvolution()    //Grayscale prozessor wandelt das Bild in grayscale um
                    .Get2ndStain(source, SharpAccessory.Imaging.Filters.ColorDeconvolution.KnownStain.HaematoxylinDAB))
                {
                    int count = 0;
                    foreach (var grayscalePixel in gsp.Pixels())    // Gehen durch das Bild und prüfen jeder Pixel,
                    {
                        if(grayscalePixel.V > RedThresholdMin && DabThresholdMax > grayscalePixel.V)    //ob es diesen Kriterien entspricht
                        {
                            count++;
                            if (count > 20) //wenn es mehr als 20 "Objekte", die oben genannten Kriterien entsprechen, gefunden
                                            // wird die Kachel entsprechend markiert.
                            {
                                r.Value = true;
                                break;
                            }
                        }
                    }
                }
            }
            return r;
		}
	}
}
