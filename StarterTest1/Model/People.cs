using System;
using System.ComponentModel.DataAnnotations;

namespace WindowsFormsApp1.Model
{
    public class People
    {
        [Key]
        public int IdPeople { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; } 
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string SurName { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
    }
}
