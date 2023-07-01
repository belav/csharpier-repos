using System;
using System.ComponentModel.DataAnnotations;

namespace BasicViews
{
    public class Person
    {
        public int Id { get; set; }

        [StringLength(27, MinimumLength = 2)]
        public string Name { get; set; }

        [Range(10, 54)]
        public int Age { get; set; }

        public DateTimeOffset BirthDate { get; set; }
    }
}
