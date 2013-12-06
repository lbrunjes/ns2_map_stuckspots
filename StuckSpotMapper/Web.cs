using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace StuckSpotMapper
{
	public class Web
	{
		public static List<Annotation> GetSponitorData(string url){
			Regex getPointAndLabel = new Regex(@"""x"":(-*\d+\.\d+),""y"":(-*\d+\.\d+),""z"":(-*\d+\.\d+),""message"":""(.+?)""");
			List<Annotation> annotations = new List<Annotation>();

			String data = getData(url);
			MatchCollection mc  = getPointAndLabel.Matches(data);



			foreach(Match m in mc){

				annotations.Add(
					new Annotation(
					float.Parse(m.Groups[1].Value),
					float.Parse(m.Groups[2].Value),
					float.Parse(m.Groups[3].Value),
					m.Groups[4].Value)
					);
			}
			return annotations;
		}
		protected static string getData (string url)
		{
			try{
				HttpWebRequest req =  (HttpWebRequest)WebRequest.Create(url);
				req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.95 Safari/537.36";
				HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
				StreamReader read = new StreamReader( resp.GetResponseStream());
				String data = read.ReadToEnd();

				resp.Close();
				read.Close();
				resp =null;
				req=null;
				read =null;
				return data;
			}
			catch(Exception ex){
				Console.WriteLine (url +" "+ ex.Message);

			}
			return "";
		}
	}
}

