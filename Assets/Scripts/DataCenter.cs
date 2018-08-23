using UnityEngine;
using System.IO;
using System.Collections;

public class DataCenter  {

	public struct connect3Data  {
		public int lvlnumber;
		public int wins;
		public int loses;
		public int score;
		public int record;
	}

	public struct gridData  {
		public int score;
		public int record;
		public int complexity;
		public int successfulAttempts;
		public int failedAttempts;
	}

	public static connect3Data classic;
	public static connect3Data timeattack;
	public static gridData grid;

	public static int overallscore;

	public static void LoadData()  {
		if(!File.Exists(Application.persistentDataPath+"/data"))  {			//if there is no data file, create it with default values
			SaveData();
			return;
		}
		else  {
			StreamReader reader = new StreamReader(File.OpenRead(Application.persistentDataPath+"/data"));
			reader.ReadLine();
			string data = reader.ReadLine();
			reader.Close();

			string[] values = data.Split('-');
			classic.lvlnumber = System.Convert.ToInt32(values[0]);
			classic.wins = System.Convert.ToInt32(values[1]);
			classic.loses = System.Convert.ToInt32(values[2]);
			classic.score = System.Convert.ToInt32(values[3]);
			classic.record = System.Convert.ToInt32(values[4]);
			timeattack.lvlnumber = System.Convert.ToInt32(values[5]);
			timeattack.wins = System.Convert.ToInt32(values[6]);
			timeattack.loses = System.Convert.ToInt32(values[7]);
			timeattack.score = System.Convert.ToInt32(values[8]);
			timeattack.record = System.Convert.ToInt32(values[9]);
			grid.score = System.Convert.ToInt32(values[10]);
			grid.record = System.Convert.ToInt32(values[11]);
			grid.complexity = System.Convert.ToInt32(values[12]);
			grid.successfulAttempts = System.Convert.ToInt32(values[13]);
			grid.failedAttempts = System.Convert.ToInt32(values[14]);
			overallscore = System.Convert.ToInt32(values[15]);
		}
	}

	public static void SaveData()  {
		StreamWriter writer = new StreamWriter(File.Create(Application.persistentDataPath+"/data"));
		writer.WriteLine("//DO NOT DELETE THESE LINES!");
		writer.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}-{10}-{11}-{12}-{13}-{14}-{15}", classic.lvlnumber, classic.wins, classic.loses, classic.score, classic.record, timeattack.lvlnumber, timeattack.wins, timeattack.loses, timeattack.score, timeattack.record, grid.score, grid.record, grid.complexity, grid.successfulAttempts, grid.failedAttempts, overallscore);
		writer.Close();
	}

}