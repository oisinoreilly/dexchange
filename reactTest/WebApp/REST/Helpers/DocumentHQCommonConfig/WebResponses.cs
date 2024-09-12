using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ROISoft.Compendia.CommonConfig
{
    public abstract class Response
    {
        public void Serialize(Stream stream, bool includeXslDeclaration)
        {
            // Write the declarations at the start.
            stream.Write(s_xmlDeclaration, 0, s_xmlDeclaration.Length);
            if (includeXslDeclaration)
            {
                stream.Write(s_xslDeclaration, 0, s_xslDeclaration.Length);
            }

            // Create an XML writer that doesn't write the declaration at the start.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.OmitXmlDeclaration = true;
            settings.CloseOutput = false;
            XmlWriter writer = XmlWriter.Create(stream, settings);

            // Serialize the object.
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(writer, this);
            writer.Close();
        }

        internal static readonly DateTime JAVA_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly static byte[] s_xmlDeclaration = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?>");
        private readonly static byte[] s_xslDeclaration = Encoding.UTF8.GetBytes("<?xml-stylesheet type=\"text/xsl\" href=\"SmartphoneHandler.xsl\"?>");
    }
    [XmlRoot("TestConnectionAppWorldStoreResponse")]
    public class TestConnectionAppWorldStoreResponse : Response
    {
        private string m_schemaName;
        private string m_username;
        private string m_orgname;
        private string m_encryptionKey;
        private int m_serverTimeout;
        private bool m_bRegistered;
        private string m_primaryDocument;
        private string m_primaryFormat;
        private string m_errorMessage;
        internal TestConnectionAppWorldStoreResponse() { }

        internal TestConnectionAppWorldStoreResponse(bool bRegistered, string schemaName, string errorMessage)
        {
            m_bRegistered = bRegistered;
            m_schemaName = schemaName;
            m_errorMessage = errorMessage;
        }

        [XmlAttribute("Registered")]
        public bool Registered
        {
            get { return m_bRegistered; }
            set { m_bRegistered = value; }
        }
        [XmlAttribute("SchemaName")]
        public string SchemaName
        {
            get { return m_schemaName; }
            set { m_schemaName = value; }
        }
        [XmlAttribute("PrimaryDocument")]
        public string PrimaryDocument
        {
            get { return m_primaryDocument; }
            set { m_primaryDocument = value; }
        }
        [XmlAttribute("PrimaryFormat")]
        public string PrimaryFormat
        {
            get { return m_primaryFormat; }
            set { m_primaryFormat = value; }
        }

        [XmlAttribute("Username")]
        public string Username
        {
            get { return m_username; }
            set { m_username = value; }
        }
        [XmlAttribute("Orgname")]
        public string Orgname
        {
            get { return m_orgname; }
            set { m_orgname = value; }
        }
        [XmlAttribute("EncryptionKey")]
        public string EncryptionKey
        {
            get { return @"703248896785362994344313612842748678629324346794592876134466"; }
            set { m_encryptionKey = value; }
        }

        [XmlAttribute("ServerTimeout")]
        public int ServerTimeout
        {
            get { return 24; }
            set { m_serverTimeout = value; }
        }

        [XmlAttribute("ErrorMessage")]
        public string ErrorMessage
        {
            get { return m_errorMessage; }
            set { m_errorMessage = value; }
        }
    }

}
