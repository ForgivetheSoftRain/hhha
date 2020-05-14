using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace GetInfomation
{
    class Program
    {
        static int i = 0;
        static string account = "";
        static string password = "";
        static string grade = "";
        static string exam = "";
        static void Main(string[] args)
        {
            Console.Write("请输入账号密码\n\rAccount:\t");
          //  account = Console.ReadLine();
            Console.Write("Password:\t");
            //password = Console.ReadLine();
            account = "201709001013";
            password = "llf99723";
            grade = "";
            exam = "";
            try
            {
                i++;
                new Program().getInfo(out grade, out exam, account, password);
                new Program().handleGradeInfo(grade, account);
                new Program().handleExamInfo(exam, account);

                Timer aTimer = new Timer();
                aTimer.Elapsed += new ElapsedEventHandler(TimedEvent);
                aTimer.Interval = 3600 * 1000;
                aTimer.Enabled = true;
                aTimer.Enabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm") + ":" + e.Message);
            }
            string strLine;
            do
            {
                strLine = Console.ReadLine();
            } while (strLine != null && strLine != "exit");
            return;
        }

        private static void TimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                i++;
                new Program().getInfo(out grade, out exam, account, password);
                new Program().handleGradeInfo(grade, account);
                new Program().handleExamInfo(exam, account);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm") + ":" + ex.Message);
            }
        }
        private void getInfo(out string gradeList, out string examList, string account, string password)
        {
            // string encoded = encodeInp(account) + "%25%25%25" + encodeInp(password).Replace("=", "%3D");
            string time = "2019-2020-1";
            CookieContainer cookieContainer = new CookieContainer();
            string url = "https://jwxt.ncepu.edu.cn/Logon.do?method=logon&flag=sess";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";// POST OR GET， 如果是GET, 则没有第二步传参，直接第三步，获取服务端返回的数据
            req.KeepAlive = false;
            req.Timeout = 200 * 1000;
            req.AllowAutoRedirect = false;//服务端重定向。一般设置false
            req.CookieContainer = cookieContainer;
            HttpWebResponse resp = new HttpWebResponse();
            try
            {                
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm") + ":" + ex.Message);
            }
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            string ResponseGet = "";
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                ResponseGet = reader.ReadToEnd();
            }
            var scode = ResponseGet.Split("#")[0];
            var sxh = ResponseGet.Split("#")[1];
            var code = account + "%%%" + password;
            var encoded = "";
            for (var i = 0; i < code.Length; i++)
            {
                if (i < 20)
                {
                    encoded = encoded + code.Substring(i, i + 1) + scode.Substring(0, int.Parse(sxh.Substring(i, i + 1)));
                    scode = scode.Substring(int.Parse(sxh.Substring(i, i + 1)), scode.Length - 1);
                }
                else
                {
                    encoded = encoded + code.Substring(i, code.Length - 1);
                    i = code.Length - 1;
                }
            }
            //url = "http://jwxt.ncepu.edu.cn/jsxsd/xk/LoginToXk";
            url = "https://jwxt.ncepu.edu.cn/Logon.do?method=logon";
            req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "Post";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9;";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3941.4 Safari/537.36";
            req.AllowAutoRedirect = true;//服务端重定向。一般设置false
            req.CookieContainer = cookieContainer;
            req.KeepAlive = false;
            req.ContentType = "application/x-www-form-urlencoded;";
            #region 添加Post 参数
            string postData = string.Format("userAccount=" + account + "&userPassword=" + password + "&encoded=" + encoded);
            byte[] postdatabyte = Encoding.GetEncoding("UTF-8").GetBytes(postData);
            req.ContentLength = postdatabyte.Length;
            using (Stream tstream = req.GetRequestStream())
            {
                tstream.Write(postdatabyte, 0, postdatabyte.Length);
                tstream.Close();
            }
            #endregion 
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm") + ":" + ex.Message);
            }
            stream = resp.GetResponseStream();
            //获取响应内容
            ResponseGet = "";
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                ResponseGet = reader.ReadToEnd();
            }

            url = "http://jwxt.ncepu.edu.cn/jsxsd/newxspj/zhxspj_list.do";
            req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "Get";
            req.AllowAutoRedirect = true;//服务端重定向。一般设置false
            req.CookieContainer = cookieContainer;
            req.KeepAlive = false; try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm") + ":" + ex.Message);
            }
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                ResponseGet = reader.ReadToEnd();
            }
            gradeList = ResponseGet.Replace("\r\n", "").Replace(" ", "").Replace("\t", "");

            url = "http://jwxt.ncepu.edu.cn/jsxsd/xsks/xsksap_list";
            req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "Post";
            //  req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9;";
            // req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3941.4 Safari/537.36";
            req.AllowAutoRedirect = true;//服务端重定向。一般设置false
            req.CookieContainer = cookieContainer;
            req.ContentType = "application/x-www-form-urlencoded;";
            #region 添加Post 参数
            postData = string.Format("xqlbmc=&xnxqid=" + time + "&kc=&ksjs=&jkls=");
            postdatabyte = Encoding.GetEncoding("UTF-8").GetBytes(postData);
            req.ContentLength = postdatabyte.Length;
            using (Stream tstream = req.GetRequestStream())
            {
                tstream.Write(postdatabyte, 0, postdatabyte.Length);
                tstream.Close();
            }
            #endregion
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
                stream = resp.GetResponseStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm") + ":" + ex.Message);
            }
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                ResponseGet = reader.ReadToEnd();
            }
            examList = ResponseGet.Replace("\r\n", "").Replace(" ", "").Replace("\t", "");
            return;
        }

        private string encodeInp(string input)
        {
            var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
            var output = "";
            int chr1 = 0, chr2 = 0, chr3 = 0;
            int enc1 = 0, enc2 = 0, enc3 = 0, enc4 = 0;
            var i = 0;
            do
            {
                if (i < input.Length)
                    chr1 = (int)input[i++];
                if (i < input.Length)
                    chr2 = (int)input[i++];
                if (i < input.Length)
                    chr3 = (int)input[i++];
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;
                if (chr2 == 0)
                {
                    enc3 = enc4 = 64;
                }
                else if (chr3 == 0)
                {
                    enc4 = 64;
                }
                output = output + keyStr[enc1] + keyStr[enc2] + keyStr[enc3] + keyStr[enc4];
                chr1 = chr2 = chr3 = 0;
                enc1 = enc2 = enc3 = enc4 = 0;
            } while (i < input.Length);
            return output;

        }

        private void handleExamInfo(string examList, string account)
        {
            Console.WriteLine("第" + i + "次执行考试获取");
            string examPattern = "座位号</th></tr>(.*)</table>";
            Match resultExam = Regex.Match(examList, examPattern);
            string examData = resultExam.Groups[1].Value.ToString().Replace("style=\"background-color:#8b8b8b69;\"", "");
            string classExamPattern = "<tr><td>(.{0,4})</td><tdalign=\"left\"style=\"max-width:100px;word-wrap:break-word;word-break:break-all;white-space:normal;\">(.{0,10})</td><tdalign=\"left\">(.{7,9})</td><tdalign=\"left\">(.{0,20})</td><td>(.{0,30})</td><td>(.{0,20})</td><td>(.{0,8})</td><td></td><tdstyle=\"max-width:100px;word-wrap:break-word;word-break:break-all;white-space:normal;\"></td><td></td></tr>";
            MatchCollection resultClassExam = Regex.Matches(examData, classExamPattern);
            StringBuilder tempExam = new StringBuilder("");
            for (int i = 0; i < resultClassExam.Count; i++)
            {
                tempExam.Append(resultClassExam[i].Groups[4].Value.ToString() + "|");
                tempExam.Append(resultClassExam[i].Groups[5].Value.ToString() + "|");
                tempExam.Append(resultClassExam[i].Groups[6].Value.ToString() + "|");
            }
            string exam = "";
            string fileName = "/" + account + "Exams.txt";//文件名称与路径
            string msg = "";
            if (File.Exists(fileName) == true)
            {
                exam = File.ReadAllText(fileName);
                List<Exam> examArray = ToExamArray(exam);
                List<Exam> tempExamArray = ToExamArray(tempExam.ToString());
                var ChangeInfo = tempExamArray.Except(examArray);
                foreach (var i in ChangeInfo)
                {
                    msg += i.ExamName + "的考试信息已更新：" + i.ExamTime + "\t" + i.ExamPosition + "\n\r";
                }
                if (!msg.Equals(""))
                {
                    EmailSend(msg, "考试更新");
                    //Console.WriteLine("考试更新:" + msg);
                    File.WriteAllText(fileName, tempExam.ToString());
                }
            }
            else
            {
                File.WriteAllText(fileName, tempExam.ToString());
            }
        }


        private void handleGradeInfo(string gradeList, string account)
        {
            Console.WriteLine("第" + i + "次执行成绩获取");
            string gradePattern = ">操作</th></tr>(.*)</table><divid=\"PagingControl1";
            Match resultGrade = Regex.Match(gradeList, gradePattern);
            string gradeData = resultGrade.Groups[1].Value.ToString();
            string classGradePattern = "<tr><td>(.{1,2})</td><td>(.{7,12})</td><td>(.{2,4})</td><td>(.{1,20})</td><td>(.{7,12})</td><td>(.{0,2})</td><td>(.{1,25})</td><td>(.{1,5})</td><td>(.{0,5})</td><td></td><td><ahref(.{200,850})留言</a></td></tr>";
            MatchCollection resultClassGrade = Regex.Matches(gradeData, classGradePattern);
            StringBuilder tempGrade = new StringBuilder("");
            for (int i = 0; i < resultClassGrade.Count; i++)
            {
                tempGrade.Append(resultClassGrade[i].Groups[3].Value.ToString() + "|");
                tempGrade.Append(resultClassGrade[i].Groups[7].Value.ToString() + "|");
                tempGrade.Append(GetMD5(resultClassGrade[i].Groups[10].Value.ToString()) + "|");
            }
            string score = "";
            string fileName = "/" + account + "scoreGrade.txt";//文件名称与路径
            string msg = "";
            if (File.Exists(fileName) == true)
            {
                score = File.ReadAllText(fileName);
                List<Score> scoreArray = ToScoreArray(score);
                List<Score> tempGradeArray = ToScoreArray(tempGrade.ToString());
                var ChangeInfo = scoreArray.Except(tempGradeArray);
                foreach (var i in ChangeInfo)
                {
                    msg += i.ScoreCourse + "的成绩已更新，请注意\n";
                }
                if (!msg.Equals(""))
                {
                    EmailSend(msg, "成绩更新");
                    //Console.WriteLine("成绩更新:" + msg);
                    File.WriteAllText(fileName, tempGrade.ToString());
                }
            }
            else
            {
                File.WriteAllText(fileName, tempGrade.ToString());
            }
        }
        private string GetMD5(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);
            byte[] MD5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < MD5data.Length - 1; i++)
            {
                str += MD5data[i].ToString("X");
            }
            return str;
        }
        private void EmailSend(string inf, string subject)
        {

            string address = "a5555550723@126.com";
            MailMessage message = new MailMessage();

            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress(address);

            message.From = fromAddr;
            message.To.Add("964642794@qq.com");//自己接收

            //设置邮件标题
            message.Subject = subject;
            //设置邮件内容
            message.Body = inf;
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看
            SmtpClient client = new SmtpClient("smtp.126.com", 25);

            //设置发送人的邮箱账号和授权码
            client.Credentials = new NetworkCredential(address, "llf99723");

            //发送邮件
            client.Send(message);
            Console.WriteLine("已发送更新\t" + DateTime.Now.ToString("MM-dd HH:mm:ss"));
        }
        private List<Exam> ToExamArray(string examStr)
        {
            List<Exam> exams = new List<Exam>();
            string[] examArray = examStr.Split('|');
            for (int i = 0; i < examArray.Length - 1; i += 3)
            {
                Exam exam = new Exam();
                exam.ExamName = examArray[i];
                exam.ExamTime = examArray[i + 1];
                exam.ExamPosition = examArray[i + 2];
                exams.Add(exam);
            }
            return exams;
        }
        private List<Score> ToScoreArray(string scoreStr)
        {
            List<Score> scores = new List<Score>();
            string[] examArray = scoreStr.Split('|');
            for (int i = 0; i < examArray.Length - 1; i += 3)
            {
                Score score = new Score();
                score.ScoreTName = examArray[i];
                score.ScoreCourse = examArray[i + 1];
                score.ScoreValue = examArray[i + 2];
                scores.Add(score);
            }
            return scores;
        }
        private class Score : IEquatable<Score>
        {
            public string ScoreTName;
            public string ScoreCourse;
            public string ScoreValue;
            public bool Equals(Score score)
            {
                if (!this.ScoreTName.Equals(score.ScoreTName))
                    return false;
                if (!this.ScoreCourse.Equals(score.ScoreCourse))
                    return false;
                if (!this.ScoreValue.Equals(score.ScoreValue))
                    return false;
                return true;
            }

            public override int GetHashCode()
            {
                int hashName = ScoreTName == null ? 0 : ScoreTName.GetHashCode();
                int hashTime = ScoreCourse == null ? 0 : ScoreCourse.GetHashCode();
                int hashPosition = ScoreValue == null ? 0 : ScoreValue.GetHashCode();
                return hashName ^ hashTime ^ hashPosition;
            }
        }
        private class Exam : IEquatable<Exam>
        {
            public string ExamName;
            public string ExamTime;
            public string ExamPosition;
            public bool Equals(Exam exam)
            {
                if (!this.ExamName.Equals(exam.ExamName))
                    return false;
                if (!this.ExamTime.Equals(exam.ExamTime))
                    return false;
                if (!this.ExamPosition.Equals(exam.ExamPosition))
                    return false;
                return true;
            }

            public override int GetHashCode()
            {
                int hashName = ExamName == null ? 0 : ExamName.GetHashCode();
                int hashTime = ExamTime == null ? 0 : ExamTime.GetHashCode();
                int hashPosition = ExamPosition == null ? 0 : ExamPosition.GetHashCode();
                return hashName ^ hashTime ^ hashPosition;
            }
        }
    }

}
