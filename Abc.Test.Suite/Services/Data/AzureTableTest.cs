// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureTableTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abc.Azure;
    using Abc.Test.Suite.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    
    /// <summary>
    /// This is a test class for AzureTableTest and is intended to contain all AzureTableTest Unit Tests
    /// </summary>
    [TestClass]
    public class AzureTableTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByRowInvalidKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryByRow(StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByRowTakeInvalidKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryByRow(StringHelper.NullEmptyWhiteSpace(), 56);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByRowTakeInvalidTake()
        {
            var random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryByRow(StringHelper.NullEmptyWhiteSpace(), random.Next() * -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByPartitionNullKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryByPartition(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryByPredicateNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryBy(null, 56);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByTakeInvalidTake()
        {
            var random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryBy(d => d.PartitionKey == StringHelper.NullEmptyWhiteSpace(), random.Next() * -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AzureTableConstructorNull()
        {
            new AzureTable<Entity>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEntityNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity((Entity)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntityNullPartitionKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entity = new Entity()
            {
                PartitionKey = null,
                RowKey = StringHelper.ValidString()
            };
            table.AddEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntityInvalidRowKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entity = new Entity()
            {
                RowKey = StringHelper.NullEmptyWhiteSpace(),
                PartitionKey = StringHelper.ValidString()
            };
            table.AddEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEntitiesNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity((IEnumerable<Entity>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEntitiesEntityNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entities.Add(null);
            entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            table.AddEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntitiesNullPartitionKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                PartitionKey = null,
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            table.AddEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntitiesInvalidRowKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.NullEmptyWhiteSpace()
            };
            entities.Add(entity);
            table.AddEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateEntityNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddOrUpdateEntity((Entity)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOrUpdateEntityNullPartitionKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entity = new Entity()
            {
                PartitionKey = null,
                RowKey = StringHelper.ValidString()
            };
            table.AddOrUpdateEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOrUpdateEntityInvalidRowKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entity = new Entity()
            {
                RowKey = StringHelper.NullEmptyWhiteSpace(),
                PartitionKey = StringHelper.ValidString()
            };
            table.AddOrUpdateEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateEntitiesNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddOrUpdateEntity((IEnumerable<Entity>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateEntitiesEntityNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entities.Add(null);
            entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            table.AddOrUpdateEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOrUpdateEntitiesNullPartitionKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                PartitionKey = null,
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            table.AddOrUpdateEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOrUpdateEntitiesInvalidRowKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.NullEmptyWhiteSpace()
            };
            entities.Add(entity);
            table.AddOrUpdateEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteEntityNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteEntity((Entity)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteEntityNullPartitionKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entity = new Entity()
            {
                PartitionKey = null,
                RowKey = StringHelper.ValidString()
            };
            table.DeleteEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteEntityInvalidRowKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entity = new Entity()
            {
                RowKey = StringHelper.NullEmptyWhiteSpace(),
                PartitionKey = StringHelper.ValidString()
            };
            table.DeleteEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteEntitiesNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteEntity((IEnumerable<Entity>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteEntitiesEntityNull()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entities.Add(null);
            entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            table.DeleteEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteEntitiesNullPartitionKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                PartitionKey = null,
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            table.DeleteEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteEntitiesInvalidRowKey()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.NullEmptyWhiteSpace()
            };
            entities.Add(entity);
            table.DeleteEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntityValidationThrows()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            var entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = -1
            };
            table.AddEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntitiesValidationThrows()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var entities = new List<EntityWithDataStore>();
            var entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = 11
            };
            entities.Add(entity);
            entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = -100
            };
            entities.Add(entity);
            entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = 22
            };
            entities.Add(entity);
            table.AddEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddOrUpdateEntityValidationThrows()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            var entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = -99
            };
            table.AddOrUpdateEntity(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddOrUpdateEntitiesValidationThrows()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            var entities = new List<EntityWithDataStore>();
            var entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = 11
            };
            entities.Add(entity);
            entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = -100
            };
            entities.Add(entity);
            entity = new EntityWithDataStore()
            {
                PartitionKey = StringHelper.ValidString(),
                RowKey = StringHelper.ValidString(),
                ToTest = -22
            };
            entities.Add(entity);
            table.AddOrUpdateEntity(entities);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByNullPartitionKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryBy(null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByInvalidRowKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryBy(StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByPartitionNullPartitionKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryByPartition(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryByRowInvalidRowKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.QueryByRow(StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteByNullPartitionKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteBy(null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteByInvalidRowKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteBy(StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteByPartitionNullPartitionKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteByPartition(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteByRowInvalidRowKey()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteByRow(StringHelper.NullEmptyWhiteSpace());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void AzureTableConstructor()
        {
            new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
        }

        [TestMethod]
        public void MinimumDate()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            Assert.AreEqual<DateTime>(new DateTime(1800, 1, 1), table.MinimumDate);
        }

        [TestMethod]
        public void AzureTableCreateDelete()
        {
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();

            var cloudTableClient = new CloudTableClient(CloudStorageAccount.DevelopmentStorageAccount.TableEndpoint.ToString(), CloudStorageAccount.DevelopmentStorageAccount.Credentials);
            var success = cloudTableClient.DoesTableExist("Entity");
            Assert.IsTrue(success);

            table.DeleteIfExist();

            success = cloudTableClient.DoesTableExist("Entity");
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void AzureTableCreateDataStoreDelete()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();

            var cloudTableClient = new CloudTableClient(CloudStorageAccount.DevelopmentStorageAccount.TableEndpoint.ToString(), CloudStorageAccount.DevelopmentStorageAccount.Credentials);
            var success = cloudTableClient.DoesTableExist("testtablename");
            Assert.IsTrue(success);

            table.DeleteIfExist();

            success = cloudTableClient.DoesTableExist("testtablename");
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void AddEntityQueryBy()
        {
            Random random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();
            var entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddEntity(entity);

            var returned = table.QueryBy(entity.PartitionKey, entity.RowKey);

            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(entity.PartitionKey, returned.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, returned.RowKey);
            Assert.AreEqual<int>(entity.ToTest, returned.ToTest);
        }

        [TestMethod]
        public void AddEntityQueryRow()
        {
            Random random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();
            var entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddEntity(entity);

            var returned = table.QueryByRow(entity.RowKey).FirstOrDefault();

            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(entity.PartitionKey, returned.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, returned.RowKey);
            Assert.AreEqual<int>(entity.ToTest, returned.ToTest);
        }

        [TestMethod]
        public void AddEntityQueryPartition()
        {
            Random random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();
            var entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddEntity(entity);

            var returned = table.QueryByPartition(entity.PartitionKey);

            Assert.IsNotNull(returned);
            var items = returned.ToList();
            Assert.AreEqual<int>(1, items.Count);
            var first = returned.FirstOrDefault();
            Assert.AreEqual<string>(entity.PartitionKey, first.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, first.RowKey);
            Assert.AreEqual<int>(entity.ToTest, first.ToTest);
        }

        [TestMethod]
        public void AddEntityDeleteEntity()
        {
            Random random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();
            var entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddEntity(entity);

            var returned = table.QueryBy(entity.PartitionKey, entity.RowKey);

            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(entity.PartitionKey, returned.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, returned.RowKey);
            Assert.AreEqual<int>(entity.ToTest, returned.ToTest);

            table.DeleteEntity(entity);
        }

        [TestMethod]
        public void AddEntitiesDeleteEntities()
        {
            Random random = new Random();
            var table = new AzureTable<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            table.DeleteIfExist();
            table.EnsureExist();
            var entities = new List<Entity>();
            var entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            entities.Add(entity);
            entity = new Entity()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            entities.Add(entity);
            table.AddEntity(entities);

            var returned = from data in table.Query
                            select data;
            Assert.AreEqual<int>(entities.Count, returned.ToList().Count);

            table.DeleteEntity(entities);
        }

        [TestMethod]
        public void AddUpdateEntityDeleteEntity()
        {
            Random random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddOrUpdateEntity(entity);

            var returned = table.QueryBy(entity.PartitionKey, entity.RowKey);

            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(entity.PartitionKey, returned.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, returned.RowKey);
            Assert.AreEqual<int>(entity.ToTest, returned.ToTest);

            entity.ToTest = random.Next();

            table.AddOrUpdateEntity(entity);

            var updated = (from data in table.Query
                            where data.PartitionKey == entity.PartitionKey && data.RowKey == entity.RowKey
                            select data).SingleOrDefault();

            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(entity.PartitionKey, updated.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, updated.RowKey);
            Assert.AreEqual<int>(entity.ToTest, updated.ToTest);

            table.DeleteEntity(entity);
        }

        [TestMethod]
        public void AddUpdateEntitiesDeleteEntities()
        {
            Random random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.DeleteIfExist();
            table.EnsureExist();
            var entities = new List<EntityWithDataStore>();
            var entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            entities.Add(entity);
            entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            entities.Add(entity);
            entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            entities.Add(entity);
            table.AddOrUpdateEntity(entities);

            var returned = from data in table.Query
                           select data;
            Assert.AreEqual<int>(entities.Count, returned.ToList().Count);

            foreach (var e in entities)
            {
                e.ToTest = random.Next();
            }

            table.AddOrUpdateEntity(entities);

            table.DeleteEntity(entities);
        }

        [TestMethod]
        public void DeleteBySingle()
        {
            var random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddEntity(entity);

            table.DeleteBy(entity.PartitionKey, entity.RowKey);

            var returned = table.QueryByPartition(entity.PartitionKey);
            Assert.IsNotNull(returned);
            var list = returned.ToList();
            Assert.AreEqual<int>(0, list.Count());
        }

        [TestMethod]
        public void DeleteByMultiple()
        {
            var random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var partition = Guid.NewGuid().ToString();
            var entity1 = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = partition,
                ToTest = random.Next()
            };
            table.AddEntity(entity1);
            var entity2 = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = partition,
                ToTest = random.Next()
            };
            table.AddEntity(entity2);

            var items = table.QueryByPartition(partition);
            Assert.IsNotNull(items);
            var list = items.ToList();
            Assert.AreEqual<int>(2, list.Count());

            table.DeleteBy(entity1.PartitionKey, entity1.RowKey);
            items = table.QueryByPartition(partition);
            Assert.IsNotNull(items);
            list = items.ToList();
            Assert.AreEqual<int>(1, list.Count());

            table.DeleteBy(entity2.PartitionKey, entity2.RowKey);

            items = table.QueryByPartition(partition);
            list = items.ToList();
            Assert.AreEqual<int>(0, list.Count());
        }

        [TestMethod]
        public void DeleteByPartitionSingle()
        {
            var random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddEntity(entity);

            table.DeleteByPartition(entity.PartitionKey);

            var returned = table.QueryByPartition(entity.PartitionKey);
            Assert.IsNotNull(returned);
            var list = returned.ToList();
            Assert.AreEqual<int>(0, list.Count());
        }

        [TestMethod]
        public void DeleteByPartitionMultiple()
        {
            var random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var partition = Guid.NewGuid().ToString();
            int multiple = random.Next(1, 10);
            for (int i = 0; i < multiple; i++)
            {
                var entity = new EntityWithDataStore()
                {
                    RowKey = Guid.NewGuid().ToString(),
                    PartitionKey = partition,
                    ToTest = random.Next()
                };
                table.AddEntity(entity);
            }

            var items = table.QueryByPartition(partition);
            Assert.IsNotNull(items);
            var list = items.ToList();
            Assert.AreEqual<int>(multiple, list.Count());

            table.DeleteByPartition(partition);
            items = table.QueryByPartition(partition);
            Assert.IsNotNull(items);
            list = items.ToList();
            Assert.AreEqual<int>(0, list.Count());
        }

        [TestMethod]
        public void DeleteByRowSingle()
        {
            var random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var entity = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ToTest = random.Next()
            };
            table.AddOrUpdateEntity(entity);

            table.DeleteByRow(entity.RowKey);

            var returned = table.QueryByPartition(entity.PartitionKey);
            Assert.IsNotNull(returned);
            var list = returned.ToList();
            Assert.AreEqual<int>(0, list.Count());
        }

        [TestMethod]
        public void DeleteByRowMultiple()
        {
            var random = new Random();
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var partition = Guid.NewGuid().ToString();
            var entity1 = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = partition,
                ToTest = random.Next()
            };
            table.AddEntity(entity1);
            var entity2 = new EntityWithDataStore()
            {
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = partition,
                ToTest = random.Next()
            };
            table.AddEntity(entity2);

            var items = table.QueryByPartition(partition);
            Assert.IsNotNull(items);
            var list = items.ToList();
            Assert.AreEqual<int>(2, list.Count());

            table.DeleteByRow(entity1.RowKey);

            items = table.QueryByPartition(partition);
            Assert.IsNotNull(items);
            list = items.ToList();
            Assert.AreEqual<int>(1, list.Count());

            table.DeleteByRow(entity2.RowKey);

            items = table.QueryByPartition(partition);
            list = items.ToList();
            Assert.AreEqual<int>(0, list.Count());
        }

        [TestMethod]
        public void QueryByRow1010()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var row = Guid.NewGuid().ToString();
            var entities = new List<EntityWithDataStore>(1010);
            Parallel.For(
                0,
                1010,
                (i, loopState) =>
                {
                var entity = new EntityWithDataStore()
                {
                    RowKey = row,
                    PartitionKey = Guid.NewGuid().ToString(),
                };
                table.AddEntity(entity);
            });

            var returned = table.QueryByRow(row, 1005).ToList();
            Assert.AreEqual<int>(1005, returned.Count);
        }

        [TestMethod]
        public void QueryByRowSmallRandom()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount, new TestStoreValidator());
            table.EnsureExist();
            var row = Guid.NewGuid().ToString();
            var entities = new List<EntityWithDataStore>(1010);
            var random = new Random();
            var count = random.Next(1, 100);
            Parallel.For(
                0,
                count + 5,
                (i, loopState) =>
                {
                    var entity = new EntityWithDataStore()
                    {
                        RowKey = row,
                        PartitionKey = Guid.NewGuid().ToString(),
                    };
                    table.AddEntity(entity);
                });

            var returned = table.QueryByRow(row, count).ToList();
            Assert.AreEqual<int>(count, returned.Count);
        }

        [TestMethod]
        public void QueryBy1010()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var returned = table.QueryBy(d => d.PartitionKey == partition, 1005).ToList();
            Assert.AreEqual<int>(1005, returned.Count);
        }

        [TestMethod]
        public void QueryBy500PassWhere()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var returned = table.QueryBy(d => d.Index < 500 && d.PartitionKey == partition, 1010);
            Assert.AreEqual<int>(500, returned.AsEnumerable().Count());
        }

        [TestMethod]
        public void QueryBy10PassWhere()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var returned = table.QueryBy(d => d.Index < 10 && d.PartitionKey == partition, 1010);
            Assert.AreEqual<int>(10, returned.AsEnumerable().Count());
        }

        [TestMethod]
        public void QueryBy555PassWhere()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var returned = table.QueryBy(d => d.Index < 555 && d.PartitionKey == partition, 2000);
            Assert.AreEqual<int>(555, returned.AsEnumerable().Count());
        }

        [TestMethod]
        public void QueryByGreaterThan()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var returned = table.QueryBy(d => d.Index > 1000 && d.PartitionKey == partition, 5);
            Assert.AreEqual<int>(5, returned.AsEnumerable().Count());
            foreach (var item in returned)
            {
                Assert.IsTrue(item.Index > 1000);
            }
        }

        [TestMethod]
        public void QueryBy1009PassWhere()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var returned = table.QueryBy(d => d.Index < 1009 && d.PartitionKey == partition, 1010);
            Assert.AreEqual<int>(1009, returned.AsEnumerable().Count());
        }

        [TestMethod]
        [Ignore]
        public void QueryBySmallRandom()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            var partition = this.SetupData();
            var random = new Random();
            var count = random.Next(100);
            var returned = table.QueryBy(d => d.PartitionKey == partition, count);
            Assert.AreEqual<int>(count, returned.AsEnumerable().Count());
        }
        #endregion

        #region Helper Methods
        private string SetupData()
        {
            var table = new AzureTable<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();
            var partition = Guid.NewGuid().ToString();
            var entities = new List<EntityWithDataStore>(1010);
            Parallel.For(
                0,
                1010,
                (i, loopState) =>
                {
                    var entity = new EntityWithDataStore()
                    {
                        RowKey = Guid.NewGuid().ToString(),
                        PartitionKey = partition,
                        Hex = StringHelper.ValidString(),
                        Index = i,
                    };
                    table.AddEntity(entity);
                });

            return partition;
        }
        #endregion
    }
}