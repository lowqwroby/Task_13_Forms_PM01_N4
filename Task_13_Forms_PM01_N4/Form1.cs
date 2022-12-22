using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_13_Forms_PM01_N4
{
	public partial class Form1 : Form
	{
		Software[] software;
		static string[] ReadFile(FileStream fileStream)
		{
			byte[] buf = new byte[fileStream.Length];
			fileStream.Read(buf, 0, buf.Length);
			string textFromFile = Encoding.UTF8.GetString(buf);
			string[] textSplit = textFromFile.Split('\n');

			for (int i = 0; i < textSplit.Length; i++) textSplit[i] = textSplit[i].Trim();
			return textSplit;
		}

		static Software[] SetSWFromFile(TextBox tb, TextBox tb1)
		{
			Software[] sw = null;
			FileStream filesw;
			string filePath;
			while (true)
			{
				filePath = tb1.Text;
				filesw = new FileStream(filePath, FileMode.Open, FileAccess.Read);

				if (filesw.CanRead) break;
				tb.Text += String.Format("Введен некорректный путь.\r\n");
			}

			string[] textSplit = ReadFile(filesw);
			filesw.Close();

			int countsw = 0;
			for (int i = 0; i < textSplit.Length; i++)
			{
				if (textSplit[i] == Software.SWdate[0] || textSplit[i] == Software.SWdate[1] || textSplit[i] == Software.SWdate[2]) countsw++;
			}
			if (countsw == 0) { tb.Text += String.Format("Ошибка. Вероятно файл заполнен неверно или файл пустой."); return sw; }

			sw = new Software[countsw];
			string[] countstr = new string[5];

			int ifw = 0;
			int typeindex;

			for (int i = 0; i < sw.Length; i++)
			{
				typeindex = ifw;
				for (int j = ifw + 1; j < textSplit.Length; j++)
				{
					if (textSplit[j] == Software.SWdate[0] || textSplit[j] == Software.SWdate[1] || textSplit[j] == Software.SWdate[2])
					{
						ifw = j;
						break;
					}
					for (int l = 0; l < countstr.Length; l++)
					{
						if (l == 2 && textSplit[ifw] == Software.SWdate[0]) break;
						if (l == 4 && textSplit[ifw] == Software.SWdate[1]) break;
						countstr[l] = textSplit[j];
						j++;
					}
					j--;
				}
				if (textSplit[typeindex] == "Свободное") sw[i] = new Free(countstr[0], countstr[1]);
				else if (textSplit[typeindex] == "Условно-бесплатное") sw[i] = new Shareware(countstr[0], countstr[1], Convert.ToDateTime(countstr[2]), Convert.ToInt32(countstr[3]));
				else if (textSplit[typeindex] == "Коммерческое") sw[i] = new Commercial(countstr[0], countstr[1], Convert.ToSingle(countstr[2]), Convert.ToDateTime(countstr[3]), Convert.ToInt32(countstr[4]));
				else tb.Text += String.Format("Не получилось добавить ПО по индексу.");
			}
			tb.Text += String.Format("Данные о ПО записаны\r\n\r\n");
			return sw;
		}

		public Form1()
		{
			InitializeComponent();
		}

		public void button1_Click(object sender, EventArgs e)
		{
			try
			{
				textBox2.Text = "";
				software = SetSWFromFile(textBox2, textBox1);
			}
			catch (FormatException)
			{
				MessageBox.Show("Введены неверные значения", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch(Exception ec)
			{

				//MessageBox.Show("Неизвестная ошибка", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				MessageBox.Show(ec.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				textBox2.Text = "";
				if (software == null)
				{
					textBox2.Text += String.Format("Данные из файла не записаны\r\n");
				}
				textBox2.Text += String.Format("Вывод всех ПО\r\n\r\n");
				Software.GetAllSWInfo(software, textBox2);
			}
			catch
			{
				MessageBox.Show("Неизвестная ошибка", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				textBox2.Text = "";
				if (software.Length == 0) throw new Exception();
				for (int i = 0; i < software.Length; i++)
				{
					if (software[i].MathingTerm()) software[i].GetSoftwareInfo(textBox2);
				}
			}
			catch
			{
				MessageBox.Show("Неизвестная ошибка", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			textBox2.Text = "";
		}
	}

	abstract class Software
	{
		static public string[] SWdate = new string[3] { "Свободное", "Условно-бесплатное", "Коммерческое" };
		static public void GetAllSWInfo(Software[] sw, TextBox textBox)
		{
			for (int i = 0; i < sw.Length; i++)
			{
				sw[i].GetSoftwareInfo(textBox);
			}
		}
		abstract public void GetSoftwareInfo(TextBox textbox);
		abstract public bool MathingTerm();
	}

	class Free : Software
	{
		string name;
		string manufacturer;

		public Free(string name, string manufacturer)
		{
			this.name = name;
			this.manufacturer = manufacturer;
		}

		public override void GetSoftwareInfo(TextBox textbox)
		{
			textbox.Text += String.Format($"Название ПО: {name}\r\nПроизводитель: {manufacturer}\r\n\r\n");
		}
		public override bool MathingTerm()
		{
			return true;
		}
	}

	class Shareware : Software
	{
		string name;
		string manufacturer;
		DateTime dateOfInstall;
		int termOfFreeUse;

		public Shareware(string name, string manufacturer, DateTime dateOfInstall, int termOfFreeUse)
		{
			this.name = name;
			this.manufacturer = manufacturer;
			this.dateOfInstall = dateOfInstall;
			this.termOfFreeUse = termOfFreeUse;
		}

		public override void GetSoftwareInfo(TextBox textBox)
		{
			textBox.Text += String.Format("Название ПО: {0}\r\nПроизводитель: {1}\r\nДата установки: {2}\r\nСрок использования: {3} дней\r\n\r\n", name, manufacturer, dateOfInstall.ToShortDateString(), termOfFreeUse);
		}
		public override bool MathingTerm()
		{
			if (DateTime.Now <= dateOfInstall.AddDays(termOfFreeUse)) return true;
			else return false;
		}
	}
	class Commercial : Software
	{
		string name;
		string manufacturer;
		float price;
		DateTime dateOfInstall;
		int termOfUse;

		public Commercial(string name, string manufacturer, float price, DateTime dateOfInstall, int termOfUse)
		{
			this.name = name;
			this.manufacturer = manufacturer;
			this.price = price;
			this.dateOfInstall = dateOfInstall;
			this.termOfUse = termOfUse;
		}

		public override void GetSoftwareInfo(TextBox textBox)
		{
			textBox.Text += String.Format("Название ПО: {0}\r\nПроизводитель: {1}\r\nЦена: {2}\r\nДата установки: {3}\r\nСрок использования: {4} дней\r\n\r\n", name, manufacturer, price, dateOfInstall.ToShortDateString(), termOfUse);
		}

		public override bool MathingTerm()
		{
			if (DateTime.Now <= dateOfInstall.AddDays(termOfUse)) return true;
			else return false;
		}
	}

}
