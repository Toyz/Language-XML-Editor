using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Language_XML_Editor
{
    public  class Models
    {
        public class ListData : INotifyPropertyChanged
        {
            private string _name;
            private string _body;

            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged1();
                }
            }
            public string Body
            {
                get { return _body; }
                set
                {
                    _body = value;
                    OnPropertyChanged1();
                }
            }

            public ListData(string name, string body)
            {
                _name = name;
                _body = body;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged1([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
