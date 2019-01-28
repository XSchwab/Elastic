using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchWpf
{
    class CRUD
    {
        #region Get document info based on the ID

        public static Tuple<string, string, string, string> getDocument(string searchID)
        {
            string id = "";
            string firstname = "";
            string lastname = "";
            string city = "";

            var response = ConnectionToES.EsClient().Search<DocumentAttributes>(s => s
                .Index("accounts3")
                .Type("account3")
                .Query(q => q.Term(t => t.Field("_id").Value(searchID)))); //Search based in _id                

            //Assigining value to their controller
            foreach (var hit in response.Hits)
            {
                id = hit.Id.ToString();// Source.id.ToString();
                firstname = hit.Source.firstname.ToString();
                lastname = hit.Source.lastname.ToString();
                city = hit.Source.city.ToString();

            }

            return Tuple.Create(id, firstname, lastname, city);
        }

        #endregion Get document info based on the ID

        #region Insert document with on ID

        public static bool insertDocument(string searchID, string tbxfirstname, string tbxLastname, string tbxcity)
        {
            bool status;

            var myJson = new
            {
                firstname = tbxfirstname,
                lastname = tbxLastname,
                city = tbxcity,
            };

            var response = ConnectionToES.EsClient().Index(myJson, i => i
                .Index("accounts3")
                .Type("account3")
                .Id(searchID)
                //.Refresh()
                );

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            getAllDocument();

            return status;
        }

        #endregion Insert document with on ID

        #region Get all document from Elasticsearch

        public static DataTable getAllDocument()
        {
            DataTable dataTable = new DataTable("character");
            dataTable.Columns.Add("account_number", typeof(string));
            dataTable.Columns.Add("firstname", typeof(string));
            dataTable.Columns.Add("lastname", typeof(string));
            dataTable.Columns.Add("age", typeof(string));

            var response = ConnectionToES.EsClient().Search<DocumentAttributes>(s => s
                .Index("accounts3")
                .Type("account3")
                .From(0)
                .Size(1000)
                .Query(q => q.MatchAll()));

            foreach (var hit in response.Hits)
            {
                dataTable.Rows.Add(hit.Id.ToString(), hit.Source.firstname.ToString(), hit.Source.lastname.ToString(), hit.Source.city.ToString());
            }

            return dataTable;
        }

        #endregion Get all document from Elasticsearch

        #region Update document based on ID

        ///Updating a Document can be done in trhee way
        ///1. Update by Partial Document
        ///2. Update by Index Query
        ///3. Update by Script
        ///Here we demonstrated Update by Partial Document and  Update by Index Query. User can choose any of these from below.
        ///Just comment one part and uncomment another.

        public static bool updateDocument(string searchID, string tbxfirstname, string tbxLastname, string tbxcity)
        {
            bool status;

            //Update by Partial Document
            var response = ConnectionToES.EsClient().Update<DocumentAttributes, UpdateDocumentAttributes>(searchID, d => d
                .Index("accounts3")
                .Type("account3")
                .Doc(new UpdateDocumentAttributes
                {

                    firstname = tbxfirstname,
                    lastname = tbxLastname,
                    city = tbxcity,
                }));


            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            getAllDocument();

            return status;
        }

        #endregion Update document based on ID

        #region Delete Document based on ID

        public static bool deleteDocument(string searchID)
        {
            bool status;

            var response = ConnectionToES.EsClient().Delete<DocumentAttributes>(searchID, d => d
                .Index("accounts3")
                .Type("account3"));

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            getAllDocument();
            return status;
        }

        #endregion Delete document based on ID
    }
}
