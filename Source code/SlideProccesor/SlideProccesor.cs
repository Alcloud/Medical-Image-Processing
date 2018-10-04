/*
 * @author Sebastian Lohmann
 */
namespace SlideProccesor {
	using System;
	using System.Drawing;
	using System.IO;
	using Glaukopis.SlideProcessing;
	internal class SlideProccesor {
		private static void Main(string[] args){
			foreach(var slideName in Util.GetSlideFilenames(args)){
				using(var slideCache=new SlideCache(slideName)){
					var scale=0.2f;
					var tileSize=200;
					var slidePartitionerFileName=slideCache.DataPath+"BudDetection.Tissue.xml";
					var slidePartitioner=File.Exists(slidePartitionerFileName)?SlidePartitioner<bool?>.Load(slidePartitionerFileName):new SlidePartitioner<bool?>(slideCache.Slide,scale,new Size(tileSize,tileSize));
					using(var overViewImage=slideCache.Slide.GetImagePart(0,0,slideCache.Slide.Size.Width,slideCache.Slide.Size.Height,slidePartitioner.Columns,slidePartitioner.Rows)){
						slideCache.SetImage("overview",overViewImage);//speichert unter C:\ProgramData\processingRepository\[slideCache.SlideName]\...
						//ggf. heatmap hier erstellen
					}
                    int i = 0;
                    int max = slidePartitioner.Count;
                    double percent = 0;
                    foreach (var tile in slidePartitioner.Values){
                        percent = 100.0 * i++ / max;
                        if (tile.Data.HasValue){
							Console.WriteLine(slideCache.SlideName+"-"+tile.Index+":"+tile.Data.Value+" skipped "+ percent+"%");
							continue;
						}
						using(var tileImage=slideCache.Slide.GetImagePart(tile)){
							if(false){
								var tileCache=slideCache.GetTileCache(tile.Index);
								tileCache.SetImage("rgb",tileImage);//speichert unter C:\ProgramData\processingRepository\[slideCache.SlideName]\[Index]\... ansonsten tileImage.Save("[uri].png")
							}
							var r=BudDetection.MyColorDeconvolution.Execute(tileImage);
							tile.Data=r.Value;//Wert zur Kachel speichern
						}
						Console.WriteLine(slideCache.SlideName+"-"+tile.Index+" done " + percent + "%");
						if(Console.KeyAvailable) break;
					}

                    slidePartitioner.Save(slidePartitionerFileName);
					using(var heatMap=slidePartitioner.GenerateHeatMap(b=>b.HasValue?(b.Value?Color.Green:Color.White):Color.Black)) slideCache.SetImage("tissueHeatMap",heatMap);
				}
			}
		}
	}
}