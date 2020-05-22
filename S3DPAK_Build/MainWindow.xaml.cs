using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace S3DPAK_Build
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private string SelectPak()
		{
			string selection = null;

			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.RestoreDirectory = true;
			ofd.Filter = "S3DPAK Files (*.s3dpak)|*.s3dpak";
			if ((bool)ofd.ShowDialog())
				selection = ofd.FileName;

			return selection;
		}

		private string SelectDump()
		{
			string selection = null;

			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.RestoreDirectory = true;
			ofd.Filter = "Dump Index (*.txt)|*.txt";
			if ((bool)ofd.ShowDialog())
				selection = ofd.FileName;

			return selection;
		}

		private string SelectPath()
		{
			string selection = null;

			using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = fbd.ShowDialog();
				if ((result.ToString() == "OK"))
					selection = fbd.SelectedPath;
			}

			return selection;
		}

		private string SaveFile()
		{
			string target = null;

			Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
			sfd.RestoreDirectory = true;
			sfd.Filter = "S3DPAK Files (*.s3dpak)|*.s3dpak";
			if ((bool)sfd.ShowDialog())
				target = sfd.FileName;

			return target;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string input = SelectPak();
			if (input == null)
				return;

			string path = SelectPath();
			if (path == null)
				return;

			S3DPAK pak = new S3DPAK();
			pak.LoadPak(input);
			pak.DumpAllFiles(path);

			MessageBox.Show("Files Dumped.");
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			string input = SelectDump();
			if (input == null)
				return;

			string output = SaveFile();
			if (output == null)
				return;

			S3DPAK.BuildPak(input, output);

			MessageBox.Show("S3DPAK Built.");
		}
	}
}
