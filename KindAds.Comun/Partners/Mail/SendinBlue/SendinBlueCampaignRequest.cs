using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Partners.Mail.SendinBlue
{
    public class SendinBlueCampaignRequest
    {
        public string ApiKey { set; get; }
        public string Category { set; get; }
        public string FromName { set; get; }

        public string Name { set; get; }

        public string Bat { set; get; }

        private string Content { set; get; }

    
        public string HtmlContent
        {
            set { Content = value.ToLower(); }
            get
            {
                if(Content.Contains("html") && Content.Contains("body"))
                {
                    return Content;
                }
                else
                {
                    return string.Format("<html><body>{0}</body></html>", Content);
                }               
            }
        }

        public string HtmlUrl { set; get; }

        public List<int> ListIds { set; get; }

        public DateTime ScheduleCampaign { set; get; }
        
        public  DateTime Schedule {
            set
            {
                ScheduleCampaign = value;
                ScheduledTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}",value.Year,value.Month,value.Day,value.Hour,value.Minute,value.Second);
            }
            get
            {
                return  DateTime.Parse(ScheduledTime);
            }
        }

        public string ScheduledTime { set ; get; } //Format: 2018-05-16 16:05:01

        public string Subject { set; get; }

        public string FromEmail { set; get; }

        public string ReplyTo { set; get; }

        public string ToField { set; get; }

        public List<int> ExcludeList { set; get; }

        public string AttachmentUrl { set; get; }

        public int InlineImage { set; get; }

        public int MirrorActive { set; get; }

        public int SendNow { set; get; }

        public string UtmCampaign { set; get; }

        public SendinBlueCampaignRequest()
        {
            Category = string.Empty;
            FromName = "[DEFAULT_FROM_NAME]";
            Name = string.Empty;
            Bat = string.Empty;
            HtmlContent = string.Empty;
            HtmlUrl = string.Empty;
            ListIds = new List<int>();
            Schedule = DateTime.Now; //En automatico guarda la fecha en ScheduleCampaign y en ScheduledTime
            Subject = string.Empty;
            FromEmail = string.Empty;
            ReplyTo = "[DEFAULT_REPLY_TO]";
            ToField = "[PRENOM] [NOM]";
            ExcludeList = new List<int>();
            AttachmentUrl = string.Empty;
            InlineImage = 1;
            MirrorActive = 0;
            SendNow = 0;
            UtmCampaign = string.Empty;
        }

        public  Dictionary<string, Object> GetSendinBlueCampaignRequestObject()
        {
            Dictionary<string, Object> data =  new Dictionary<string, Object>();
            data.Add("category", this.Category);
            data.Add("from_name", this.FromName);
            data.Add("name", this.Name);
            data.Add("bat", this.Bat);
            data.Add("html_content", this.HtmlContent);
            data.Add("html_url", this.HtmlUrl);
            data.Add("listid", this.ListIds);
            data.Add("scheduled_date", this.ScheduledTime);
            data.Add("subject", this.Subject);
            data.Add("from_email", this.FromEmail);
            data.Add("reply_to", this.ReplyTo);
            data.Add("to_field",this.ToField);
            data.Add("exclude_list",this.ExcludeList);
            data.Add("attachment_url", this.AttachmentUrl);
            data.Add("inline_image", this.InlineImage);
            data.Add("mirror_active", this.MirrorActive);
            data.Add("send_now", this.SendNow);
            data.Add("utm_campaign", this.UtmCampaign);
            return data;
        }
    }
}
