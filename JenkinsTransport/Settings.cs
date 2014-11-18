using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace JenkinsTransport
{
    [XmlRoot(ElementName = "JenkinsServerManagerSettings")]
    public class Settings
    {
        #region Properties
        [XmlElement(ElementName = "Server")]
        public string Server { get; set; }

        [XmlElement(ElementName = "Project")]
        public string Project { get; set; }

        [XmlElement(ElementName = "AuthorizationInformation")]
        private string _authorizationInformation;
        public string AuthorizationInformation
        {
            get
            {
                return _authorizationInformation ?? (_authorizationInformation = !String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password)
                  ? Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", Username, Password)))
                  : String.Empty);
            }
            set
            {
                _authorizationInformation = value;
            }
        }
        
        [XmlIgnore]
        public string Username { get; set; }
        [XmlIgnore]
        public string Password { get; set; }

        #endregion

        public static Settings GetSettings(string settingsString)
        {
            if (String.IsNullOrEmpty(settingsString))
            {
                return new Settings();
            }
            else
            {
                var ser = new XmlSerializer(typeof(Settings));
                using (var rdr = new StringReader(settingsString))
                {
                    return ser.Deserialize(rdr) as Settings;
                }
            }
        }

        public override string ToString()
        {
            var ser = new XmlSerializer(typeof(Settings));
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                ser.Serialize(writer, this);
            }

            return sb.ToString();
        }

    }
}
