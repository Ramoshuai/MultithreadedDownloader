using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Gac
{
  
    public class DownLoadFile
    {
        public int DownLoadThreadNum = 3;
        List<Thread> list = new List<Thread>();
        public DownLoadFile()
        {
            doSendMsg += Change;
        }
        private void Change(DownMsg msg)
        {
            if (msg.Tag==DownStatus.Error||msg.Tag==DownStatus.End)
            {
                StartDown(1);
            }
        }
        public void AddDown(string DownUrl,string Dir, int Id = 0,string FileName="")
        {
            Thread tsk = new Thread(() =>
            {
                download(DownUrl, Dir, FileName,Id);
            });
            list.Add(tsk);
        }
        public void StartDown(int StartNum=3)
        {
            for (int i2 = 0; i2 < StartNum; i2++)
            {
                lock (list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].ThreadState == ThreadState.Unstarted || list[i].ThreadState == ThreadState.Suspended)
                        {
                            list[i].Start();
                            break;
                        }
                    }
                }
            }
        }

        public void StopDown()
        {
            
        }


        public delegate void dlgSendMsg(DownMsg msg);
        public event dlgSendMsg doSendMsg;
        private void download(string path, string dir, string filename, int id)
        {
            DownMsg msg = new DownMsg()
            {
                Id = id
            };
            try
            {
                msg.Tag = 0;
                doSendMsg(msg);
                FileDownloader loader = new FileDownloader(path, dir, filename, DownLoadThreadNum);
                loader.data.Clear();
                msg.Tag = DownStatus.Start;
                msg.Length = (int)loader.getFileSize(); ;
                doSendMsg(msg);
                DownloadProgressListener linstenter = new DownloadProgressListener(msg)
                {
                    doSendMsg = new DownloadProgressListener.dlgSendMsg(doSendMsg)
                };
                loader.download(linstenter);
            }
            catch (Exception ex)
            {
                msg.Length = 0;
                msg.Tag =DownStatus.Error;
                msg.ErrMessage = ex.Message;
                doSendMsg(msg);
               
                Console.WriteLine(ex.Message);
            }
        }


    }
   
}
