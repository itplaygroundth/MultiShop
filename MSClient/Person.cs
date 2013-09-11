using System;

namespace MSClient
{
    internal class Person
    {
        public Person(string firstName, string secondName)
        {
            this.FirstName = firstName;
            this.SecondName = secondName;
            Comments = String.Empty;
        }
        public Person(string firstName, string secondName, string comments)
            : this(firstName, secondName)
        {
            this.Comments = comments;
        }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Comments { get; set; }
    }
}
