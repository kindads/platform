using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class CurrentConversation
    {
        public string ProfileName { set; get; }
        public string ProfileTagLine { set; get; }
        public string Id { set; get; }
        public bool SendProposal { set; get; }
        public string Message { set; get; }
        public string MessageWithoutHtml { set; get; }
        public DateTime MessageTime { set; get; }
        public string SignedBy { set; get; }

        public CurrentConversation()
        {
            ProfileName = string.Empty;
            ProfileTagLine = string.Empty;
            Id = string.Empty;
            SendProposal = false;
            Message = string.Empty;
            MessageTime = DateTime.UtcNow; // to convert an any time zone
            SignedBy = "UserName or ProfileName";
        }
    }
}
