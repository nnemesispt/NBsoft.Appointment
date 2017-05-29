using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Dictionary
{
    public class Language
    {
        private string _Image;
        private System.Globalization.CultureInfo _CI;
        private string _DictionaryFile;

        public Language()
        {
            _Image = null;
            _CI = null;
            _DictionaryFile = null;
        }

        public Language(string flagImageFile, System.Globalization.CultureInfo cultureInfo, string dictionaryFile)
            : this()
        {
            _Image = flagImageFile;
            _CI = cultureInfo;
            _DictionaryFile = dictionaryFile;
        }

        public string Image { get { return _Image; } set { _Image = value; } }
        public System.Globalization.CultureInfo CultureInfo { get { return _CI; } set { _CI = value; } }
        public string DictionaryFile { get { return _DictionaryFile; } set { _DictionaryFile = value; } }

        public override string ToString()
        {
            return _CI.NativeName;
        }
        public override int GetHashCode()
        {
            return _CI.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Language other = obj as Language;
            return this._CI.Equals(other._CI);
        }

    }
}
