using System.Collections.Generic;
using System.IO;
using System.Text;

namespace S3DPAK_Build
{
	/* pak file types
	 * x3 text
	 * xE strings
	 * x12 gfx
	 * */

	public class S3DPAK
	{
		private const string NoName = "_noname_";

		public List<S3DFile> Files { get; set; }

		public string SourcePath { get; set; }

		public void LoadPak(string path)
		{
			SourcePath = path;
			using (FileStream fs = new FileStream(SourcePath, FileMode.Open))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					int filecount = br.ReadInt32();

					Files = new List<S3DFile>();

					for (int i = 0; i < filecount; i++)
					{
						int off = br.ReadInt32();
						int siz = br.ReadInt32();
						int namelen = br.ReadInt32();
						byte[] n = br.ReadBytes(namelen);
						string nam = Encoding.ASCII.GetString(n);
						int typ = br.ReadInt32();
						int u1 = br.ReadInt32();
						int u2 = br.ReadInt32();

						Files.Add(new S3DFile()
						{
							Offset = off,
							Size = siz,
							Name = nam,
							Type = typ,
							Unk1 = u1,
							Unk2 = u2
						});
					}
				}
			}
		}

		public void DumpAllFiles(string path)
		{
			using (FileStream fs = new FileStream(SourcePath, FileMode.Open))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					using (StringWriter sw = new StringWriter())
					{
						foreach (S3DFile p in Files)
						{
							string runname = p.Name;

							if (string.IsNullOrEmpty(runname))
								runname = NoName + p.Offset;

							sw.WriteLine("{0:X2},{1:X8},{2:X8},{3}", p.Type, p.Unk1, p.Unk2, runname);
							fs.Position = p.Offset;
							string filec = Path.Combine(path, runname);
							File.WriteAllBytes(filec, br.ReadBytes(p.Size));
						}

						string pathc = Path.Combine(path, "_s3d_dump.txt");
						File.WriteAllText(pathc, sw.ToString());
					}
				}
				
			}
			
		}

		public static void BuildPak(string index, string output)
		{
			string dir = Path.GetDirectoryName(index);
			string[] entries = File.ReadAllLines(index);

			List<S3DFile> ff = new List<S3DFile>();

			long datastart = 4;//file count

			for (int i = 0; i < entries.Length; i++)
			{
				string[] aa = entries[i].Split(',');

				int t = int.Parse(aa[0], System.Globalization.NumberStyles.HexNumber);
				int u1 = int.Parse(aa[1], System.Globalization.NumberStyles.HexNumber);
				int u2 = int.Parse(aa[2], System.Globalization.NumberStyles.HexNumber);

				string filec = Path.Combine(dir, aa[3]);
				var inf = new FileInfo(filec);

				ff.Add(new S3DFile()
				{
					Size = (int)inf.Length,
					Name = aa[3],
					Type = t,
					Unk1 = u1,
					Unk2 = u2,
				});

				datastart += 0x18 + aa[3].Length;
			}

			using (FileStream fs = new FileStream(output, FileMode.Create))
			{
				using (BinaryWriter bw = new BinaryWriter(fs))
				{
					bw.Write(ff.Count);

					long runningtable = fs.Position;
					long runningdata = datastart;

					foreach (S3DFile pf in ff)
					{
						fs.Position = runningtable;

						bw.Write((int)runningdata);
						bw.Write(pf.Size);

						if (pf.Name.Contains(NoName))
						{
							bw.Write((int)0);
						}
						else
						{
							bw.Write(pf.Name.Length);
							foreach (char c in pf.Name)
								bw.Write((byte)c);
						}

						bw.Write(pf.Type);

						bw.Write(pf.Unk1);
						bw.Write(pf.Unk2);

						runningtable = fs.Position;

						fs.Position = runningdata;

						string filec = Path.Combine(dir, pf.Name);
						bw.Write(File.ReadAllBytes(filec));

						runningdata = fs.Position;
					}

				}

			}

		}
	}
	
	public struct S3DFile
	{
		public int Offset { get; set; }
		public int Size { get; set; }
		public string Name { get; set; }
		public int Type { get; set; }
		public int Unk1 { get; set; }
		public int Unk2 { get; set; }
	}
}
