using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Language_XML_Editor
{
    public  class Models
    {
        public class ListData : INotifyPropertyChanged
        {
            private string _name;
            private string _body;
            private Brush _basecolor;

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
            

            public string TitleCorrect { get; private set; }

            public Brush BaseColor
            {
                get
                {
                    return _basecolor;
                }
                set
                {
                    _basecolor = value;
                    OnPropertyChanged1();
                }
            }

            public ListData(string name, string body, string correctTitle)
            {
                _basecolor = new SolidColorBrush(Colors.White);
                _name = name;
                _body = body;
                TitleCorrect = correctTitle;
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
