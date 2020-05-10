using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTrialTask
{
    public partial class Form1 : Form
    {
        private bool semaphore = true;
        public delegate void MyDelegate();

        public Form1()
        {
            InitializeComponent();
            //Клик перехода на гугл таблицы
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
        }

    

        /// <summary>
        /// Кнопка старт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            labelSizeDd.Text = "";


          labelSizeDd.Text = "-"  + JobPostgreSQL.GetSizeTablesAsync(); // получение размера БД
          // labelInfo.Text += JobPostgreSQL.GetTable();

            GetSizeInForm();
            JobSpreadsheetTable.InitStart();

        }

        /// <summary>
        /// Запуск при старте программы
        /// </summary>
        async void Init()
        {
            int i = 0;
            while (semaphore)
            {
            i++;
            labelSizeDd.Text = "-" + JobPostgreSQL.GetSizeTablesAsync();
            // await Task.Run(() => GetSizeInForm());
             labelInfo.Text = "";
             label4.Text = $"Обновленно соединение с БД {i}";

             var tempList = JobPostgreSQL.GetTable();
             foreach (var item in tempList)
               {
                 labelInfo.Text += item;
               }

            //label4.Text = i.ToString();
            await Task.Delay(6000);
           // label4.Text += "Обновленно соединение с бд";
            }

        }

        /// <summary>
        /// Получение и вывод размера БД
        /// </summary>
        private  void GetSizeInForm()
        {
            labelInfo.Text = "";

            var tempList=   JobPostgreSQL.GetTable();

            foreach (var item in tempList)
            {
                labelInfo.Text += item;
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            semaphore = false;
                Close();
                Application.Exit();
        }

        /// <summary>
        /// Обновление при запуске формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MyDelegate(Init));
        }

        /// <summary>
        /// Кнопка перехода на гугл докс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/spreadsheets/d/1nr9EpJ9CEezikivzWVIGpr7aooOvWuZN6MsQlio-3_g/edit#gid=0");
        }
    }

    
    
}
