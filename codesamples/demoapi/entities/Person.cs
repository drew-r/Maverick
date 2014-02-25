using Maverick;
using System;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace MaverickDynamic
{
    [Table(Name="People")]
    public class Person
    {
		public Person() { }

        Guid _id = Guid.NewGuid();
        [Column(IsPrimaryKey = true)]
        public Guid ID { get { return _id; } set { _id = value; } }

		[Column]
			public string Forename { get; set; }
		[Column]
			public string Surname { get; set; }


        [Column]
        public string AuthUser { get; set; }
        [JsonIgnore]
        [Column]
        public string AuthPass { get; set; }		 
    }
}
