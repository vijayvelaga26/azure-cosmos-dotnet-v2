namespace DocumentDB.Samples.DocumentManagement
{
    using Shared;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections;
    

    public class Program
    {
        private static readonly string databaseName = "c3";
        private static readonly string collectionName = "leases5";

        // Read config
        private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];

        //Reusable instance of DocumentClient which represents the connection to a DocumentDB endpoint
        private static DocumentClient client;

        public static void Main(string[] args)
        {
            try
            {
                ConnectionPolicy connectionPolicy = new ConnectionPolicy();
                connectionPolicy.UserAgentSuffix = " samples-net/3";
                connectionPolicy.ConnectionMode = ConnectionMode.Direct;
                connectionPolicy.ConnectionProtocol = Protocol.Tcp;

                using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey, connectionPolicy))
                {
                    Initialize().Wait();

                    RunDocumentsDemo().Wait();
                
                }
            }
#if !DEBUG
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
#endif
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }


        private static async Task RunDocumentsDemo()
        {
            await RunBasicOperationsOnStronglyTypedObjects();


        }

        private static async Task RunBasicOperationsOnStronglyTypedObjects()
        {

            // Below method creates 3 sample documents in the above collection. Please see below for document structure. 
           await CreateDocumentsAsync();

            // Below method prints all the documtnts in the collection. 
           await GetAllProducts();


            //// Below method returns item number and company of the documents which contain a specific value in the Tags property. 
            //await GetProducts();

            //// Below method is for deleting a given tag from a document using its item number and company

            //await DeleteProducts();


            //// Below method returns the item number and company of the documents after the tag is deleted. 
            //await Finaltest();

        }


        private static async Task CreateDocumentsAsync()
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

            Console.WriteLine("\n1.1 - Creating documents");

            DocDBProduct newdoc = GetDocSample("7013374","test1");
            await client.CreateDocumentAsync(collectionUri, newdoc);

            DocDBProduct newdoc1 = GetDocSample("7013375", "test2");
            await client.CreateDocumentAsync(collectionUri, newdoc1);

            DocDBProduct newdoc2 = GetDocSample("7013376", "test3");
            await client.CreateDocumentAsync(collectionUri, newdoc2);
        }

        private static async Task GetAllProducts()
        {
            Console.WriteLine("\n1.3 - Reading all documents in a collection");

            string continuationToken = null;
            do
            {
                var feed = await client.ReadDocumentFeedAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    new FeedOptions { MaxItemCount = 10, RequestContinuation = continuationToken });
                continuationToken = feed.ResponseContinuation;
                foreach (Document document in feed)
                {
                    Console.WriteLine(document);
                }
            } while (continuationToken != null);
        }

        private static async Task GetProducts()
        {
            var prods = GetProductsThatHaveTagFromDb(new TagRequest() { Company = "MS", Tag = "BatchTest4" });
            foreach (var item in prods)
            {
                Console.WriteLine($"Product with tag:{item.ItemBase.Item},{item.ItemBase.Company}");
            }
        }

        private static async Task  DeleteProducts()
        {
            Console.WriteLine("Delete tag from 9994014");
            DeleteTagFromProduct2(new TagRemoveRequest() { Company = "MS", ItemNo = "9994014", Tag = "BatchTest4" });
            Console.WriteLine("Delete Tag done");
        }


        private static async Task Finaltest()
        {
            var prods1 = GetProductsThatHaveTagFromDb(new TagRequest() { Company = "MS", Tag = "BatchTest4" });
            foreach (var item in prods1)
            {
                Console.WriteLine($"Product with tag:{item.ItemBase.Item},{item.ItemBase.Company}");
            }
        }

        private static async Task UpdateSales()
        {
           
        }

        private static List<DocDBProduct> GetProductsThatHaveTagFromDb(TagRequest request)
        {
            var list = client.CreateDocumentQuery<DocDBProduct>( UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                new FeedOptions { EnableCrossPartitionQuery = true })
              .Where(so => so.Tags.Contains(request.Tag))
              .Where(so => so.ItemBase.Company.Equals(request.Company))              
              .AsEnumerable()
              .ToList();   

            return list;
         
        }              


        public static void DeleteTagFromProduct2(TagRemoveRequest request)
        {

            DocDBProduct doc1 = GetProductByCompanyAndItemNumber(request.Company, request.ItemNo);
            List<string> ls = doc1.Tags;
            ls.Remove(request.Tag);
            doc1.Tags = ls; 
            client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, doc1.Id), doc1).Wait();
            //Console.WriteLine("Doc after updated");
            //Console.WriteLine(doc1);
        }

        private static DocDBProduct GetProductByCompanyAndItemNumber(string company, string itemNumber)
        {
            return client.CreateDocumentQuery<DocDBProduct>(
                                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), new FeedOptions() { EnableCrossPartitionQuery = true })
                                .Where(x =>
                                    x.ItemBase.Item == itemNumber &&
                                    x.ItemBase.Company == company)
                                .AsEnumerable()
                                .FirstOrDefault() 
                                ;
        }



        public static void UpdateSalesFromDoc(UpdateSales updateSales)
        {
            DocDBProduct doc1 = GetProductByCompanyAndItemNumber(updateSales.ItemNo,updateSales.Company);            

        }


         private static DocDBProduct GetDocSample(string itemnumber, string ordername)
        {
            DocDBProduct newdoc = new DocDBProduct
            {

                ItemBase = new ItemBaseDetail
               
                    {
                        Company = "MS",
                        Item = itemnumber,
                        Uniqueitem = "MS7013374",
                        Sector = "0020",
                        Family = "7000",
                        Group = "7019",
                        Aswdescription = "RIGEL II 42W  60X60 4000K"
                    
                },
                               
                Tags = new List<string> {"Banan","BatchTest4","Test3"},
                Manual = "test data" ,                
                Certificate = "test2",
                 Done = true,
                 Published= true,
                  Modified = false,

                    Items = new SalesOrderDetail[]
                {
                    new SalesOrderDetail
                    {
                        OrderName = ordername                        
                    }
                }

            };
                       
            return newdoc;
        }




        private static async Task Initialize()
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });

           
            DocumentCollection collectionDefinition = new DocumentCollection();
            collectionDefinition.Id = collectionName;

            
            collectionDefinition.PartitionKey.Paths.Add("/_partitionKey");

            // Use the recommended indexing policy which supports range queries/sorting on strings
            collectionDefinition.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

            // Create with a throughput of 1000 RU/s
            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseName),
                collectionDefinition,
                new RequestOptions { OfferThroughput = 400 });
        }
    }
}
