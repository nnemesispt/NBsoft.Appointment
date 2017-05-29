using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace NBsoft.Appointment.WPF.Dictionary
{
    public class Manager
    {
        #region Variables

        string _Directory;
        List<Language> _AvailableLanguages;
        Hashtable _Dictionary;
        int _ActiveLanguage;
        ResourceDictionary _resDictionary;
        //bool _UseResourceDictionary;

        #endregion

        #region Constructor

        public Manager(string LanguageDir)
        {
            //_UseResourceDictionary = UseResourceDictionary;
            _Directory = LanguageDir;
            _AvailableLanguages = new List<Language>();
            System.IO.DirectoryInfo Di = new System.IO.DirectoryInfo(_Directory);


            foreach (System.IO.FileInfo xmlfile in Di.GetFiles("*.xaml"))
            {
                string flagimage;
                System.Globalization.CultureInfo cinfo;
                flagimage = xmlfile.FullName.Replace(xmlfile.Extension, ".png");
                System.IO.FileInfo flagImageFile = new System.IO.FileInfo(flagimage);
                if (!flagImageFile.Exists)
                    flagimage = null;

                try { cinfo = new System.Globalization.CultureInfo(xmlfile.Name.Replace(xmlfile.Extension, "")); }
                catch { cinfo = System.Globalization.CultureInfo.InvariantCulture; }

                Language ln = new Language(flagimage, cinfo, xmlfile.FullName);
                _AvailableLanguages.Add(ln);
            }

            SetActive();
        }

        #endregion

        #region Accessors

        public Language[] AvailableLanguages { get { return _AvailableLanguages.ToArray(); } }
        public Language ActiveLanguage { get { return AvailableLanguages[_ActiveLanguage]; } }
        public int ActiveLanguageId { get { return _ActiveLanguage; } }
        public ResourceDictionary Dictionary { get { return _resDictionary; } }

        #endregion

        #region Methods

        private void SetActive()
        {
            _ActiveLanguage = -1;
            SetActive(0);
        }

        public void SetActive(int LanguageIndex)
        {
            if (LanguageIndex == _ActiveLanguage)
                return;
            Language active = _AvailableLanguages[LanguageIndex];

            _resDictionary = null;
            _Dictionary = null;


            LoadResourceDictionary(active.DictionaryFile);
            LoadDictionary(active.DictionaryFile);

            _ActiveLanguage = LanguageIndex;
            OnLanguageChanged(new LanguageChangedEventArgs(active));

        }
        public void SetActive(Language language)
        {
            int activelang = -1;
            for (int i = 0; i < _AvailableLanguages.Count; i++)
            {
                if (_AvailableLanguages[i] == language)
                {
                    activelang = i;
                    break;
                }
            }
            if (activelang >= 0)
                SetActive(activelang);

        }

        private void LoadDictionary(string dictionary)
        {
            _Dictionary = new Hashtable();
            System.Xml.XmlDocument XDoc = new System.Xml.XmlDocument();
            XDoc.Load(dictionary);

            foreach (System.Xml.XmlNode XNode in XDoc.DocumentElement.ChildNodes)
            {
                if (XNode == null || XNode.NodeType == System.Xml.XmlNodeType.Comment || XNode.Attributes == null)
                    continue;
                string id = XNode.Attributes["x:Key"].Value;
                string value = XNode.InnerText;
                _Dictionary.Add(id, value);
            }
        }
        private void LoadResourceDictionary(string dictionaryFile)
        {
            _resDictionary = new ResourceDictionary();
            Uri u = new Uri(dictionaryFile, UriKind.Absolute);
            try { _resDictionary.Source = u; }
            catch
            {
                //_resDictionary.Source = null; 
            }
        }

        public string Get(object Key)
        {
            string retval = "";
            try
            {
                retval = _Dictionary[Key.ToString()].ToString();
                if (retval == "")
                    retval = Key.ToString();
            }
            catch { retval = string.Format("[{0}]", Key); }
            return retval;
        }

        public Hashtable GetDictionary(string dictionary)
        {
            Hashtable MyDictionary = new Hashtable();
            System.Xml.XmlDocument XDoc = new System.Xml.XmlDocument();
            XDoc.Load(dictionary);

            foreach (System.Xml.XmlNode XNode in XDoc.DocumentElement.ChildNodes)
            {
                if (XNode == null || XNode.NodeType == System.Xml.XmlNodeType.Comment || XNode.Attributes == null)
                    continue;
                string id = XNode.Attributes["x:Key"].Value;
                string value = XNode.InnerText;
                MyDictionary.Add(id, value);
            }

            return MyDictionary;
        }

        public void SaveNewDictionary(Hashtable dic, string pathFile)
        {
            System.IO.TextWriter tw = new System.IO.StreamWriter(pathFile);

            tw.WriteLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:system=\"clr-namespace:System;assembly=mscorlib\">");

            foreach (DictionaryEntry item in dic)
            {
                tw.WriteLine(string.Format("<system:String x:Key=\"{0}\">{1}</system:String>", item.Key, item.Value));
            }

            tw.WriteLine("</ResourceDictionary>");

            // close the stream
            tw.Close();
        }

        protected void OnLanguageChanged(LanguageChangedEventArgs e)
        {
            LanguageChanged?.Invoke(this, e);
        }

        #endregion

        #region Events
        public event LanguageChangedDelegate LanguageChanged;

        #endregion
    }
}
