using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityInfo.DataEntities;
using InfinityInfo.DataEntities.Entities;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var ent = new SampleEntity();
            ent.PrimaryKeyID.Value = "111";
            ent.FirstName.Value = "Glenn";
            ent.LastName.Value = "Ferrie";
            ent.City.Value = "NYC";
            ent.Save(QueryExecutionMethod.Insert);

            var query = new SampleEntity();
            query.LastName.Value = "Fer%";
            query.LastName.Comparator = DataFieldComparison.Like;

            var factory = new DataEntityFactory();
            var results = factory.Retrieve(query);


        }
    }
}
