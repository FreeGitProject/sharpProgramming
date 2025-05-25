using System.Xml.Linq;

namespace Programming
{
    internal class Students
    {
        private string Name;
        private int Age;

        public string SetGetName
        {
            get { return Name;}
            set { Name =value; }
        }
        public void GetName()
        {
            if (string.IsNullOrEmpty(this.Name))
            {

            }
            else
                Console.WriteLine("Name Is : " + this.Name);   

        }
        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name Is empty ");
            }
            else
                this.Name = name;

        }
        public void GetAge()
        {
            if (Age <= 0)
            { }
            else

                Console.WriteLine("Age Is : " + this.Age);
            
                

        }
        public void SetAge(int age)
        {
            if (age <= 0)
                Console.WriteLine("Age is invalid");
            else
                this.Age = age;

        }


    }
}
