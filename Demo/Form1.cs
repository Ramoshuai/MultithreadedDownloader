using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Gac;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DownLoadFile dlf = new DownLoadFile();

        private string downloadPath = AppDomain.CurrentDomain.BaseDirectory + "DownloadFiles";
        private void btnTest_Click(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines("1.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                string path = Uri.EscapeUriString(lines[i]);
                string filename = Path.GetFileName(path);
                ListViewItem item = listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), filename, "0", "0", "0%", "0", "0", DateTime.Now.ToString(), "等待中", lines[i] }));
                int id = item.Index;
                dlf.AddDown(path, downloadPath, id, id + filename);
            }
            dlf.StartDown();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dlf.DownLoadThreadNum = 3;//下载线程数，不设置默认为3
            dlf.doSendMsg += SendMsgHander;//下载过程处理事件

            tb_downloadPath.Text = downloadPath;
        }
        private void SendMsgHander(DownMsg msg)
        {
            switch (msg.Tag)
            {
                case DownStatus.Start:
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        listView1.Items[msg.Id].SubItems[2].Text = msg.LengthInfo;
                        listView1.Items[msg.Id].SubItems[8].Text = "开始下载";
                        listView1.Items[msg.Id].SubItems[7].Text = DateTime.Now.ToString();
                    });
                    break;
                case DownStatus.End:
                case DownStatus.DownLoad:
                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            listView1.Items[msg.Id].SubItems[3].Text = msg.SizeInfo;
                            listView1.Items[msg.Id].SubItems[4].Text = msg.Progress.ToString() + "%";
                            listView1.Items[msg.Id].SubItems[5].Text = msg.SpeedInfo;
                            listView1.Items[msg.Id].SubItems[6].Text = msg.SurplusInfo;
                            if (msg.Tag == DownStatus.DownLoad)
                            {
                                listView1.Items[msg.Id].SubItems[8].Text = "下载中";
                            }
                            else
                            {
                                listView1.Items[msg.Id].SubItems[8].Text = "下载完成";
                            }
                            Application.DoEvents();
                        });
                    }));
                    break;
                case DownStatus.Error:
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        listView1.Items[msg.Id].SubItems[6].Text = "失败";
                        listView1.Items[msg.Id].SubItems[8].Text = msg.ErrMessage;
                        Application.DoEvents();
                    });
                    break;
            }
        }

        private void bt_path_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.tb_downloadPath.Text = path.SelectedPath;
            downloadPath = path.SelectedPath;
        }
    }
}
