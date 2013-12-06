using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;


namespace StuckSpotMapper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			String map = "ns2_tram";
			String version = "261";
			Bitmap overview = new Bitmap(1024,1024);
			MapExtract.Map level=null; 	
			List<Annotation> annotations; 
			//Print info
			Console.WriteLine (@"Annotation grapher
run from install root
Confused! for version 262
");

			if (args.Length == 2) {
				//read in the map and the version
				map = args [0];
				version = args [1];
			} else {
				Console.WriteLine ("Expected arguments: <mapname> <version>");
				return;
			}

			//load the minimap if we can.
			Console.WriteLine(" loading ns2/maps/overviews/" + map + ".tga");
		
			if (File.Exists ("ns2/maps/overviews/" + map + ".tga")) {
				//Load tga into bitmap;
				overview = Paloma.TargaImage.LoadTargaImage ("ns2/maps/overviews/" + map + ".tga");

				;
			} else {
				Console.WriteLine ("No Overview for map (" + map + ")");
				//return;
			}

			//parse the level for the size/origin.
			Console.WriteLine(" loading ns2/maps/" + map + ".level");
			if (File.Exists ("ns2/maps/" + map + ".level")) {
				level = new MapExtract.Map("ns2/maps/" + map + ".level");

			} else {
				Console.WriteLine("Cannot locate level file for ("+map+")");
				//return;
			}

			//load the level from hive.

			annotations = Web.GetSponitorData(
				String.Format ("http://sponitor2.herokuapp.com/api/get/annotations/{0}/{1}", map, version)
				);

			//draw the dots and all

			DrawDots(map, ref overview, level, annotations);

			Console.WriteLine(String.Format("Saving: Annotations-{0}-{1}.png", map, version));
			overview.Save(String.Format("Annotations-{0}-{1}.png", map, version)
			              ,System.Drawing.Imaging.ImageFormat.Png);

		}

		protected static void DrawDots (string map, ref Bitmap overview, MapExtract.Map lvl, List<Annotation> annotations)
		{
			minimap_extents minimap = null;
			if (lvl != null) {
				foreach (MapExtract.EntityInstance ent in lvl.Entities) {
					Console.WriteLine (ent.EntityName);
					if (ent.EntityName == "minimap_extents") {
						MapExtract.Types.VectorType origin = (MapExtract.Types.VectorType)ent.Fields ["origin"];
						MapExtract.Types.VectorType scale = (MapExtract.Types.VectorType)ent.Fields ["scale"];
						minimap = new minimap_extents (origin.X, origin.Y, origin.Z, scale.X, scale.Y, scale.Z);
					}
				}
			}
			if (minimap == null) {
				Console.WriteLine ("Level File Cannot be read checking fallback text file");
				float[] data = ReadMinimapFile(map);
				minimap = new minimap_extents(data[0],data[1],data[2],data[3],data[4],data[5]);


			}

			Console.WriteLine("minimap scaling:"+ minimap.ToString());

			Graphics g = Graphics.FromImage (overview);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			g.TranslateTransform (overview.Width / 2f, overview.Height / 2f);
			g.RotateTransform (-90f);

		
			//aahh
			if (Math.Abs (minimap.ScaleX - minimap.ScaleZ) > 10) {
				if (minimap.ScaleZ > minimap.ScaleX) {
					g.ScaleTransform (minimap.ScaleX / minimap.ScaleZ, 1);
				} else {
					g.ScaleTransform (1, minimap.ScaleZ / minimap.ScaleX);
				}
			}

			g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

			Brush b =  new SolidBrush(Color.FromArgb (128, 255, 1, 1));
			float[] vec;
			foreach (Annotation a in annotations) {			
				vec = minimap.CalculateOffset(a.x,a.y,a.z, overview.Width);
				//Console.WriteLine(a.ToString() + vec[0]+ " " + vec[2]);
				g.FillRectangle(b,vec[0] -2, vec[2]-2, 4,4);
			}

			g.Flush();
		}

		protected static float[] ReadMinimapFile(string map){
			float[] output = new float[]{0f,0f,0f,500f,500f,500f};

			if(File.Exists("levels.txt")){
				foreach(String s in File.ReadAllLines("levels.txt")){
					if(s.StartsWith (map)){
						string[] data = s.Split(';');
						for(int i = 1; i < data.Length;i++){
							float.TryParse(data[i], out output[i-1]);
						}
					}
				}
			}
			else{
				Console.WriteLine ("levels.txt not found using defaults. zoom will be wrong on dots");

			}
			return output;

		}
	}
	public class Annotation{
		public float x;
		public float y;
		public float z;
		public string text;

		public Annotation (float X, float Y, float Z, string message)
		{
			this.x = X;
			this.y = Y;
			this.z = Z;
			this.text = message;
		}
		public override string  ToString(){
			return String.Format("({0},{1},{2}){3}",x,y,z,text);
		}
	
	}

	public class minimap_extents{
		public float OriginX =0;
		public float OriginY =0;
		public float OriginZ =0;
		public float ScaleX =0;
		public float ScaleY =0;
		public float ScaleZ =0;

		public minimap_extents(float ox,float oy,float oz,float sx,float sy,float sz){
			this.OriginX = ox;
			this.OriginY = oy;
			this.OriginZ = oz;
			this.ScaleX = sx;
			this.ScaleY = sy;
			this.ScaleZ = sz;

		}

		public float[] CalculateOffset(float x , float y, float z, int size){

			float _x = x;
			float _y = y;
			float _z = z;

			_x -= this.OriginX;
			_y -= this.OriginY;
			_z -= this.OriginZ;

			float scaleX = (float) size/(this.ScaleX/2);
			float scaleY = (float) size/(this.ScaleY/2) ;
			float scaleZ = (float) size/(this.ScaleZ/2) ;

			_x = _x * scaleX;
			_y = _y * scaleY;
			_z = _z * scaleZ;



			return new float[]{_x,_y,_z};
		}

		public override string  ToString(){
			return String.Format("({0},{1},{2})[{3}, {4},{5}]",OriginX,OriginY,OriginZ,ScaleX,ScaleY,ScaleZ);
		}

	}


}
