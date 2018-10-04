/*
 * @author Sebastian Lohmann
 */
namespace BudDetection {
	using Glaukopis.SharpAccessoryIntegration;
	using SharpAccessory.Imaging.Classification.Features.Size;
	using SharpAccessory.Imaging.Classification.Features.Shape;
	using SharpAccessory.Imaging.Segmentation;

	public static class AxesOfCorrespondingEllipse {
		public static void ProcessLayer(ObjectLayer layer){
			foreach(var io in layer.Objects){
				var correspondingEllipse=CorrespondingEllipse.FromContour(io.Contour);
				io.SetFeature("MajorAxisOfCorrespondingEllipse", new MajorAxisOfCorrespondingEllipse(correspondingEllipse).Value);
				io.SetFeature("MinorAxisOfCorrespondingEllipse", new MinorAxisOfCorrespondingEllipse(correspondingEllipse).Value);

                ConvexHull ch = ConvexHull.FromContour(io.Contour);
                ConvexHullProperties chp = ConvexHullProperties.FromConvexHull(ch);
                ContourProperties cp = ContourProperties.FromContour(io.Contour);
                AreaOfContour ac = new AreaOfContour(cp);
                AreaOfConvexHull ach = new AreaOfConvexHull(chp);
				io.SetFeature("FormFactor", cp.FormFactor);
				io.SetFeature("Area", cp.Area);
				io.SetFeature("Convexity", cp.Convexity);
                io.SetFeature("AreaHull", ach.Value);
                io.SetFeature("AreaDiv", ach.Value/cp.Area);
                io.SetFeature("FormFactorOfConvexHull", new FormFactorOfConvexHull(chp).Value);
			}
		}
	}
}
