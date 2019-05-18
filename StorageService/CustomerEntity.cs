using Microsoft.WindowsAzure.Storage.Table;

namespace StorageService
{
    class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public CustomerEntity() { }
    }
}
