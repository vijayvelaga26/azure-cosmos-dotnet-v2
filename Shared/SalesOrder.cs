namespace DocumentDB.Samples.Shared
{
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    
    public class DocDBProduct : Resource

    {
        [JsonProperty(PropertyName = "itemBase")]
        public ItemBaseDetail ItemBase { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

         [JsonProperty(PropertyName = "manual")]
        public string Manual { get; set; }

        [JsonProperty(PropertyName = "certificate")]
        public string Certificate { get; set; }

        [JsonProperty(PropertyName = "done")]
        public bool Done { get; set; }

        [JsonProperty(PropertyName = "published")]
        public bool Published { get; set; }

        [JsonProperty(PropertyName = "modified")]
        public bool Modified { get; set; }

        [JsonProperty(PropertyName = "needToTranslate")]
        public string NeedToTranslate { get; set; }
       

    }


    public class ItemBaseDetail
    {
        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }
        [JsonProperty(PropertyName = "item")]
        public string Item { get; set; }
        [JsonProperty(PropertyName = "uniqueItem")]
        public string Uniqueitem { get; set; }
        [JsonProperty(PropertyName = "sector")]
        public string Sector { get; set; }
        [JsonProperty(PropertyName = "family")]
        public string Family { get; set; }
        [JsonProperty(PropertyName = "group")]
        public string Group { get; set; }
        [JsonProperty(PropertyName = "aswDescription")]
        public string Aswdescription { get; set; }
    }


    public class TagRemoveRequest
    {
        public string Company { get; set; }
        public string Tag { get; set; }
        public string ItemNo { get; set; }
    }

    public class TagRequest
    {
        public string Company { get; set; }
        public string Tag { get; set; }
    }

    public class ProductDetail
    {
        public string Company { get; set; }
        public string ItemNo { get; set; }
    }

       












}
