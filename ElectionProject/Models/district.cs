//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElectionProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class district
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public district()
        {
            this.election_list = new HashSet<election_list>();
            this.election_list_request = new HashSet<election_list_request>();
            this.voter_user = new HashSet<voter_user>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public int competitive_seat { get; set; }
        public int women_seats { get; set; }
        public int christian_seats { get; set; }
        public int circassians_seats { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<election_list> election_list { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<election_list_request> election_list_request { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<voter_user> voter_user { get; set; }
    }
}