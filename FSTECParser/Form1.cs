using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSTECParser
{
    public partial class Form1 : Form
    {
        List<string> stringList = new List<string>();
        SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
        private static BindingList<Threat> threatList = new BindingList<Threat>();
        private static BindingList<Threat> updateThreatList = new BindingList<Threat>();

        private static string pathThreatList = "C:\\IO\\ThreatListStec.xlsx";
        private static string pathUpdateThreatList = "C:\\IO\\CloneThreatListStec.xlsx";

        private static int startItem = 0;
        private static int lastItem = 15;

        public Form1()
        {
            InitializeComponent();
            SaveFileDialog1.Filter = "Text Files(*.txt)|*.txt";
            if (!File.Exists(pathThreatList))
            {
                new Thread(new ThreadStart(AutoDownload)).Start();
            }
            else
            {
                AutoParse();
                new Thread(new ThreadStart(CheckUpdate)).Start();
            }
        }

        private void AutoDownload()
        {
            DialogResult result = MessageBox.Show("Уважаемый пользователь, на вашем компьютере отсутствует локальная база данных угроз безопасности информации. Загрузить ее на ваш компьютер перед началом работы?",
                "Уведомление",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.Yes)
            {
                new WebClient().DownloadFile("https://bdu.fstec.ru/documents/files/thrlist.xlsx", pathThreatList);
                result = MessageBox.Show("База данных угроз безопасности информации была успешно загружена на ваш компьютер.",
                    "Уведомление",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.None,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            if(result == DialogResult.OK)
            {
                threatParse(pathThreatList);
                for (int i = 0, j = 0; i < stringList.Count; i++, j++)
                {
                    threatList.Add(InitializeThreat(stringList[i].Split('@')));
                }
            }
        }

        private void AutoParse()
        {
            threatParse(pathThreatList);
            for (int i = 0, j = 0; i < stringList.Count; i++, j++)
            {
                threatList.Add(InitializeThreat(stringList[i].Split('@')));
            }
            if (File.Exists(pathUpdateThreatList))
            {
                File.Delete(pathUpdateThreatList);
            }
            new WebClient().DownloadFile("https://bdu.fstec.ru/documents/files/thrlist.xlsx", pathUpdateThreatList);
            threatParse(pathUpdateThreatList);
            for (int i = 0, j = 0; i < stringList.Count; i++, j++)
            {
                updateThreatList.Add(InitializeThreat(stringList[i].Split('@')));
            }
            File.Delete(pathUpdateThreatList);
        }

        private static void CheckUpdate()
        {
            int length = 0;
            string message = "Добавлено новых записей: " + (updateThreatList.Count - threatList.Count) + "\n";
            List<Threat[]> differences = new List<Threat[]>();
            if (threatList.Count < updateThreatList.Count)
            {
                length = threatList.Count;
                for(int i = length; i < threatList.Count; i++)
                {
                    message += updateThreatList[i] + "\n";
                }
            }
            else
                length = updateThreatList.Count;
            try
            {
                for (int i = 0; i < length; i++)
                {
                    if (!threatList[i].Equals(updateThreatList[i]))
                        differences.Add(new Threat[] { threatList[i], updateThreatList[i] });
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            message += "Изменено старых записей: " + differences.Count + "\n";
            for (int i = 0; i < differences.Count; i++)
            {
                message += "Было:\n" + differences[i][0] + "\nСтало:\n" + differences[i][1] + "\n";
            }
            MessageBox.Show(message, "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (!File.Exists(pathThreatList))
            {
                new WebClient().DownloadFile("https://bdu.fstec.ru/documents/files/thrlist.xlsx", pathThreatList);
                MessageBox.Show("База данных угроз безопасности информации была успешно загружена на ваш компьютер.");
            }
            else
            {
                MessageBox.Show("База данных УБИ уже имеется на вашем компьютере.");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string filename = SaveFileDialog1.FileName;
            try
            {
                using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
                {
                    foreach (var threat in threatList)
                    {
                        sw.WriteLine(threat);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("Файл успешно сохранен!");
        }

        private void threatParse(string fileName)
        {
            stringList.Clear();
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First();
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;
                for (int rowNum = 3; rowNum <= totalRows; rowNum++)
                {
                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString());
                    stringList.Add(string.Join("@", row));
                }
            }
        }

        private Threat InitializeThreat(string[] fields)
        {
            int id = 0;
            bool confidentiality =  false, integrity = false, availability = false;
            try
            {
                id = Convert.ToInt32(fields[0]);
                confidentiality = Convert.ToInt32(fields[5]) == 1;
                integrity = Convert.ToInt32(fields[6]) == 1;
                availability = Convert.ToInt32(fields[7]) == 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            return new Threat(id, fields[1], fields[2], fields[3], fields[4],
                                        confidentiality, integrity, availability);
        }

        private void PrintAll()
        {
            listBox1.Items.Clear();
            foreach (var item in threatList)
            {
                string id = "";
                if (item.IdThreat < 10)
                    id = "00" + item.IdThreat;
                else if (item.IdThreat < 100)
                    id = "0" + item.IdThreat;
                else
                    id = "" + item.IdThreat;
                listBox1.Items.Add("УБИ." + id + ". " + item.ThreatName);
            }
        }

        private void PrintTable()
        {
            listBox1.Items.Clear();
            for (int i = startItem; i < lastItem; i++)
            {
                listBox1.Items.Add(threatList[i].ToString());
            }
        }

        private void RightShift()
        {
            if (startItem < 15)
                return;
            startItem = startItem - 15;
            lastItem = lastItem - 15;
            PrintTable();
        }

        private void LeftShift()
        {
            if (lastItem + 15 <= threatList.Count)
            {
                startItem = startItem + 15;
                lastItem = lastItem + 15;
                PrintTable();
            }
            else if (threatList.Count - lastItem <= 0)
                return;
            else
            {
                listBox1.Items.Clear();
                for (int i = lastItem; i < threatList.Count; i++)
                {
                    listBox1.Items.Add(threatList[i].ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RightShift();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PrintTable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LeftShift();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text.Equals(""))
            {
                MessageBox.Show("Вы не указали индекс", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            string confidentiality = "", integrity = "", availability = "";
            int id = Convert.ToInt32(maskedTextBox1.Text);
            if (threatList[id].Confidentiality)
                confidentiality = "Yes";
            else
                confidentiality = "No";
            if (threatList[id].Integrity)
                integrity = "Yes";
            else
                integrity = "No";
            if (threatList[id].Availability)
                availability = "Yes";
            else
                availability = "No";
            listBox1.Items.Clear();
            listBox1.Items.Add("1) Идентификатор угрозы: " + threatList[id].IdThreat + " \n");
            listBox1.Items.Add("2) Наименование угрозы: " + threatList[id].ThreatName + " \n");
            listBox1.Items.Add("3) Описание угрозы: " + threatList[id].ThreatDescription + " \n");
            listBox1.Items.Add("4) Источник угрозы: " + threatList[id].ThreatSource + " \n");
            listBox1.Items.Add("5) Объект воздействия угрозы: " + threatList[id].ThreatObject + " \n");
            listBox1.Items.Add("6) Нарушение конфиденциальности: " + confidentiality + " \n");
            listBox1.Items.Add("7) Нарушение целостности: " + integrity + " \n");
            listBox1.Items.Add("8) Нарушение доступности: " + availability + " \n");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AutoParse();
            CheckUpdate();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            threatParse(pathThreatList);
        }
    }
}
