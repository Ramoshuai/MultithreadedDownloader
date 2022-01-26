using System;
using System.Collections.Generic;
using System.Text;

namespace Gac
{
    public class DownloadProgressListener : IDownloadProgressListener
    {
        private long presize=0;
        private readonly DownMsg downMsg;
        public DownloadProgressListener(DownMsg downmsg)
        {
            this.downMsg = downmsg;
            //this.id = id;
            //this.Length = Length;
        }
        public delegate void dlgSendMsg(DownMsg msg);
        public dlgSendMsg doSendMsg = null;
        public void OnDownloadSize(long size)
        {
            //下载速度
            if (downMsg.Size == 0)
            {
                downMsg.Speed = size;
            }
            else
            {
                downMsg.Speed = (float)(size - downMsg.Size);
                
            }
            if (downMsg.Speed == 0)
            {
                downMsg.Surplus = -1;
                downMsg.SurplusInfo = "未知";
            }
            else
            {
                downMsg.Surplus = ((downMsg.Length - downMsg.Size) / downMsg.Speed);
            }
            downMsg.Size = size; //下载总量
           
            if (size == downMsg.Length)
            {
                //下载完成
                downMsg.Tag = DownStatus.End;
                downMsg.Speed = 0;
                downMsg.SurplusInfo = "已完成";
            }
            else
            {
                //下载中
                downMsg.Tag = DownStatus.DownLoad;
            }
            
            
            if (doSendMsg != null) doSendMsg(downMsg);//通知具体调用者下载进度
        }

        public void OnStop()
        {
            
        }
    }
    public enum DownStatus
    {
        Start,
        DownLoad,
        End,
        Error,
        Stop
    }
    public class DownMsg
    {
        public int Id { get; set; }

        /// <summary>
        /// 标志
        /// </summary>
        public DownStatus Tag { get; set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public int Length { get; set; }
        public string LengthInfo => GetFileSize(Length);

        /// <summary>
        /// 已经下载量
        /// </summary>
        public long Size { get; set; }
        public string SizeInfo => GetFileSize(Size);

        /// <summary>
        /// 速度
        /// </summary>
        public float Speed { get; set; }
        public string SpeedInfo => GetFileSize(Speed);

        /// <summary>
        /// 剩余
        /// </summary>
        public float Surplus { get; set; }

        private string _surplusInfo;
        public string SurplusInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(_surplusInfo))
                    return _surplusInfo;
                return Surplus > 0 ? GetDateName((int)Math.Round(Surplus, 0)) : "";
            }
            set { _surplusInfo = value; }
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrMessage { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        public double Progress
        {
            get
            {
                if (Length >= Size)
                {
                    return Math.Round((double)Size / Length * 100, 2);
                }
                else
                {
                    return -1;
                }
            }
        }

        

        private string GetFileSize(float Len)
        {
            float temp = Len;
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (temp >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                temp /= 1024;
            }
            return string.Format("{0:0.##} {1}", temp, sizes[order]);
        }
        private string GetDateName(int Second)
        {
            float temp = Second;
            string suf = "秒";
            if (Second > 60)
            {
                suf = "分钟";
                temp /= 60;
                if (Second > 60)
                {
                    suf = "小时";
                    temp /= 60;
                    if (Second > 24)
                    {
                        suf = "天";
                        temp /= 24;
                        if (Second > 30)
                        {
                            suf = "月";
                            temp /= 30;
                            if (Second > 12)
                            {
                                suf = "年";
                                temp /= 12;
                            }
                        }
                    }
                }
            }
            return String.Format("{0:0} {1}", temp, suf);
        }
    }
}