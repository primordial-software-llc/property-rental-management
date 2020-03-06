﻿using System;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using FinanceApi.Tests;
using Tests.InfrastructureAsCode;
using Xunit;

namespace Tests.InfrastructureAsCode
{
    public class CreateBankDataAccessUserTable
    {
        //[Fact]
        public void Create()
        {
            var request = new CreateTableRequest
            {
                TableName = "Finance-Users",
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "email",
                        KeyType = "HASH"
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "email",
                        AttributeType = "S"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 1
                }
            };
            var tableFactory = new DynamoDbTableFactory(
                new AmazonDynamoDBClient(Factory.CreateCredentialsFromDefaultProfile(), Factory.HomeRegion));
            tableFactory.CreateTable(request);
        }
    }
}
