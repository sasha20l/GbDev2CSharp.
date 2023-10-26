﻿using CloneHabr.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloneHabr.Data
{
    [Table("Accounts")]
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        
        [StringLength(255)]
        public string EMail { get; set; }

        [StringLength(255)]
        public string FirstName { get; set; }

        [StringLength(255)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string SecondName { get; set; }

        public DateTime Birthday { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool Online { get; set; }

        public int Gender { get; set; }

        public int? Raiting { get; set; }

    }
}
