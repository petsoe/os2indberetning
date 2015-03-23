﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModel
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CprNumber { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public double WorkDistanceOverride { get; set; }
        public bool RecieveMail { get; set; }
        public double DistanceFromHomeToBorder { get; set; }
        public string Initials { get; set; }
        public string FullName { get; set; }
        public double DistanceFromHomeToWork { get; set; }

        public ICollection<PersonalAddress> PersonalAddresses { get; set; }
        public ICollection<PersonalRoute> PersonalRoutes { get; set; }
        public ICollection<LicensePlate> LicensePlates { get; set; }
        public ICollection<MobileToken> MobileTokens { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<Employment> Employments { get; set; }

        public ICollection<Substitute> Substitutes { get; set; }
        public ICollection<Substitute> SubstituteFor { get; set; }
        public ICollection<Substitute> SubstituteLeaders { get; set; }
    }
}
