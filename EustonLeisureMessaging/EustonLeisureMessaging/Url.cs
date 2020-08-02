using EustonLeisureMessaging.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EustonLeisureMessaging
{
    public class Url
    {
        private string _sourceMessageId;
        private string _path;
        public string SourceMessageId
        {
            get { return _sourceMessageId; }
            set { _sourceMessageId = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
