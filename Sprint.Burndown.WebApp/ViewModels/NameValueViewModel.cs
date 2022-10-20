namespace Sprint.Burndown.WebApp.ViewModels
{
    public class NameValueViewModel 
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public NameValueViewModel()
        {
        }
        public NameValueViewModel(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}