using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processors
{
    [TestClass]
    public class UtIssueTransactionProcessor : TestBase
    {
        [TestMethod]
        public async Task Process_IssueAssets_IncreaseAvailable()
        {
            var assetId = new UInt256(RandomByteArray(32));
            var oldAvailability = RandomInt(10000);
            var changeInAvailability1 = RandomInt(10000);
            var changeInAvailability2 = RandomInt(10000);
            var input = new IssueTransaction
            {
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = assetId,
                        ScriptHash = new UInt160(RandomByteArray(20)),
                        Value = new Fixed8(changeInAvailability1)
                    },
                    new TransactionOutput
                    {
                        AssetId = assetId,
                        ScriptHash = new UInt160(RandomByteArray(20)),
                        Value = new Fixed8(changeInAvailability2)
                    }
                }
            };
            var asset = new Asset
            {
                Available = new Fixed8(oldAvailability)
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetAsset(It.Is<UInt256>(u => u.Equals(assetId)))).ReturnsAsync(asset);
            var testee = AutoMockContainer.Create<IssueTransactionProcessor>();

            await testee.Process(input);

            repositoryMock.Verify(m => m.AddAsset(It.Is<Asset>(a =>
                a == asset &&
                a.Available.Equals(new Fixed8(oldAvailability + changeInAvailability1 + changeInAvailability2)))));
        }

        [TestMethod]
        public async Task Process_BurnAssets_DecreaseAvailable()
        {
            var assetId = new UInt256(RandomByteArray(32));
            var oldAvailability = RandomInt(20000, 100000);
            var changeInAvailability1 = RandomInt(10000);
            var changeInAvailability2 = RandomInt(10000);
            var prevTx1 = new Transaction
            {
                Hash = new UInt256(RandomByteArray(32)),
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = assetId,
                        ScriptHash = new UInt160(RandomByteArray(20)),
                        Value = new Fixed8(changeInAvailability1)
                    },
                    new TransactionOutput
                    {
                        AssetId = assetId,
                        ScriptHash = new UInt160(RandomByteArray(20)),
                        Value = new Fixed8(RandomInt())
                    }
                }
            };
            var prevTx2 = new Transaction
            {
                Hash = new UInt256(RandomByteArray(32)),
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = assetId,
                        ScriptHash = new UInt160(RandomByteArray(20)),
                        Value = new Fixed8(RandomInt())
                    },
                    new TransactionOutput
                    {
                        AssetId = assetId,
                        ScriptHash = new UInt160(RandomByteArray(20)),
                        Value = new Fixed8(changeInAvailability2)
                    }
                }
            };
            var input = new IssueTransaction
            {
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = prevTx1.Hash,
                        PrevIndex = 0
                    },
                    new CoinReference
                    {
                        PrevHash = prevTx2.Hash,
                        PrevIndex = 1
                    }
                }
            };
            var asset = new Asset
            {
                Available = new Fixed8(oldAvailability)
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetTransaction(It.Is<UInt256>(u => u.Equals(prevTx1.Hash))))
                .ReturnsAsync(prevTx1);
            repositoryMock.Setup(m => m.GetTransaction(It.Is<UInt256>(u => u.Equals(prevTx2.Hash))))
                .ReturnsAsync(prevTx2);
            repositoryMock.Setup(m => m.GetAsset(It.Is<UInt256>(u => u.Equals(assetId)))).ReturnsAsync(asset);
            var testee = AutoMockContainer.Create<IssueTransactionProcessor>();

            await testee.Process(input);

            repositoryMock.Verify(m => m.AddAsset(It.Is<Asset>(a =>
                a == asset &&
                a.Available.Equals(new Fixed8(oldAvailability - changeInAvailability1 - changeInAvailability2)))));
        }
    }
}